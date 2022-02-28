using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Samples;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class GameboardObject : NetworkBehaviour
{
    private Vector3 targetPosition;
    private float animationSpeed = 8.0f;
    private float floatingHeight = 0.5f;
    private bool isSelected = false;

    [SerializeField]
    private NetworkVariable<Vector2Int> hexPosition = new NetworkVariable<Vector2Int>(); // Q, R (e.g., X, Y)

    [SerializeField] private NetworkVariable<Meta> meta = new NetworkVariable<Meta>();
    [SerializeField] private NetworkVariable<Attributes> attributes = new NetworkVariable<Attributes>();
    [SerializeField] private NetworkVariable<Types> gboType = new NetworkVariable<Types>();

    [SerializeField] private NetworkVariable<int> hpServer = new NetworkVariable<int>();
    private int hp;

    [SerializeField] private NetworkVariable<int> remainingMoveActions = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> remainingAttackActions = new NetworkVariable<int>(0);

    void Start() {

    }

    void Update() {
        UpdatePosition();
    }

    public void SetupGboDetails(Types type, int health, Meta info, Attributes attr) {
        SetGboTypeServerRpc(type);
        SetupGboDetailsServerRpc(info, attr);
        SetHpServerRpc(health);
        hp = health;
    }

    public void SetPosition(Hex hex) {
        transform.position = hex.Position();
        targetPosition = hex.Position();
        UpdateHexPositionServerRpc(new Vector2Int(hex.Q, hex.R));
    }

    public void MoveToPosition(Hex hex) {
        targetPosition = hex.Position();
        UpdateHexPositionServerRpc(new Vector2Int(hex.Q, hex.R));

        SetRemainingMoveActionsServerRpc(remainingMoveActions.Value - 1);
    }

    public void SetRotation(Vector3 rotation) {
        transform.Rotate(rotation);
    }

    public void Select() {
        if (!TurnManager.Instance.IsMyTurn()) return;
        this.isSelected = true;
        this.targetPosition.y = floatingHeight;        
    }

    public void Deselect() {
        this.isSelected = false;
    }

    public void UpdatePosition() {
        Vector3 currentPosition = transform.position;
    
        if (currentPosition.x == targetPosition.x && currentPosition.z == targetPosition.z) {
            if (currentPosition.y > 0 && !isSelected) targetPosition.y = 0;
            else if (currentPosition.y == 0 && isSelected) targetPosition.y = floatingHeight;
        } else {
            targetPosition.y = floatingHeight;
        }

        if (!IsOwner) targetPosition.y = 0;

        if (currentPosition == targetPosition) return;

        float step = animationSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);
    }

    public List<Hex> GetOccupiedHexes() {
        Hex center = Gameboard.Instance.GetHexAt(hexPosition.Value);
        List<Hex> hexesInRange = Gameboard.Instance.GetHexesWithinRange(center, attributes.Value.occupiedRadius);

        return hexesInRange;
    }

    public Hex GetHexPosition() { 
        return Gameboard.Instance.GetHexAt(hexPosition.Value);
    }

    public int GetHp() {
        return hp;
    }

    public void SetHp(int hp) {
        SetHpServerRpc(hp);
    }

    public int GetCost() {
        return attributes.Value.cost;
    }

    public int GetSpeed() {
        return attributes.Value.speed;
    }

    public int GetRange() {
        return attributes.Value.range;
    }

    public int GetDamage() {
        return attributes.Value.damage;
    }

    public int GetOccupiedRadius() {
        return attributes.Value.occupiedRadius;
    }

    public bool IsValidMovement(Hex target) {
        Hex currentPosition = Gameboard.Instance.GetHexAt(hexPosition.Value);
        int distanceToTarget = Cube.GetDistanceToHex(currentPosition, target);

        if (!TurnManager.Instance.IsMyTurn()) return false;
        
        // Permanent objects can only move during SETUP phase and placement
        // Otherwise creatures are free to move at any time
        if (GetGboType() == Types.PERMANENT) {
            return TurnManager.Instance.GetGameState() == GameState.SETUP;
        }

        if (remainingMoveActions.Value <= 0 && GetGboType() == Types.CREATURE) return false;

        return distanceToTarget <= GetSpeed();
    }

    public bool IsValidAttack(Hex target) {
        Hex currentPosition = Gameboard.Instance.GetHexAt(hexPosition.Value);
        int distanceToTarget = Cube.GetDistanceToHex(currentPosition, target);

        if (!TurnManager.Instance.IsMyTurn()) return false;

        if (remainingAttackActions.Value <= 0) return false;

        return distanceToTarget <= GetRange();
    }

    public bool CanMoveOrAttack() {
        if (TurnManager.Instance.GetGameState() == GameState.SETUP) {
            return true;
        } else {
            if (gboType.Value == Types.PERMANENT) return false;

            if (remainingAttackActions.Value <= 0 && remainingMoveActions.Value <= 0) return false;
        }

        return true;
    }

    public bool Attack(GameboardObject target) {
        int targetHp = target.GetHp() - attributes.Value.damage;
        target.SetHp(targetHp);

        SetRemainingAttackActionsServerRpc(remainingAttackActions.Value - 1);

        return targetHp <= 0;
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
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetupGboDetailsServerRpc(Meta info, Attributes attr) {
        meta.Value = info;
        attributes.Value = attr;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHpServerRpc(int health) {
        hpServer.Value = health;
        SetHpClientRpc(health);
    }

    [ClientRpc]
    public void SetHpClientRpc(int health) {
        hp = health;
    }
}
