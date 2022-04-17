using System.Collections;
using System.Collections.Generic;
using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameboardObjectManager : NetworkSingleton<GameboardObjectManager>
{
    private List<GameboardObject> gameboardObjects = new List<GameboardObject>();

    [SerializeField] private NetworkVariable<int> playerOneCastleHealth = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playerTwoCastleHealth = new NetworkVariable<int>();
    [SerializeField] public GameObject CreatureToken;
    [SerializeField] public GameObject WallPrefab;
    [SerializeField] private GameboardObject _castle;
    [SerializeField] private Transform[] points;

    [SerializeField] private GameObject _selectedGameboardObject = null;
    [SerializeField] private bool _leftClickMouseChanged = false;
    [SerializeField] private bool _lastLeftMouseButtonStateIsClicked = false;
    [SerializeField] private bool _isCardBeingDragged = false;

    private Dictionary<Hex, int> activatedTraps = new Dictionary<Hex, int>();
    private float _defaultTrapDuration = 2.0f;
    private float _trapDuration = 0.0f;

    private Hex _spawnLocation;
    private Card _spawnCard;

    private GameObject _tooltip;
    private Card _cardHovered;

    // Update is called once per frame
    void Update() {
        if (!TurnManager.Instance.IsMyTurn() && _selectedGameboardObject != null) {
            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = null;
        }
        
        CheckOnLeftClick();
        HighlightHexes();
        SelectObjectAtRayCastHit();
        UpdateCastleHealth();
        ShowTooltipOnHover();
        ShowRadiusOfActivatedTraps();
    }

    // Required to pass the player manually as GameSettings.player was encountering race conditions on awake
    public void SetPlayerCastle(GameObject castle, Players player) {
        if (castle.GetComponent<NetworkObject>().IsOwner && ((player == Players.PLAYER_ONE) || (player == Players.PLAYER_TWO))) {
            Logger.Instance.LogInfo("Setting castle for player: " + player);
        }

        NetworkObject networkObject = castle.GetComponent<NetworkObject>();
        GameboardObject gbo = castle.GetComponent<GameboardObject>();
        gameboardObjects.Add(gbo);

        if (networkObject.IsOwner) {
            _castle = gbo;

            gbo.SetupGboDetails(
                new Card(
                    Types.PERMANENT,
                    Sprites.CASTLE,
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
                    },
                    new Enchantment{},
                    new Spell{}
                )
            );
            
            if (player == Players.PLAYER_ONE) {
                Hex hex = Gameboard.Instance.GetHexAt(-5, Gameboard.Instance.boardRadius - 2);
                gbo.SetPosition(hex);
                gbo.SetRotation(new Vector3(0, 180.0f, 0));
            } else if (player == Players.PLAYER_TWO){
                Hex hex = Gameboard.Instance.GetHexAt(5, -(Gameboard.Instance.boardRadius - 2));
                gbo.SetPosition(hex);
            }

            if (player == Players.SPECTATOR) {
                gbo.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }

            if (player == Players.PLAYER_ONE) {
                SetPlayerOneCastleHealthServerRpc(GameDefaults.CASTLE_HEALTH);
            } else if (player == Players.PLAYER_TWO) {
                SetPlayerTwoCastleHealthServerRpc(GameDefaults.CASTLE_HEALTH);
            }
        }
    }

    // TODO: Can be used to show castle placement
    // Need to think through render order of highlighted hexes, though.
    private void ShowCastleAvailableMovestInSetup() {
        if (TurnManager.Instance.GetGameState() == GameState.SETUP) {
            Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

            foreach (KeyValuePair<string, Hex> kvp in Gameboard.Instance.GetGameboard()) {
                if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                    if (kvp.Value.R > 0) hitSpots.Add(kvp.Value, new HitSpot(HexColors.AVAILABLE_MOVES, 0, true));
                }

                if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
                    if (kvp.Value.R < 0) hitSpots.Add(kvp.Value, new HitSpot(HexColors.AVAILABLE_MOVES, 0, true));
                }
            }

            Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
        }
    }

    public GameboardObject GetCastle() {
        return _castle;
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

                if (networkObject.IsOwner && gbo.CanMoveOrAttack() && gbo.GetGboType() != Types.TRAP) {
                    _selectedGameboardObject = gameboardObjectRayCast;
                    gbo.Select();
                }

            // Game object already selected, and I'm clicking on another gameboard object
            } else if (_selectedGameboardObject != null && (gameboardObjectRayCast != null && !IsEnemyTrap(gameboardObjectRayCast))) {
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
                    
                    if (targetGbo.GetGboType() != Types.TRAP) {
                        _selectedGameboardObject = gameboardObjectRayCast;
                        targetGbo.Select();
                    } else {
                        _selectedGameboardObject = null;
                    }
                } else {
                    // I am not the owner, and I'm trying to select another game object -> attack
                    TryAttack(_selectedGameboardObject.GetComponent<GameboardObject>(), gameboardObjectRayCast.GetComponent<GameboardObject>());
                }

            // Since we're not targeting another gameboard object, we're targeting a hex to try and move
            } else if (_selectedGameboardObject != null && (hexRayCast != null || gameboardObjectRayCast != null && IsEnemyTrap(gameboardObjectRayCast))) {
                if (_selectedGameboardObject.GetComponent<NetworkObject>().IsOwner) {
                    TryMovement(_selectedGameboardObject.GetComponent<GameboardObject>(), hexRayCast);
                }
            }
        }
    }

    private bool IsEnemyTrap(GameObject gameObject) {
        if (gameObject == null) return false;

        GameboardObject gbo = gameObject.GetComponent<GameboardObject>();
        return gbo.GetGboType() == Types.TRAP && !gbo.IsOwner;
    }

    public void UseCard(Hex location, Card card) {
        switch (card.type) {
            case Types.CREATURE:
                SpawnCreature(location, card);
                break;

            case Types.ENCHANTMENT:
                CastEnchantment(location, card);
                break;

            case Types.TRAP:
                SpawnTrap(location, card);
                break;

            case Types.PERMANENT:
                SpawnPermanent(location, card);
                break;

            case Types.SPELL:
                CastSpell(location, card);
                break;
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_CARDS_PLAYED);
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_CARDS_PLAYED);
        }
    }

    public void SpawnCreature(Hex location, Card card) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        GameboardObject gbo = GetGboAtHex(location);
        if (gbo != null && (!gbo.IsTrap() || (gbo.IsTrap() && gbo.IsOwner))) return;
        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) return;

        _spawnLocation = location;
        _spawnCard = card;

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnCreatureServerRpc(location.Position(), clientId, card.type, card.sprite, card.meta, card.attributes, card.enchantment, card.spell);

        SoundManager.Instance.Play(Sounds.SPAWN);

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_CREATURES_PLAYED);
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_CREATURES_PLAYED);
        }
    }

    public void SpawnPermanent(Hex location, Card card) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        if (GetGboAtHex(location) != null) return;
        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) return;

        _spawnLocation = location;
        _spawnCard = card;

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnPermanentServerRpc(location.Position(), clientId, card.type, card.sprite, card.meta, card.attributes, card.enchantment, card.spell);

        SoundManager.Instance.Play(Sounds.SPAWN);
    }

    public void SpawnTrap(Hex location, Card card) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        if (GetGboAtHex(location) != null) return;
        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) return;

        _spawnLocation = location;
        _spawnCard = card;

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnTrapServerRpc(location.Position(), clientId, card.type, card.sprite, card.meta, card.attributes, card.enchantment, card.spell);

        SoundManager.Instance.Play(Sounds.SPAWN);

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_TRAPS_PLAYED);
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_TRAPS_PLAYED);
        }
    }

    private void ShowRadiusOfActivatedTraps() {
        Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

        if (_trapDuration <= 0) {
            activatedTraps = new Dictionary<Hex, int>();
        }

        foreach (KeyValuePair<Hex, int> kvp in activatedTraps) {
            if (!hitSpots.ContainsKey(kvp.Key)) hitSpots.Add(kvp.Key, new HitSpot(HexColors.INVALID_MOVE, kvp.Value, true));
        }

        if (_trapDuration > 0) {
            _trapDuration -= Time.deltaTime;
        }

        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
    }

    public void CastSpell(Hex location, Card card) {
        List<Hex> targetHexes = Gameboard.Instance.GetHexesWithinRange(location, card.attributes.occupiedRadius, true);
        List<GameboardObject> gbos = new List<GameboardObject>();

        foreach (Hex hex in targetHexes) {
            GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);
            if (gbo != null) {
                if (card.spell.validTarget == Targets.ALLY && gbo.IsOwner && !gbos.Contains(gbo)) gbos.Add(gbo);
                if (card.spell.validTarget == Targets.ENEMY && !gbo.IsOwner && !gbos.Contains(gbo)) gbos.Add(gbo);
                if (card.spell.validTarget == Targets.ANY && !gbos.Contains(gbo)) gbos.Add(gbo);
            }
        }

        switch(card.spell.spellType) {
            case SpellTypes.REMOVE_ENCHANTMENTS:
                foreach(GameboardObject gbo in gbos) {
                    gbo.RemoveAllEnchantments();
                }
                break;

            case SpellTypes.CHARGE_WALL:
                List<GameboardObject> walls = new List<GameboardObject>();

                foreach(GameboardObject gbo in gameboardObjects) {
                    if (gbo.GetGboType() == Types.PERMANENT && !gbo.IsOwner && gbo.GetTitle() == "Wall") {
                        walls.Add(gbo);
                    }
                }

                if (walls.Count > 0) {
                    GameboardObject closest = null;
                    
                    foreach (GameboardObject p in walls) {
                        if (closest == null) closest = p;
                        else if (Cube.GetDistanceToHex(p.GetHexPosition(), gbos[0].GetHexPosition()) < Cube.GetDistanceToHex(closest.GetHexPosition(), gbos[0].GetHexPosition())) {
                            closest = p;
                        }
                    }

                    foreach(GameboardObject gbo in gbos) {
                        List<Hex> surroundingHexes = Gameboard.Instance.GetHexesWithinRange(closest.GetHexPosition(), 1, true);
                        Hex closestUnoccupiedHex = null;

                        foreach (Hex hex in surroundingHexes) {
                            if (GetGboAtHex(hex) == null && closestUnoccupiedHex == null) {
                                closestUnoccupiedHex = hex;
                            } else if (GetGboAtHex(hex) == null && Cube.GetDistanceToHex(hex, gbo.GetHexPosition()) < Cube.GetDistanceToHex(closestUnoccupiedHex, gbo.GetHexPosition())) {
                                closestUnoccupiedHex = hex;
                            }
                        }

                        gbo.MoveToPosition(closestUnoccupiedHex);
                    }
                }
                break;

            case SpellTypes.HEAL:
                foreach(GameboardObject gbo in gbos) {
                    gbo.SetHp(card.spell.effectAmount + gbo.GetSpellModifier());
                }
                break;

            case SpellTypes.DAMAGE:
                foreach(GameboardObject gbo in gbos) {
                    gbo.SetHp(-(card.spell.effectAmount + gbo.GetSpellModifier()));

                    if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                        AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_TOTAL_DAMAGE, card.spell.effectAmount + gbo.GetSpellModifier());
                    } else {
                        AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_TOTAL_DAMAGE, card.spell.effectAmount + gbo.GetSpellModifier());
                    }
                }
                break;
            
            case SpellTypes.STUN:
                foreach(GameboardObject gbo in gbos) {
                    gbo.SetStunnedDuration(card.spell.effectDuration);
                }
                break;

            case SpellTypes.GROUND_FLYING_CREATURE:
                foreach(GameboardObject gbo in gbos) {
                    gbo.SetGroundedDuration(card.spell.effectDuration);
                }
                break;

            case SpellTypes.REDUCE_RESOURCE_COST_CREATURES:
                PlayerHand.Instance.SetResourceCostModifier(card.spell.effectAmount, card.spell.effectDuration);
                break;

            case SpellTypes.DAMAGE_AND_STUN:
                foreach(GameboardObject gbo in gbos) {
                    gbo.SetHp(-(card.spell.effectAmount + gbo.GetSpellModifier()));
                    gbo.SetStunnedDuration(card.spell.effectDuration);

                    if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                        AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_TOTAL_DAMAGE, card.spell.effectAmount + gbo.GetSpellModifier());
                    } else {
                        AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_TOTAL_DAMAGE, card.spell.effectAmount + gbo.GetSpellModifier());
                    }
                }
                break;
        }

        SoundManager.Instance.Play(Sounds.SPELL);

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_SPELLS_PLAYED);
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_SPELLS_PLAYED);
        }
    }

    public void CastEnchantment(Hex location, Card card) {
        GameboardObject gbo = GetGboAtHex(location);

        if (gbo == null || !gbo.IsOwner) return;

        gbo.AddEnchantment(card.enchantment);

        SoundManager.Instance.Play(Sounds.ENCHANT);

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_ENCHANTMENTS_PLAYED);
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_ENCHANTMENTS_PLAYED);
        }
    }

    public void ResetSelection() {
        if (_selectedGameboardObject == null) return;

        GameboardObject gbo = _selectedGameboardObject.GetComponent<GameboardObject>();

        if (gbo) gbo.Deselect();

        _selectedGameboardObject = null;
    }

    public void HighlightHexes() {
        Hex raycastHitSpot = Gameboard.Instance.GetHexRayCastHit();
        Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

        if (_selectedGameboardObject == null) {
            if (TurnManager.Instance.IsMyTurn()) {
                foreach (GameboardObject gbo in gameboardObjects) {
                    if (gbo != null && gbo.IsSpawned && TurnManager.Instance.IsMyTurn() && gbo.IsOwner && gbo.CanMoveOrAttack()) {
                        // Show which GBO's can move
                        if (!hitSpots.ContainsKey(gbo.GetHexPosition()) && !activatedTraps.ContainsKey(gbo.GetHexPosition())) hitSpots.Add(gbo.GetHexPosition(), new HitSpot(HexColors.CAN_MOVE, gbo.GetOccupiedRadius(), true));
                        else if (!activatedTraps.ContainsKey(gbo.GetHexPosition())) hitSpots[gbo.GetHexPosition()] = new HitSpot(HexColors.CAN_MOVE, gbo.GetOccupiedRadius(), true);
                    }
                }

                Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
            }

            return;
        }

        if (_selectedGameboardObject != null) {
            GameboardObject gbo = _selectedGameboardObject.GetComponent<GameboardObject>();

            if (gbo.CanMove()) {
                if (!hitSpots.ContainsKey(gbo.GetHexPosition())) hitSpots.Add(gbo.GetHexPosition(), new HitSpot(HexColors.AVAILABLE_MOVES, gbo.GetSpeed(), true));
                else hitSpots[gbo.GetHexPosition()] = new HitSpot(HexColors.AVAILABLE_MOVES, gbo.GetSpeed(), true);

                if (gbo.GetGboType() == Types.PERMANENT && raycastHitSpot != null) {
                    if (gbo.IsValidMovement(raycastHitSpot)) {
                        if (!hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.VALID_MOVE, gbo.GetOccupiedRadius()));
                        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
                        return;
                    } else {
                        if (raycastHitSpot != null && !hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.INVALID_MOVE, gbo.GetOccupiedRadius()));
                        
                        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
                        return;
                    }
                }
            }

            if (raycastHitSpot != null) {
                GameboardObject target = GetGboAtHex(raycastHitSpot);
                
                if (target != null && gbo.IsValidAttack(target) && !target.IsOwner && target.GetGboType() != Types.TRAP) {
                    if (!hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.VALID_ATTACK, gbo.GetOccupiedRadius()));
                    Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);

                    return;
                }

                if ((target == null || target.GetGboType() == Types.TRAP && !target.IsOwner) && gbo.IsValidMovement(raycastHitSpot)) {
                    if (!hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.VALID_MOVE, gbo.GetOccupiedRadius()));
                    Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
                    return;
                }
            }

            if (raycastHitSpot != null && !hitSpots.ContainsKey(raycastHitSpot)) hitSpots.Add(raycastHitSpot, new HitSpot(HexColors.INVALID_MOVE, gbo.GetOccupiedRadius()));
        }

        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
    }

    private void ShowTooltipOnHover() {
        Hex raycastHitSpot = Gameboard.Instance.GetHexRayCastHit();
        GameboardObject gbo = GetGboAtHex(raycastHitSpot);

        if (gbo == null || PlayerHand.Instance.IsCardDragged() || (_cardHovered != null && !_cardHovered.Equals(gbo.GetCard()))) {
            if (_tooltip != null) {
                Destroy(_tooltip);
            }

            _tooltip = null;
            _cardHovered = null;

            return;
        }

        if (_tooltip == null && gbo != null && (!gbo.IsTrap() || gbo.IsOwner)) {
            Card card = gbo.GetCard();

            if (card != null) {
                _tooltip = (GameObject)Instantiate(PlayerHand.Instance.CardHandPrefab, Vector3.zero, Quaternion.identity);
                _tooltip.transform.SetParent(PlayerHand.Instance.playerHandArea.transform, false);

                _cardHovered = card;
                card.attributes.damage = gbo.GetDamage();
                card.attributes.hp = gbo.GetModifiedHp();
                CardBuilder.Instance.BuildCard(_tooltip, card, false);

                Image[] panels = _tooltip.GetComponentsInChildren<Image>();

                // Stunned
                panels[2].gameObject.SetActive(gbo.IsStunned());

                // Grounded
                panels[3].gameObject.SetActive(gbo.IsGrounded());
            }
            
        }

        if (_tooltip != null) {
            Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            mousePosition.x *= Screen.width;
            mousePosition.y *= Screen.height;

            _tooltip.transform.position = mousePosition + new Vector2(70f, -100f);
        }
    }

    private void TryMovement(GameboardObject gbo, Hex target) {
        // If target is off, or partially off the map
        if (!Gameboard.Instance.IsValidRaycast(target, gbo.GetOccupiedRadius())) {
            gbo.Deselect();
            _selectedGameboardObject = null;
            return;
        }

        GameboardObject targetGbo = GetGboAtHex(target);
        if (targetGbo != null && !IsEnemyTrap(targetGbo.gameObject)) {
            // If I'm the owner, then select that gbo
            if (targetGbo.gameObject.GetComponent<NetworkObject>().IsOwner) {
                _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();

                if (targetGbo.gameObject != _selectedGameboardObject) {
                    _selectedGameboardObject = targetGbo.gameObject;
                    _selectedGameboardObject.GetComponent<GameboardObject>().Select();
                } else {
                    _selectedGameboardObject = null;
                }
                
            } else {
                TryAttack(gbo, targetGbo);
            }
        } else {
            if (gbo.IsValidMovement(target)) {
                gbo.MoveToPosition(target);

                if (targetGbo != null && IsEnemyTrap(targetGbo.gameObject)) {
                    targetGbo.ActivateTrap();
                    _trapDuration = _defaultTrapDuration;
                    activatedTraps.Add(targetGbo.GetHexPosition(), targetGbo.GetRange());
                    SoundManager.Instance.Play(Sounds.TRAP);
                }

                if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                    AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_NUMBER_OF_MOVES);
                } else {
                    AnalyticsManager.Instance.Track(Analytics.PLAYER_TWO_NUMBER_OF_MOVES);
                }
            } else {
                GameManager.Instance.ShowToastMessage("Invalid move");
                SoundManager.Instance.Play(Sounds.INVALID);
            }

            gbo.Deselect();
            _selectedGameboardObject = null;
        }
    }

    private void TryAttack(GameboardObject gbo, GameboardObject target) {
        if (!Gameboard.Instance.IsValidRaycast(target.GetHexPosition(), gbo.GetOccupiedRadius())) {
            if (gbo) gbo.Deselect();
            _selectedGameboardObject = null;
            return;
        }

        if (target.gameObject.GetComponent<NetworkObject>().IsOwner) {
            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = target.gameObject;
            _selectedGameboardObject.GetComponent<GameboardObject>().Select();
        } else {
            if (gbo.IsValidAttack(target)) {
                gbo.Attack(target);
                LineController.Instance.DrawAttackLine(_selectedGameboardObject.gameObject, target.gameObject);
                SoundManager.Instance.Play(Sounds.ATTACK);
            } else {
                GameManager.Instance.ShowToastMessage("Invalid attack");
                SoundManager.Instance.Play(Sounds.INVALID);
            }

            _selectedGameboardObject.GetComponent<GameboardObject>().Deselect();
            _selectedGameboardObject = null;
        }
    }

    public void DestroyGameObject(GameObject gameObject) {
        NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();

        GameboardObject gbo = gameObject.GetComponent<GameboardObject>();
        RemoveGameObjectClientRpc(networkObject.NetworkObjectId);
        DestroyServerRpc(networkObject.NetworkObjectId);
    }

    [ClientRpc]
    public void RemoveGameObjectClientRpc(ulong objectId) {
        GameObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;
        GameboardObject gbo = go.GetComponent<GameboardObject>();
        if (!gbo.IsEthereal() && gbo.IsOwner) DeckManager.Instance.AddToGrave(gbo.GetCard());
        gameboardObjects.Remove(gbo);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DestroyServerRpc(ulong objectId) {
        NetworkObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId];
        go.Despawn();
    }

    public GameboardObject GetGboAtHex(Hex hex, bool getTrap = false) {
        List<GameboardObject> gbos = new List<GameboardObject>();
        foreach (GameboardObject gbo in gameboardObjects) {
            foreach (Hex h in gbo.GetOccupiedHexes()) {
                if (h == hex) gbos.Add(gbo);
            }
        }

        foreach (GameboardObject gbo in gbos) {
            if (!getTrap && gbo.GetGboType() != Types.TRAP) return gbo;
            else if (getTrap && gbo.GetGboType() == Types.TRAP) return gbo;
        }

        if (gbos.Count > 0) return gbos[0];

        return null;
    }

    public void DoStartOfTurnActions() {
        ResetSelection();

        foreach (GameboardObject gbo in gameboardObjects) {
            if (gbo.IsTrap() && !TurnManager.Instance.IsMyTurn()) gbo.UseActivatedTrapAtBeginningOfOpponentTurn();
            
            if (TurnManager.Instance.IsMyTurn() && gbo.IsOwner) {
                gbo.UseDoesDamageAtStartOfTurn();
                gbo.SetGroundedDuration(-1);
                gbo.SetStunnedDuration(-1);
            }
        }

        PlayerHand.Instance.DecrementResourceCostModifierDuration();
    }

    public void ResetActions(Players player) {
        List<GameboardObject> gboListSpawnGhoulEveryTurn = new List<GameboardObject>();
        List<GameboardObject> gbosToRemove = new List<GameboardObject>();

        if (GameManager.Instance.GetCurrentPlayer() == player) {
            foreach (GameboardObject gbo in gameboardObjects) {
                try {
                    if (gbo.IsOwner) {
                        gbo.ResetActions();
                        if (gbo.GetSpawnGhoulEveryTurn()) gboListSpawnGhoulEveryTurn.Add(gbo);
                    }
                } catch {
                    gbosToRemove.Add(gbo);
                }
            }

            foreach (GameboardObject gbo in gbosToRemove) {
                // Destroyed by the other client
                gameboardObjects.Remove(gbo);
            }
        }

        foreach (GameboardObject gbo in gboListSpawnGhoulEveryTurn) {
            List<Hex> areaAroundCreature = Gameboard.Instance.GetHexesWithinRange(gbo.GetHexPosition(), 1, true);

            foreach (Hex hex in areaAroundCreature) {
                GameboardObject go = GetGboAtHex(hex);

                // This should not modify the collection since we're looping through right now
                if (go == null) {
                    SpawnCreature(hex, CardsLibrary.CreateGhoul());
                    return;
                }
            }
        }
    }

    public void SetIsCardBeingDragged(bool isBeingDragged) {
        _isCardBeingDragged = isBeingDragged;
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnCreatureServerRpc(Vector3 position, ulong clientId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = Instantiate(CreatureToken, position, Quaternion.Euler(new Vector3(0, clientId == 1 ? 180.0f : 0.0f, 0)));
        GameboardObject gbo = go.GetComponent<GameboardObject>();
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.ChangeOwnership(clientId);

        SpawnCreatureClientRpc(networkObject.NetworkObjectId, type, sprite, meta, attributes, enchantment, spell);
    }

    [ClientRpc]
    private void SpawnCreatureClientRpc(ulong objectId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;
        GameboardObject gbo = go.GetComponent<GameboardObject>();

        Sprite s = CardBuilder.Instance.GetSprite(sprite);
        go.GetComponentInChildren<MeshRenderer>().material.mainTexture = s.texture;
        
        gameboardObjects.Add(gbo);
        if (_spawnLocation != null) gbo.SetPosition(_spawnLocation);

        Card card = new Card(type, sprite, meta, attributes, enchantment, spell);
        gbo.SetupGboDetails(card);

        _spawnLocation = null;

        Hex location = gbo.GetHexPosition();
        GameboardObject trap = GetGboAtHex(location, true);
        if (trap != null) trap.ActivateTrap();
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnPermanentServerRpc(Vector3 position, ulong clientId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = Instantiate(WallPrefab, position, Quaternion.Euler(new Vector3(0, clientId == 1 ? 180.0f : 0.0f, 0)));
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.ChangeOwnership(clientId);

        SpawnPermanentClientRpc(networkObject.NetworkObjectId, type, sprite, meta, attributes, enchantment, spell);
    }

    [ClientRpc]
    private void SpawnPermanentClientRpc(ulong objectId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;
        GameboardObject gbo = go.GetComponent<GameboardObject>();

        Sprite s = CardBuilder.Instance.GetSprite(sprite);
        go.GetComponentInChildren<MeshRenderer>().material.mainTexture = s.texture;

        gameboardObjects.Add(gbo);

        if (_spawnLocation != null) gbo.SetPosition(_spawnLocation);

        Card card = new Card(type, sprite, meta, attributes, enchantment, spell);
        gbo.SetupGboDetails(card);

        _spawnLocation = null;
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnTrapServerRpc(Vector3 position, ulong clientId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = Instantiate(CreatureToken, position, Quaternion.Euler(new Vector3(0, clientId == 1 ? 180.0f : 0.0f, 0)));
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.ChangeOwnership(clientId);

        SpawnTrapClientRpc(networkObject.NetworkObjectId, type, sprite, meta, attributes, enchantment, spell);
    }

    [ClientRpc]
    private void SpawnTrapClientRpc(ulong objectId, Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        GameObject go = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;
        GameboardObject gbo = go.GetComponent<GameboardObject>();

        Sprite s = CardBuilder.Instance.GetSprite(sprite);
        go.GetComponentInChildren<MeshRenderer>().material.mainTexture = s.texture;
        
        gameboardObjects.Add(gbo);
        if (_spawnLocation != null) gbo.SetPosition(_spawnLocation);

        Card card = new Card(type, sprite, meta, attributes, enchantment, spell);
        gbo.SetupGboDetails(card);

        if (!gbo.IsOwner) gbo.gameObject.SetActive(false);

        _spawnCard = null;
        _spawnLocation = null;
    }
}
