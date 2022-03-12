using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerSpawn : NetworkBehaviour
{
    void Start()
    {
        Players player = NetworkManager.Singleton.LocalClientId == 0 ? Players.PLAYER_ONE : Players.PLAYER_TWO;
        GameboardObjectManager.Instance.SetPlayerCastle(gameObject, player);
    }
}
