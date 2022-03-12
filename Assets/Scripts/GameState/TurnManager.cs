using UnityEngine;
using UnityEngine.UI;
using Utilities.Singletons;
using TMPro;
using Unity.Netcode;

public class TurnManager: NetworkSingleton<TurnManager> {
    public TextMeshProUGUI GameStateText;
    public Button GameStateButton;

    [SerializeField] private NetworkVariable<GameState> gameStateServer = new NetworkVariable<GameState>(GameState.SETUP);
    private GameState gameState = GameState.SETUP;

    [SerializeField] private NetworkVariable<bool> readyPlayerOne = new NetworkVariable<bool>(false);
    [SerializeField] private NetworkVariable<bool> readyPlayerTwo = new NetworkVariable<bool>(false);

    public void HandleTurnPhaseButtonClicked() {
        switch(GetGameState()) {
            case GameState.SETUP:
                ReadyButtonClicked();
                break;

            case GameState.PLAYER_ONE_TURN:
                SetGameState(GameState.PLAYER_TWO_TURN);
                break;

            case GameState.PLAYER_TWO_TURN:
                SetGameState(GameState.PLAYER_ONE_TURN);
                break;

            default:
                break;
        }
    }

    private void ReadyButtonClicked() {
        GameStateButton.interactable = false;

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            SetPlayerOneReadyServerRpc(true);
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            SetPlayerTwoReadyServerRpc(true);
        }
    }

    private void StartGameIfBothPlayersReady() {
        if (GetGameState() == GameState.SETUP) {
            SetGameState(GameState.PLAYER_ONE_TURN);
            GameboardObjectManager.Instance.ResetActions(Players.PLAYER_ONE);
            GameStateText.text = "Player One";
            GameStateButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
            GameStateButton.interactable = GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE;

            DeckManager.Instance.DrawInitialHandOfCards();
            ResourceManager.Instance.SetDefaultResources();
        }
    }

    public void SetGameState(GameState state) {
        SetGameStateServerRpc(state);
    }

    public GameState GetGameState() {
        return gameState;
    }

    public bool IsMyTurn() {
        if (GetGameState() == GameState.SETUP) return true;

        if (GetGameState() == GameState.PLAYER_ONE_TURN && GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) return true;

        if (GetGameState() == GameState.PLAYER_TWO_TURN && GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) return true;

        return false;
    }

    public void UpdateClientGameState() {
        switch(GetGameState()) {
            case GameState.PLAYER_ONE_TURN:
                GameboardObjectManager.Instance.ResetActions(Players.PLAYER_ONE);
                GameStateText.text = "Player One";
                GameStateButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
                GameStateButton.interactable = GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE;
                break;

            case GameState.PLAYER_TWO_TURN:
                GameboardObjectManager.Instance.ResetActions(Players.PLAYER_TWO);
                GameStateText.text = "Player Two";
                GameStateButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
                GameStateButton.interactable = GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO;
                break;

            default:
                break;
        }

        if (IsMyTurn()) {
            DeckManager.Instance.DrawCard();
            ResourceManager.Instance.SetPlayerResourcesForTurn();

            // TODO: Expect this number to increment per turn
            // but only player one is incrementing at the moment
            // Logger.Instance.LogInfo("Current resources for " + GameManager.Instance.GetCurrentPlayer() + ": " + ResourceManager.Instance.GetResourcesForCurrentPlayer());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetGameStateServerRpc(GameState state) {
        gameStateServer.Value = state;
        SetGameStateClientRpc(state);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerOneReadyServerRpc(bool isReady) {
        readyPlayerOne.Value = isReady;

        if (readyPlayerOne.Value == true && readyPlayerTwo.Value == true) StartGameClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTwoReadyServerRpc(bool isReady) {
        readyPlayerTwo.Value = isReady;

        if (readyPlayerOne.Value == true && readyPlayerTwo.Value == true) StartGameClientRpc();
    }

    [ClientRpc]
    public void SetGameStateClientRpc(GameState state) {
        gameState = state;
        UpdateClientGameState();
    }
    
    [ClientRpc]
    public void StartGameClientRpc() {
        StartGameIfBothPlayersReady();
    }
}
