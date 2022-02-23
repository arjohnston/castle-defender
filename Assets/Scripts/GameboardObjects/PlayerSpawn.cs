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

        if (IsOwner) {
            if (player == Players.PLAYER_ONE) {
                Hex hex = Gameboard.Instance.GetHexAt(-5, Gameboard.Instance.boardRadius - 2);
                transform.position = Gameboard.Instance.GetHexGameObject(hex).transform.position;
                transform.Rotate(0, 180f, 0);
            } else {
                Hex hex = Gameboard.Instance.GetHexAt(5, -(Gameboard.Instance.boardRadius - 2));
                transform.position = Gameboard.Instance.GetHexGameObject(hex).transform.position;
            }
        }


        // if (networkObject.IsOwner && player == Players.PLAYER_ONE) {
        //     Hex hex = Gameboard.Instance.GetHexAt(-5, Gameboard.Instance.boardRadius - 2);
        //     // Logger.Instance.LogInfo("P1 Hex: " + hex);
        //     castle.transform.position = Gameboard.Instance.GetHexGameObject(hex).transform.position;
        //     _castle = castle;
        // } else if (networkObject.IsOwner && player == Players.PLAYER_TWO) {
        //     Hex hex = Gameboard.Instance.GetHexAt(5, -(Gameboard.Instance.boardRadius - 2));
        //     // Logger.Instance.LogInfo("P2 Hex: " + hex);

        //     // TODO: Needs to be sent to the server to update!
        //     // Otherwise the commented code has been verified to work.
        //     castle.transform.position = Gameboard.Instance.GetHexGameObject(hex).transform.position;
        //     // Logger.Instance.LogInfo("GO " + Gameboard.Instance.GetHexGameObject(hex));
        //     // Logger.Instance.LogInfo("GO POS" + Gameboard.Instance.GetHexGameObject(hex).transform.position);
        //     _castle = castle;
        // }
    }
}
