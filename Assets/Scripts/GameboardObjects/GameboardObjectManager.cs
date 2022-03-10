using System.Collections;
using System.Collections.Generic;
using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameboardObjectManager : NetworkSingleton<GameboardObjectManager>
{
    private List<GameboardObject> gameboardObjects = new List<GameboardObject>();

    [SerializeField] private GameObject _castle;
    [SerializeField] public GameObject CreatureToken;

    [SerializeField] private GameObject _selectedGameboardObject = null;
    [SerializeField] private bool _leftClickMouseChanged = false;
    [SerializeField] private bool _lastLeftMouseButtonStateIsClicked = false;
    [SerializeField] private bool _isRaycastRangeValid = true;
    [SerializeField] private bool _isCardBeingDragged = false;

    // Update is called once per frame
    void Update()
    {
        if (!TurnManager.Instance.IsMyTurn()) _selectedGameboardObject = null;

        CheckOnLeftClick();
        HighlightRaycastIfSelected();
        SelectObjectAtRayCastHit();
    }

    // Required to pass the player manually as GameSettings.player was encountering race conditions on awake
    public void SetPlayerCastle(GameObject castle, Players player) {
        if (castle.GetComponent<NetworkObject>().IsOwner) Logger.Instance.LogInfo("Setting castle for player: " + player);
        NetworkObject networkObject = castle.GetComponent<NetworkObject>();
        GameboardObject gbo = castle.GetComponent<GameboardObject>();
        gameboardObjects.Add(gbo);

        if (networkObject.IsOwner) {
            _castle = castle;

            gbo.SetupGboDetails(
                Types.PERMANENT,
                new Meta{
                title = "Castle",
                description = "Player castle."
                },
                new Attributes{
                    hp = GameDefaults.CASTLE_HEALTH,
                    cost = 0,
                    speed = 1,
                    range = 10,
                    damage = 0,
                    occupiedRadius = 1,
                }
            );

            if (player == Players.PLAYER_ONE) {
                Hex hex = Gameboard.Instance.GetHexAt(-5, Gameboard.Instance.boardRadius - 2);
                gbo.SetPosition(hex);
                gbo.SetRotation(new Vector3(0, 180.0f, 0));
            } else {
                Hex hex = Gameboard.Instance.GetHexAt(5, -(Gameboard.Instance.boardRadius - 2));
                gbo.SetPosition(hex);
            }
        }
    }

    private void CheckOnLeftClick() {
        bool isClicked = Mouse.current.leftButton.isPressed;
        if (!_lastLeftMouseButtonStateIsClicked && isClicked) _leftClickMouseChanged = true;
        else _leftClickMouseChanged = false;
        _lastLeftMouseButtonStateIsClicked = Mouse.current.leftButton.isPressed;
    }

    private void SelectObjectAtRayCastHit() {
        // Should be in its own function
        if (_leftClickMouseChanged) {
            if (!TurnManager.Instance.IsMyTurn()) return;

            Hex hexRayCast = Gameboard.Instance.GetHexRayCastHit();
            GameObject gameboardObjectRayCast = Gameboard.Instance.GetGameboardObjectAtRayCastHit();

            if (hexRayCast == null && gameboardObjectRayCast == null) return;

            // Nothing selected, selecting a gameboard object
            if (_selectedGameboardObject == null && gameboardObjectRayCast != null) {
                GameboardObject gbo = gameboardObjectRayCast.GetComponent<GameboardObject>();
                NetworkObject networkObject = gameboardObjectRayCast.GetComponent<NetworkObject>();

                if (networkObject.IsOwner && gbo.CanMoveOrAttack()) {
                    _selectedGameboardObject = gameboardObjectRayCast;
                    gbo.Select();
                }

            // Game object already selected, and I'm clicking on another gameboard object
            } else if (_selectedGameboardObject != null && gameboardObjectRayCast != null) {
                // Clicking on the same gameboard object I already have selected -> deselect it
                GameboardObject targetGbo = gameboardObjectRayCast.GetComponent<GameboardObject>();
                if (_selectedGameboardObject == gameboardObjectRayCast) {
                    _selectedGameboardObject = null;
                    targetGbo.Deselect();
                }

                // If I am the owner, then select that
                if (gameboardObjectRayCast.GetComponent<NetworkObject>().IsOwner && targetGbo.CanMoveOrAttack()) {
                    GameboardObject currentGbo = _selectedGameboardObject.GetComponent<GameboardObject>();
                    if (currentGbo) currentGbo.Deselect();
                    _selectedGameboardObject = gameboardObjectRayCast;
                    targetGbo.Select();
                } else {
                    // I am not the owner, and I'm trying to select another game object -> attack
                    TryAttack(_selectedGameboardObject.GetComponent<GameboardObject>(), gameboardObjectRayCast.GetComponent<GameboardObject>());
                }

            // Since we're not targeting another gameboard object, we're targeting a hex to try and move
            } else if (_selectedGameboardObject != null && hexRayCast != null) {
                if (_selectedGameboardObject.GetComponent<NetworkObject>().IsOwner) {
                    TryMovement(_selectedGameboardObject.GetComponent<GameboardObject>(), hexRayCast);
                }
            }
        }
    }

    public bool Spawn(Hex location, Card card) {
        if (!TurnManager.Instance.IsMyTurn()) return false;
        if (GetGboAtHex(location) != null) return false;

        // TODO: Check to ensure I have enough resources
        GameObject go = Instantiate(CreatureToken, location.Position(), Quaternion.identity, this.transform);
        
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        
        GameboardObject gbo = go.GetComponent<GameboardObject>();
        gameboardObjects.Add(gbo);
        gbo.SetPosition(location);

        gbo.SetupGboDetails(
            Types.CREATURE,
            card.meta,
            card.attributes
        );

        return true;
    }

    public void HighlightRaycastIfSelected() {
        if (_selectedGameboardObject == null) {
            if (!_isCardBeingDragged) Gameboard.Instance.ClearHighlightedSpaces();
            return;
        }

        Hex raycastHitSpot = Gameboard.Instance.GetHexRayCastHit();
        GameboardObject gbo = _selectedGameboardObject.GetComponent<GameboardObject>();

        if (GetGboAtHex(raycastHitSpot) != null || !gbo.IsValidMovement(raycastHitSpot)) {
            _isRaycastRangeValid = Gameboard.Instance.HighlightRaycastHitSpot(raycastHitSpot, HexColors.INVALID_MOVE, gbo.GetOccupiedRadius());
        } else {
            _isRaycastRangeValid = Gameboard.Instance.HighlightRaycastHitSpot(raycastHitSpot, HexColors.VALID_MOVE, gbo.GetOccupiedRadius());
        }
    }

    private void TryMovement(GameboardObject gbo, Hex target) {
        // If target is off, or partially off the map
        if (!_isRaycastRangeValid) {
            gbo.Deselect();
            _selectedGameboardObject = null;
            return;
        }
        GameboardObject targetGbo = GetGboAtHex(target);
        if (targetGbo != null) {
            // If I'm the owner, then select that gbo
            if (targetGbo.gameObject.GetComponent<NetworkObject>().IsOwner) {
                _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
                _selectedGameboardObject = targetGbo.gameObject;
                _selectedGameboardObject.GetComponent<GameboardObject>().Select();
            } else {
                TryAttack(gbo, targetGbo);
            }
        } else {
            if (gbo.IsValidMovement(target)) {
                gbo.MoveToPosition(target);    
            }

            gbo.Deselect();
            _selectedGameboardObject = null;
        }
    }

    private void TryAttack(GameboardObject gbo, GameboardObject target) {
        if (!_isRaycastRangeValid) {
            if (gbo) gbo.Deselect();
            _selectedGameboardObject = null;
            return;
        }

        if (target.gameObject.GetComponent<NetworkObject>().IsOwner) {
            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = target.gameObject;
            _selectedGameboardObject.GetComponent<GameboardObject>().Select();
        } else {
            if (gbo.IsValidAttack(target.GetHexPosition())) {
                bool isTargetDestroyed = gbo.Attack(target);

                if (isTargetDestroyed) {
                    gameboardObjects.Remove(target);
                    Destroy(target.gameObject);
                }
            }

            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = null;
        }
    }

    public GameboardObject GetGboAtHex(Hex hex) {
        foreach (GameboardObject gbo in gameboardObjects) {
            foreach (Hex h in gbo.GetOccupiedHexes()) {
                if (h == hex) return gbo;
            }
        }

        return null;
    }

    public void ResetActions(Players player) {
        if (GameManager.Instance.GetCurrentPlayer() == player) {
            foreach (GameboardObject gbo in gameboardObjects) {
                if (gbo.IsOwner) {
                    gbo.ResetActions();
                }
            }
        }
    }

    public void SetIsCardBeingDragged(bool isBeingDragged) {
        _isCardBeingDragged = isBeingDragged;
    }
}
