using Unity.Netcode;
using UnityEngine;

public class GameboardObject : NetworkBehaviour
{
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
    private static float animationSpeed = 8.0f;

    private Vector3 targetClientPosition;
    private Vector3 currentClientPosition;

    public void MoveTo(Vector3 position) {
        targetClientPosition = position;
    }

    void Update() {
        if (IsServer) UpdateServer();
        if (IsClient && IsOwner) UpdateClient();
    }

    private void UpdateServer() {
        transform.position = position.Value;
    }

    private void UpdateClient() {
        if (currentClientPosition != targetClientPosition) {
            float step = animationSpeed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(currentClientPosition, targetClientPosition, step);
            currentClientPosition = newPosition;

            UpdatePositionServerRpc(newPosition);
        }
    }

    public void MoveToServer(Vector3 newPosition) {
        if (IsServer) {
            position.Value = newPosition;
        }
    }

    [ServerRpc]
    public void UpdatePositionServerRpc(Vector3 newPosition) {
        position.Value = newPosition;
    }
}
