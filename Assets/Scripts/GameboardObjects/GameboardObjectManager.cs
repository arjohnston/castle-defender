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
}
