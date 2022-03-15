using System.Collections;
using System.Collections.Generic;
using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameboardObjectManager : NetworkSingleton<GameboardObjectManager>
{
    private List<GameboardObject> gameboardObjects = new List<GameboardObject>();

    [SerializeField] private NetworkVariable<int> playerOneCastleHealth = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playerTwoCastleHealth = new NetworkVariable<int>();
    [SerializeField] public GameObject CreatureToken;
    [SerializeField] private GameboardObject _castle;

    [SerializeField] private GameObject _selectedGameboardObject = null;
    [SerializeField] private bool _leftClickMouseChanged = false;
    [SerializeField] private bool _lastLeftMouseButtonStateIsClicked = false;
    [SerializeField] private bool _isRaycastRangeValid = true;
    [SerializeField] private bool _isCardBeingDragged = false;

    private Hex _spawnLocation;
    private Card _spawnCard;

    // Update is called once per frame
    void Update()
    {
        if (!TurnManager.Instance.IsMyTurn()) _selectedGameboardObject = null;

        CheckOnLeftClick();
        HighlightRaycastIfSelected();
        SelectObjectAtRayCastHit();
        UpdateCastleHealth();
    }

    // Required to pass the player manually as GameSettings.player was encountering race conditions on awake
    public void SetPlayerCastle(GameObject castle, Players player) {
        if (castle.GetComponent<NetworkObject>().IsOwner) Logger.Instance.LogInfo("Setting castle for player: " + player);
        NetworkObject networkObject = castle.GetComponent<NetworkObject>();
        GameboardObject gbo = castle.GetComponent<GameboardObject>();
        gameboardObjects.Add(gbo);

        if (networkObject.IsOwner) {
            _castle = gbo;

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

            if (player == Players.PLAYER_ONE) {
                SetPlayerOneCastleHealthServerRpc(GameDefaults.CASTLE_HEALTH);
            } else if (player == Players.PLAYER_TWO) {
                SetPlayerTwoCastleHealthServerRpc(GameDefaults.CASTLE_HEALTH);
            }
        }
    }

    public int GetCurrentPlayerCastleHealth() {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE) {
            return playerOneCastleHealth.Value;
        } else {
            return playerTwoCastleHealth.Value;
        }
    }

    public int GetOpposingPlayerCastleHealth() {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE) {
            return playerTwoCastleHealth.Value;
        } else {
            return playerOneCastleHealth.Value;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerOneCastleHealthServerRpc(int health) {
        playerOneCastleHealth.Value = health;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerTwoCastleHealthServerRpc(int health) {
        playerTwoCastleHealth.Value = health;
    }

    private void UpdateCastleHealth() {
        Players player = GameManager.Instance.GetCurrentPlayer();
        if (_castle == null) return;

        int castleHealth = _castle.GetHp();

        if (player == Players.PLAYER_ONE && castleHealth != playerOneCastleHealth.Value) {
            SetPlayerOneCastleHealthServerRpc(castleHealth);
        } else if (player == Players.PLAYER_TWO && castleHealth != playerTwoCastleHealth.Value) {
            SetPlayerTwoCastleHealthServerRpc(castleHealth);
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
                else if (gameboardObjectRayCast.GetComponent<NetworkObject>().IsOwner && targetGbo.CanMoveOrAttack()) {
                    GameboardObject currentGbo = _selectedGameboardObject.GetComponent<GameboardObject>();
                    if (currentGbo != null) currentGbo.Deselect();
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

    public void SpawnCreature(Hex location, Card card) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        if (GetGboAtHex(location) != null) return;
        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) return;

        _spawnLocation = location;
        _spawnCard = card;

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnServerRpc(location.Position(), clientId);
    }

    public void SpawnWall() {
        // TODO: Mechanics for spawning a wall
    }

    public void SpawnTrap() {
        // TODO: Mechanics for trap
    }

    public void CastSpell() {
        // TODO: Mechanics for casting a spell
    }

    public void CastEnchantment() {
        // TODO: Mechanics for casting enchantment
    }

    public void ResetSelection() {
        if (_selectedGameboardObject == null) return;

        GameboardObject gbo = _selectedGameboardObject.GetComponent<GameboardObject>();

        if (gbo) gbo.Deselect();

        _selectedGameboardObject = null;
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnServerRpc(Vector3 position, ulong clientId) {
        GameObject go = Instantiate(CreatureToken, position, Quaternion.identity);
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.ChangeOwnership(clientId);

        // TODO: Set the material

        SpawnClientRpc(networkObject.NetworkObjectId);
    }

    [ClientRpc]
    private void SpawnClientRpc(ulong objectId) {
        GameObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;
        GameboardObject gbo = go.GetComponent<GameboardObject>();
        
        gameboardObjects.Add(gbo);
        gbo.SetPosition(_spawnLocation);

        gbo.SetupGboDetails(
            Types.CREATURE,
            _spawnCard.meta,
            _spawnCard.attributes
        );

        _spawnCard = null;
        _spawnLocation = null;
    }


    public void HighlightRaycastIfSelected() {
        if (_selectedGameboardObject == null) {
            if (!_isCardBeingDragged) Gameboard.Instance.ClearHighlightedSpaces();
            return;
        }

        Hex raycastHitSpot = Gameboard.Instance.GetHexRayCastHit();
        GameboardObject gbo = _selectedGameboardObject.GetComponent<GameboardObject>();
        Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

        if (gbo.CanMove()) {
            hitSpots.Add(gbo.GetHexPosition(), new HitSpot(HexColors.AVAILABLE_MOVES, gbo.GetSpeed()));
        }

        if (raycastHitSpot != null && GetGboAtHex(raycastHitSpot) != null && gbo.IsValidAttack(GetGboAtHex(raycastHitSpot)) && !GetGboAtHex(raycastHitSpot).IsOwner) {
            if (!hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.VALID_ATTACK, gbo.GetOccupiedRadius()));
            _isRaycastRangeValid = Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
            return;
        }

        if (raycastHitSpot != null && GetGboAtHex(raycastHitSpot) == null && gbo.IsValidMovement(raycastHitSpot)) {
            if (!hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.VALID_MOVE, gbo.GetOccupiedRadius()));
            _isRaycastRangeValid = Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
            return;
        }

        if (raycastHitSpot != null && !hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.INVALID_MOVE, gbo.GetOccupiedRadius()));

        _isRaycastRangeValid = Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
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
            if (gbo.IsValidAttack(target)) { // this is returning 2 hexes away
                gbo.Attack(target);
            }

            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = null;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void DestroyServerRpc(ulong objectId) {
        NetworkObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId];
        go.Despawn();
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
