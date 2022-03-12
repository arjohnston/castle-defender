using UnityEngine;
using UnityEngine.UI;
using Utilities.Singletons;
using TMPro;
using Unity.Netcode;

public class ResourceManager: NetworkSingleton<ResourceManager> {
    [SerializeField] private NetworkVariable<int> playerOneCurrentResources = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoCurrentResources = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerOneMaxResources = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoMaxResources = new NetworkVariable<int>(0);

    public void SetDefaultResources() {
        SetDefaultResourcesServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetDefaultResourcesServerRpc() {
        playerOneCurrentResources.Value = GameDefaults.STARTING_RESOURCES;
        playerTwoCurrentResources.Value = GameDefaults.STARTING_RESOURCES;
        playerOneMaxResources.Value = GameDefaults.STARTING_RESOURCES;
        playerTwoMaxResources.Value = GameDefaults.STARTING_RESOURCES;
    }

    public void SetPlayerResourcesForTurn() {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE) {
            SetPlayerOneResourcesAtStartOfTurnServerRpc();
        } else {
            SetPlayerTwoResourcesAtStartOfTurnServerRpc();
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerOneResourcesAtStartOfTurnServerRpc() {
        int currentMaxResources = playerOneMaxResources.Value;
        if (playerOneMaxResources.Value < GameDefaults.MAX_RESOURCES)
            currentMaxResources += GameDefaults.GAIN_AMOUNT_OF_RESOURCES_PER_TURN;

        playerOneMaxResources.Value = currentMaxResources;
        playerOneCurrentResources.Value = currentMaxResources;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerTwoResourcesAtStartOfTurnServerRpc() {
        int currentMaxResources = playerTwoMaxResources.Value;
        if (playerTwoMaxResources.Value < GameDefaults.MAX_RESOURCES)
            currentMaxResources += GameDefaults.GAIN_AMOUNT_OF_RESOURCES_PER_TURN;

        playerTwoMaxResources.Value = currentMaxResources;
        playerTwoCurrentResources.Value = currentMaxResources;
    }

    public bool HaveEnoughResources(int quantity) {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE && playerOneCurrentResources.Value >= quantity) return true;

        if (player == Players.PLAYER_TWO && playerTwoCurrentResources.Value >= quantity) return true;
    
        return false;
    }

    public void UseResource(int quantity) {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE) {
            SetPlayerOneResourcesServerRpc(-quantity);
        } else {
            SetPlayerTwoResourcesServerRpc(-quantity);
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerOneResourcesServerRpc(int quantity) {
        playerOneCurrentResources.Value += quantity;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerTwoResourcesServerRpc(int quantity) {
        playerTwoCurrentResources.Value += quantity;
    }

    public int GetResourcesForCurrentPlayer() {
        Players player = GameManager.Instance.GetCurrentPlayer();
        return GetResourcesForPlayer(player);
    }

    public int GetResourcesForOpposingPlayer() {
        Players player = GameManager.Instance.GetCurrentPlayer();

        if (player == Players.PLAYER_ONE) {
            return playerTwoCurrentResources.Value;
        } else {
            return playerOneCurrentResources.Value;
        }
    }

    public int GetResourcesForPlayer(Players player) {
        if (player == Players.PLAYER_ONE) {
            return playerOneCurrentResources.Value;
        } else {
            return playerTwoCurrentResources.Value;
        }
    }
}