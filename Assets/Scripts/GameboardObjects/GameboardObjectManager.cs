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

    [SerializeField] private GameObject _selectedGameboardObject = null;
    [SerializeField] private bool _leftClickMouseChanged = false;
    [SerializeField] private bool _lastLeftMouseButtonStateIsClicked = false;
    [SerializeField] private bool _isRaycastRangeValid = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
                GameDefaults.CASTLE_HEALTH,
                new Meta{
                title = "Castle",
                description = "Player castle."
                },
                new Attributes{
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

    public void Spawn() {
        // gameboardObjects.Add(gbo);
    }

    public void HighlightRaycastIfSelected() {
        if (_selectedGameboardObject == null) {
            Gameboard.Instance.ClearHighlightedSpaces();
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

    private GameboardObject GetGboAtHex(Hex hex) {
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























    // TODO: Look at video 4 and replicate how there is the spawning pool
    // also like how the singleton works...
    // public void Spawn() {
    //     if (IsClient) SpawnServerRpc();
    // }
    
    // [ServerRpc(RequireOwnership = false)]
    // public void SpawnServerRpc() {
    //     // if (!IsServer) return;

    //     Vector3 position = new Vector3(0, 0, 0);
    //     obj1 = (GameObject)Instantiate(creaturePrefab, position, Quaternion.identity);
    //     obj1.GetComponent<NetworkObject>().Spawn();

    //     // obj1.GetComponent<NetworkObject>().Spawn();
    //     // SpawnServerRpc(obj1);
    //     // obj1.GetComponentInChildren<GameboardObject>().MoveTo(position);
    // }

    // public void Move() {
    //     // obj1.GetComponentInChildren<GameboardObject>().MoveTo(new Vector3(10, 0, 10));
    //     if (IsClient) MoveServerRpc();
    //     // obj1.GetComponentInChildren<GameboardObject>().MoveTo(new Vector3(10, 0, 10));
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void MoveServerRpc() {
    //     Debug.Log("Called!");
    //     obj1.GetComponentInChildren<GameboardObject>().MoveToServer(new Vector3(10, 0, 10));
    // }

    // [ServerRpc]
    // private void SpawnServerRpc(GameObject go) {
        // Vector3 position = new Vector3(0, 0, 0);
        // obj1 = (GameObject)Instantiate(creaturePrefab, position, Quaternion.identity);
        // go.GetComponent<NetworkObject>().Spawn();
    // }









    // private void HandleCreatureMovement(Hex target) {
    //     int distanceToHex = Cube.GetDistanceToHex(selectedHex, target);
    //     bool isTargetWithinRangeMovement = selectedHex != null ? ((Creature)selectedHex.GetGameboardObject()).attributes.speed >= distanceToHex : true;
    //     bool isTargetWithinRangeAttack = selectedHex != null ? ((Creature)selectedHex.GetGameboardObject()).attributes.range >= distanceToHex : false;

    //     HighlightRaycastHitSpot(target, isTargetWithinRangeMovement, 0);

    //     // Toggle selection of occupied gameboard piece
    //     if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON) && !IsRaycastTargetUI()) {
    //         // We're already targeting, so select the target game object
    //         if (selectedHex != null) {
    //             if (isTargetWithinRangeMovement) {
    //                 // if we're already targetting but we're now wanting to target a new creature
    //                 if (target.IsOccupied()) {
    //                     if (this.selectedHex != target && isTargetWithinRangeAttack) {
    //                         gboManager.Attack(selectedHex.GetGameboardObject(), target.GetGameboardObject());
    //                         this.selectedHex = null;
    //                         ClearHighlightedSpaces();
    //                     }
    //                 // else we're targeting an empty cell to move the currently selected creature to
    //                 } else {
    //                     gboManager.Move(selectedHex.GetGameboardObject(), target);
    //                     ClearHighlightedSpaces();
    //                 }

    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //             } else {
    //                 // If attempting to click on a spot that's not in range of currently selected creature, then deselect
    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //                 ClearHighlightedSpaces();
    //             }
                
    //         } else if (target.IsOccupied()) {
    //             if (gboManager.Select(target.GetGameboardObject())) this.selectedHex = target;
    //         }
    //     }
    // }

    // private void HandlePermanentMovement(Hex target) {
    //     int radius = 0;
    //     bool isWithinRange = true;

    //     if (selectedHex != null && selectedHex.GetGameboardObject() != null) {
    //         Permanent p = selectedHex.GetGameboardObject() as Permanent;
    //         radius = p.GetRadius();
            
    //         // Clamp castle on each players side
    //         if (p.GetHex().R < 0 && target.R >= 0 || p.GetHex().R > 0 && target.R <= 0) isWithinRange = false;
    //     }

    //     HighlightRaycastHitSpot(target, isWithinRange, radius);

    //     if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON) && !IsRaycastTargetUI()) {
    //         if (selectedHex != null) {
    //             // Move it
    //             if ((!target.IsOccupied() || target.GetGameboardObject() == selectedHex.GetGameboardObject()) && isWithinRange && isRaycastValidMovement) {
    //                 // we can move there
    //                 gboManager.Move(selectedHex.GetGameboardObject(), target);
    //                 selectedHex = null;
    //                 gboManager.DeselectAll();
    //             } else {
    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //                 ClearHighlightedSpaces();
    //             }
    //         } else {
    //             // Select it
    //             if (target.IsOccupied()) {
    //                 if (gboManager.Select(target.GetGameboardObject())) selectedHex = target;
    //             }
    //         }
    //     }
    // }
}
