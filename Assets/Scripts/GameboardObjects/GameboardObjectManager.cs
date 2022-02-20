using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;

public class GameboardObjectManager : NetworkSingleton<GameboardObjectManager>
{
    [SerializeField]
    public GameObject creaturePrefab;

    // [SerializeField]
    GameObject obj1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: Look at video 4 and replicate how there is the spawning pool
    // also like how the singleton works...
    public void Spawn() {
        if (IsClient) SpawnServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc() {
        // if (!IsServer) return;

        Vector3 position = new Vector3(0, 0, 0);
        obj1 = (GameObject)Instantiate(creaturePrefab, position, Quaternion.identity);
        obj1.GetComponent<NetworkObject>().Spawn();

        // obj1.GetComponent<NetworkObject>().Spawn();
        // SpawnServerRpc(obj1);
        // obj1.GetComponentInChildren<GameboardObject>().MoveTo(position);
    }

    public void Move() {
        // obj1.GetComponentInChildren<GameboardObject>().MoveTo(new Vector3(10, 0, 10));
        if (IsClient) MoveServerRpc();
        // obj1.GetComponentInChildren<GameboardObject>().MoveTo(new Vector3(10, 0, 10));
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveServerRpc() {
        Debug.Log("Called!");
        obj1.GetComponentInChildren<GameboardObject>().MoveToServer(new Vector3(10, 0, 10));
    }

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
