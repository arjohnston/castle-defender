using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Samples;
using System.Collections;
using System.Collections.Generic;
using System;
 using UnityEngine.EventSystems;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class GameboardObject : NetworkBehaviour {
    private NetworkVariable<Vector3> targetPositionServer = new NetworkVariable<Vector3>();
    private NetworkVariable<bool> isInstantiated = new NetworkVariable<bool>(false);
    private float animationSpeed = 8.0f;
    private float floatingHeight = 0.5f;
    private float defaultHeight = 0.0f;
    private float flyingHeight = 0.75f;
    private bool hasAttacked = false;

    private NetworkVariable<bool> isSelected = new NetworkVariable<bool>(false);

    [SerializeField]
    private NetworkVariable<Vector2Int> hexPosition = new NetworkVariable<Vector2Int>(); // Q, R (e.g., X, Y)

    [SerializeField] private NetworkVariable<Meta> meta = new NetworkVariable<Meta>();
    [SerializeField] private NetworkVariable<Attributes> attributes = new NetworkVariable<Attributes>();
    [SerializeField] private NetworkVariable<Types> gboType = new NetworkVariable<Types>();

    [SerializeField] private NetworkVariable<int> hpServer = new NetworkVariable<int>();

    [SerializeField] private NetworkVariable<int> remainingMoveActions = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> remainingAttackActions = new NetworkVariable<int>(0);

    [SerializeField] private NetworkVariable<bool> isTrapActivated = new NetworkVariable<bool>(false);
    [SerializeField] private NetworkVariable<bool> isInActivatedTrap = new NetworkVariable<bool>(false);

    [SerializeField] private NetworkList<Enchantment> enchantments;

    // Greater than 0 == in effect
    [SerializeField] private NetworkVariable<int> isStunnedDuration = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> isGroundedDuration = new NetworkVariable<int>(0);

    private Card card;

    void Awake() {
        SetEnchantmentListServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetEnchantmentListServerRpc() {
        enchantments = new NetworkList<Enchantment>(); // TODO: invalid RPC call
    }

    void Update() {
        CheckIfDestroyed();
        UpdatePosition();
    }

    public void SetupGboDetails(Card card) {
        this.card = card;

        SetGboTypeServerRpc(card.type);
        SetupGboDetailsServerRpc(card.meta, card.attributes);
        SetHpServerRpc(card.attributes.hp);

        if (GetFlying()) defaultHeight = flyingHeight;
    }

    public bool GetFlying() {
        if (!isInstantiated.Value) return false;

        if (isGroundedDuration.Value > 0) return false;

        if (enchantments != null) {
            foreach (Enchantment enchantment in enchantments) {
                if (enchantment.flying) return true;
            }
        }

        return card != null ? card.attributes.flying : false;
    }

    public void SetStunnedDuration(int duration) {
        SetStunnedDurationServerRpc(duration);
    }

    public void SetGroundedDuration(int duration) {
        SetGroundedDurationServerRpc(duration);
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetStunnedDurationServerRpc(int duration) {
        if (isStunnedDuration.Value <= 0 && duration < 0) return;

        isStunnedDuration.Value += duration;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetGroundedDurationServerRpc(int duration) {
        if (isGroundedDuration.Value <= 0 && duration < 0) return;

        isGroundedDuration.Value += duration;
    }

    public bool IsGrounded() {
        return isGroundedDuration.Value > 0;
    }

    public bool IsStunned() {
        return isStunnedDuration.Value > 0;
    }

    public void SetPosition(Hex hex) {
        transform.position = hex.Position();
        SetTargetPositionServerRpc(hex.Position());
        UpdateHexPositionServerRpc(new Vector2Int(hex.Q, hex.R));
    }

    public void MoveToPosition(Hex hex) {
        SetTargetPositionServerRpc(hex.Position());
        UpdateHexPositionServerRpc(new Vector2Int(hex.Q, hex.R));

        SetRemainingMoveActionsServerRpc(remainingMoveActions.Value - 1);
    }

    public void SetRotation(Vector3 rotation) {
        transform.Rotate(rotation);
    }

    public void Select() {
        if (!TurnManager.Instance.IsMyTurn() || isStunnedDuration.Value > 0) return;
        SetIsSelectedServerRpc(true);
    }

    public void Deselect() {
        SetIsSelectedServerRpc(false);
    }

    public void UpdatePosition() {
        if (!isInstantiated.Value) return;

        if (GetFlying()) {
            defaultHeight = flyingHeight;
        } else {
            defaultHeight = 0.0f;
        }

        Vector3 currentPosition = transform.position;
        float newHeight = defaultHeight;
    
        if (!IsSamePosition(currentPosition, targetPositionServer.Value) || isSelected.Value) {
            newHeight = defaultHeight + floatingHeight;
        } else {
            newHeight = defaultHeight;
        }

        float step = animationSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, new Vector3(targetPositionServer.Value.x, newHeight, targetPositionServer.Value.z), step);
    }

    public void CheckIfDestroyed() {
        if (!isInstantiated.Value) return;

        if (GetModifiedHp() <= 0) {
            GameboardObjectManager.Instance.DestroyGameObject(gameObject);
        }

        // Destroy if there are no enemies in trap
        if (IsTrap() && IsTrapActivated()) {
            List<Hex> hexesInRange = Gameboard.Instance.GetHexesWithinRange(GetHexPosition(), GetRange(), true);
            bool hasEnemyInRange = false;
            
            foreach (Hex hex in hexesInRange) {
                GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);
                
                if (gbo != null && !gbo.IsOwner) hasEnemyInRange = true;
            }

            if (!hasEnemyInRange) GameboardObjectManager.Instance.DestroyGameObject(gameObject);
        }
    }

    public List<Hex> GetOccupiedHexes() {
        Hex center = Gameboard.Instance.GetHexAt(hexPosition.Value);
        List<Hex> hexesInRange = Gameboard.Instance.GetHexesWithinRange(center, attributes.Value.occupiedRadius);

        return hexesInRange;
    }

    public Hex GetHexPosition() { 
        return Gameboard.Instance.GetHexAt(hexPosition.Value);
    }

    public int GetModifiedHp() {
        int modifier = GetEnchantmentModifier("health");
        return hpServer.Value + modifier;
    }

    public int GetHp() {
        return hpServer.Value;
    }

    public void SetHp(int hp) {
        if (card != null && hp + GetHp() > card.attributes.hp) SetHpServerRpc(card.attributes.hp);
        else SetHpServerRpc(GetHp() + hp);
    }

    public int GetCost() {
        return attributes.Value.cost;
    }

    public float GetSpeedModifier() {
        return attributes.Value.speedModifier;
    }

    public bool HasTrapImmunity() {
        if (enchantments != null) {
            foreach (Enchantment enchantment in enchantments) {
                if (enchantment.trapImmunity) return true;
            }
        }

        return false;
    }

    public String GetTitle() {
        return meta.Value.title;
    }

    public int GetSpeed() {
        int modifier = GetEnchantmentModifier("speed");

        // Check if trapped, if so then multiply by speed modifier
        List<Hex> area = Gameboard.Instance.GetHexesWithinRange(GetHexPosition(), attributes.Value.speed, true);
        float speedModifier = attributes.Value.speedModifier;

        foreach (Hex hex in area) {
            GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);
            if (gbo != null && gbo.IsTrap() && !gbo.IsOwner && gbo.IsTrapActivated()) speedModifier *= gbo.GetSpeedModifier();
        }

        return (int)Math.Floor(attributes.Value.speed * speedModifier * (modifier == 0 ? 1 : modifier));
    }

    public int GetRange() {
        int modifier = GetEnchantmentModifier("range");
        return Math.Max(attributes.Value.range, modifier);
    }

    public int GetDamage() {
        int modifier = GetEnchantmentModifier("damage");
        int enchantMod = GetEnchantmentModifier("enchantmentDamage");
        return attributes.Value.damage + modifier + (enchantMod * enchantments.Count);
    }

    public int GetOccupiedRadius() {
        return attributes.Value.occupiedRadius;
    }

    public bool GetSpawnGhoulEveryTurn() {
        return attributes.Value.spawnGhoulEveryTurn;
    }

    public bool IsEthereal() {
        return attributes.Value.ethereal;
    }

    public Card GetCard() {
        return card;
    }

    public int GetSpellModifier() {
        return attributes.Value.spellModifier;
    }

    public int GetRangedHealthModifier() {
        return attributes.Value.rangedHealthModifier;
    }

    public int GetMeleeHealthModifier() {
        return attributes.Value.meleeHealthModifier;
    }

    public void AddEnchantment(Enchantment enchantment) {
        if (enchantments == null) enchantments = new NetworkList<Enchantment>();
        AddEnchantmentServerRpc(enchantment);
    }

    public void RemoveEnchantment(Enchantment enchantment) {
        RemoveEnchantmentServerRpc(enchantment);
    }

    public void RemoveAllEnchantments() {
        RemoveAllEnchantmentsServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void AddEnchantmentServerRpc(Enchantment enchantment) {
        enchantments.Add(enchantment);
    }

    [ServerRpc(RequireOwnership=false)]
    public void RemoveEnchantmentServerRpc(Enchantment enchantment) {
        enchantments.Remove(enchantment);
    }

    [ServerRpc(RequireOwnership=false)]
    public void RemoveAllEnchantmentsServerRpc() {
        NetworkList<Enchantment> newList = new NetworkList<Enchantment>();
        enchantments = newList;
    }

    public int GetEnchantmentModifier(string modifier) {
        int mod = 0;
        if (enchantments == null) return mod;

        foreach (Enchantment enchantment in enchantments) {
            switch (modifier) {
                case "damage":
                    mod += enchantment.damageModifier;
                    break;

                case "health":
                    mod += enchantment.healthModifier;
                    break;

                case "range":
                    mod += enchantment.rangeModifier;
                    break;

                case "speed":
                    mod += enchantment.speedModifier;
                    break;

                case "enchantmentDamage":
                    mod += enchantment.enchantmentDamageModifier;
                    break;
            }
        }

        return mod;
    }

    public bool IsValidMovement(Hex target) {
        Hex currentPosition = Gameboard.Instance.GetHexAt(hexPosition.Value);
        int distanceToTarget = Cube.GetDistanceToHex(currentPosition, target);

        if (!TurnManager.Instance.IsMyTurn()) return false;
        
        // Permanent objects can only move during SETUP phase and placement
        // Otherwise creatures are free to move at any time
        if ((GetGboType() == Types.PERMANENT || GetGboType() == Types.TRAP) && IsValidPermanentMovement(target)) {
            return TurnManager.Instance.GetGameState() == GameState.SETUP;
        }

        if (remainingMoveActions.Value <= 0 && GetGboType() == Types.CREATURE) return false;

        return distanceToTarget <= GetSpeed();
    }

    private bool IsValidPermanentMovement(Hex target) {
        if (target != null) {
            Players player = GameManager.Instance.GetCurrentPlayer();
            
            foreach (Hex h in Gameboard.Instance.GetHexesWithinRange(target, GetOccupiedRadius())) {
                if (player == Players.PLAYER_ONE && h.R <= 0) return false;
                if (player == Players.PLAYER_TWO && h.R >= 0) return false;
            }
        }
        
        return true;
    }

    public bool IsValidAttack(GameboardObject target) {
        if (!TurnManager.Instance.IsMyTurn()) return false;
        if (remainingAttackActions.Value <= 0 && (!HasDoubleStrike() || remainingMoveActions.Value <= 0 )) return false;

        // Check entire radius of target. If any of the tiles are within range, then
        // its a valid attack
        foreach (Hex hex in target.GetOccupiedHexes()) {
            int distanceToTarget = Cube.GetDistanceToHex(GetHexPosition(), hex);
            if (distanceToTarget <= GetRange()) return true;
        }

        // Ground creatures cannot attack flying creatures by default
        if (!GetFlying() && target.GetFlying()) {
            return false;
        }

        return false;
    }

    public bool HasDoubleStrike() {
        if (enchantments != null) {
            foreach (Enchantment enchantment in enchantments) {
                if (enchantment.doubleStrike) return true;
            }
        }

        return false;
    }

    public bool CanMoveOrAttack() {
        return CanMove() || CanAttack();
    }

    public bool CanMove() {
        if (isStunnedDuration.Value > 0) return false;

        if (TurnManager.Instance.GetGameState() == GameState.SETUP) {
            return true;
        } else {
            if (gboType.Value == Types.PERMANENT || gboType.Value == Types.TRAP) return false;

            if (remainingMoveActions.Value <= 0) return false;
        }

        return true;
    }

    public bool CanAttack() {
        if (TurnManager.Instance.GetGameState() == GameState.SETUP) {
            return true;
        } else {
            if (gboType.Value == Types.PERMANENT || gboType.Value == Types.TRAP) return false;

            if (remainingAttackActions.Value <= 0 && (!HasDoubleStrike() || remainingMoveActions.Value <= 0 )) return false;

            // If there are no creatures in range, return false
            List<Hex> hexesInRange = Gameboard.Instance.GetHexesWithinRange(GetHexPosition(), GetRange(), true);
            foreach (Hex hex in hexesInRange) {
                GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);
                if (gbo != null && !gbo.IsOwner && gbo.GetGboType() != Types.TRAP) return true;
            }
        }

        return false;
    }

    public void Attack(GameboardObject target) {
        int damageModifier = 0;
        if (target.GetGboType() == Types.PERMANENT) damageModifier += attributes.Value.permanentDamageModifier;
        if (!hasAttacked) damageModifier += attributes.Value.firstAttackDamageModifier;

        int distance = Cube.GetDistanceToHex(GetHexPosition(), target.GetHexPosition());
        if (distance > 1) damageModifier += attributes.Value.rangedHealthModifier;
        else damageModifier += attributes.Value.meleeHealthModifier;

        if (damageModifier + GetDamage() <= 0) return;

        target.SetHp(-(damageModifier + GetDamage()));

        SetRemainingAttackActionsServerRpc(remainingAttackActions.Value - 1);
        hasAttacked = true;

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_TOTAL_DAMAGE, damageModifier + GetDamage());
        } else {
            AnalyticsManager.Instance.Track(Analytics.PLAYER_ONE_TOTAL_DAMAGE, damageModifier + GetDamage());
        }
    }

    public bool IsTrap() {
        return GetGboType() == Types.TRAP;
    }

    public bool IsTrapActivated() {
        return isTrapActivated.Value;
    }

    public void ActivateTrap() {
        if (!IsTrap()) return;

        SetTrapIsActivatedServerRpc(true);
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetTrapIsActivatedServerRpc(bool isActivated) {
        isTrapActivated.Value = isActivated;
        ActivateTrapClientRpc();
    }

    [ClientRpc]
    public void ActivateTrapClientRpc() {
        UseActivatedTrap();
    }

    public void UseActivatedTrapAtBeginningOfOpponentTurn() {
        if (
            IsSpawned &&
            IsTrap() && 
            !IsOwner && 
            IsTrapActivated() && 
            TurnManager.Instance.IsMyTurn()
        ) UseActivatedTrap();
    }

    private void UseActivatedTrap() {
        List<Hex> area = Gameboard.Instance.GetHexesWithinRange(GetHexPosition(), GetRange(), true);
        List<GameboardObject> affectedCreatures = new List<GameboardObject>();

        foreach (Hex hex in area) {
            GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);

            if (gbo != null && gbo.GetGboType() == Types.CREATURE && !gbo.HasTrapImmunity()) affectedCreatures.Add(gbo);
        }

        foreach (GameboardObject creature in affectedCreatures) {
            creature.SetHp(-GetDamage());
        }

        // Trap life determines the amount of turns its active for
        SetHp(-1);
    }

    public void UseDoesDamageAtStartOfTurn() {
        if (IsSpawned && IsOwner && TurnManager.Instance.IsMyTurn()) {
            int modifier = 0;

            // Get all creatures in range
            List<Hex> hexesInRange = Gameboard.Instance.GetHexesWithinRange(GetHexPosition(), GetRange(), true);
            List<GameboardObject> gbos = new List<GameboardObject>();

            foreach (Hex hex in hexesInRange) {
                GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hex);
                if (gbo != null && !gbo.IsOwner) gbos.Add(gbo);
            }

            if (enchantments != null) {
                foreach (Enchantment enchantment in enchantments) {
                    if (enchantment.doesDamageEachTurn) modifier += enchantment.damageModifier;
                }
            }

            if (modifier <= 0) return;

            // Do damage to the 0th index of creatures in range
            if (gbos.Count > 0) {
                gbos[0].SetHp(-modifier);
            }
        }
    }

    public void ResetActions() {
        SetRemainingMoveActionsServerRpc(1); // only 1 movement per round
        SetRemainingAttackActionsServerRpc(1); // only one attack per round
    }

    public void SetGboType(Types type) {
        SetGboTypeServerRpc(type);
    }

    public Types GetGboType() {
        return gboType.Value;
    }

    // https://stackoverflow.com/questions/44221679/example-of-seemingly-equal-float-variables-not-equal
    public bool IsSamePosition(Vector3 pos1, Vector3 pos2) {
        return Mathf.Abs(pos1.x - pos2.x) < 1e-5 && Mathf.Abs(pos1.z - pos2.z) < 1e-5;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetGboTypeServerRpc(Types type) {
        gboType.Value = type;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRemainingMoveActionsServerRpc(int value) {
        remainingMoveActions.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRemainingAttackActionsServerRpc(int value) {
        remainingAttackActions.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHexPositionServerRpc(Vector2Int position) {
        hexPosition.Value = position;
        isInstantiated.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetupGboDetailsServerRpc(Meta info, Attributes attr) {
        meta.Value = info;
        attributes.Value = attr;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHpServerRpc(int health) {
        hpServer.Value = health;
    }

    [ServerRpc]
    public void SetTargetPositionServerRpc(Vector3 position) {
        targetPositionServer.Value = position;
    }

    [ServerRpc]
    public void SetIsSelectedServerRpc(bool selected) {
        isSelected.Value = selected;
    }
}
