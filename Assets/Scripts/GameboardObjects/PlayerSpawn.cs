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
        Players player;
        //Players player = NetworkManager.Singleton.LocalClientId == 0 ? Players.PLAYER_ONE : Players.PLAYER_TWO;
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            player = Players.PLAYER_ONE;
        }
        else if ((NetworkManager.Singleton.LocalClientId == 1))
        {
            player = Players.PLAYER_TWO;
        }
        else
        {
            player = Players.SPECTATOR;
        }

        if ((player == Players.PLAYER_ONE) || (player == Players.PLAYER_TWO))
        {
            GameboardObjectManager.Instance.SetPlayerCastle(gameObject, player);
        }
        
    }
}
