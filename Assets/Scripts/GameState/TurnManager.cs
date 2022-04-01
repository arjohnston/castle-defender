using UnityEngine;
using UnityEngine.UI;
using Utilities.Singletons;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class TurnManager: NetworkSingleton<TurnManager> {
    public TextMeshProUGUI GameStateText;
    public Button GameStateButton;
    public GameObject Banner;
    private bool _bannerIsAnimatingFadeIn = false;
    private float _bannerFadeAnimationTiming = 1.3f;
    private float _bannerState = 0.0f;

    [SerializeField] private NetworkVariable<GameState> gameStateServer = new NetworkVariable<GameState>(GameState.SETUP);

    [SerializeField] private NetworkVariable<bool> readyPlayerOne = new NetworkVariable<bool>(false);
    [SerializeField] private NetworkVariable<bool> readyPlayerTwo = new NetworkVariable<bool>(false);
    [SerializeField] private NetworkVariable<bool> hasGameStarted = new NetworkVariable<bool>(false);

    private bool _hasTurnChanged = false;
    private GameState _prevGameState = GameState.SETUP;

    void Update() {
        CheckOnPlayerTurnChange();
        UpdateClientGameState();
        FadeBanner();
    }

    public void HandleTurnPhaseButtonClicked() {
        switch(gameStateServer.Value) {
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
        return gameStateServer.Value;
    }

    public bool IsMyTurn() {
        if (gameStateServer.Value == GameState.SETUP) return true;

        if (gameStateServer.Value == GameState.PLAYER_ONE_TURN && GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            return true;
        }

        if (gameStateServer.Value == GameState.PLAYER_TWO_TURN && GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            return true;
        }

        return false;
    }

    public void UpdateClientGameState() {
        switch(gameStateServer.Value) {
            case GameState.PLAYER_ONE_TURN:
                if (_hasTurnChanged) GameboardObjectManager.Instance.ResetActions(Players.PLAYER_ONE);

                GameStateText.text = "Player One";
                GameStateButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
                GameStateButton.interactable = GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE;
                break;

            case GameState.PLAYER_TWO_TURN:
                if (_hasTurnChanged)  GameboardObjectManager.Instance.ResetActions(Players.PLAYER_TWO);

                GameStateText.text = "Player Two";
                GameStateButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
                GameStateButton.interactable = GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO;
                SetHasGameStartedServerRpc(true);
                break;

            default:
                break;
        }

        if (IsMyTurn()) {
            Banner.GetComponentInChildren<TextMeshProUGUI>().text = "Your Turn";
        } else {
            Banner.GetComponentInChildren<TextMeshProUGUI>().text = "Opponent's Turn";
        }

        if (_hasTurnChanged) {
            // Weird hack to ensure that p1 doesn't draw at the start, but we do start drawing once p2 goes
            if ((hasGameStarted.Value || gameStateServer.Value == GameState.PLAYER_TWO_TURN) && IsMyTurn()) {
                DeckManager.Instance.DrawCard();
                ResourceManager.Instance.SetPlayerResourcesForTurn();
                AnalyticsManager.Instance.IncrementAnalytic(Analytics.AMOUNT_OF_TURNS, 1);
            }

            if (Application.isEditor && !GameManager.Instance.allowMultiplayerInEditor && gameStateServer.Value == GameState.PLAYER_TWO_TURN) {
                SetGameStateServerRpc(GameState.PLAYER_ONE_TURN);
            }

            GameboardObjectManager.Instance.DoStartOfTurnActions();

            FadeBanner(true);
        }
    }

    private void CheckOnPlayerTurnChange() {
        if (_prevGameState != gameStateServer.Value) _hasTurnChanged = true;
        else _hasTurnChanged = false;
        _prevGameState = gameStateServer.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetGameStateServerRpc(GameState state) {
        gameStateServer.Value = state;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerOneReadyServerRpc(bool isReady) {
        readyPlayerOne.Value = isReady;

        if ((readyPlayerOne.Value == true && readyPlayerTwo.Value == true) || Application.isEditor) StartGameClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTwoReadyServerRpc(bool isReady) {
        readyPlayerTwo.Value = isReady;

        if ((readyPlayerOne.Value == true && readyPlayerTwo.Value == true) || Application.isEditor) StartGameClientRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetHasGameStartedServerRpc(bool hasStarted) {
        hasGameStarted.Value = hasStarted;
    }
    
    [ClientRpc]
    public void StartGameClientRpc() {
        StartGameIfBothPlayersReady();
    }

    private void FadeOutBanner() {
        Image image = Banner.GetComponentInChildren<Image>();
        TextMeshProUGUI text = Banner.GetComponentInChildren<TextMeshProUGUI>();
        Color color = image.color;
        Color textColor = text.color;

        if (_bannerState > 0) {
            _bannerState -= Time.deltaTime * _bannerFadeAnimationTiming;

            if (_bannerState <= 1.0f) {
                color.a = _bannerState;
                image.color = color;
                textColor.a = _bannerState;
                text.color = textColor;
            }
        } else {
            Banner.SetActive(false);
        }
    }

    private void FadeInBanner() {
        Image image = Banner.GetComponentInChildren<Image>();
        TextMeshProUGUI text = Banner.GetComponentInChildren<TextMeshProUGUI>();
        Color color = image.color;
        Color textColor = text.color;

        Banner.SetActive(true);

        if (_bannerState <= _bannerFadeAnimationTiming) {
            _bannerState += Time.deltaTime * _bannerFadeAnimationTiming;

            if (_bannerState <= 1.0f) {
                color.a = _bannerState;
                image.color = color;
                textColor.a = _bannerState;
                text.color = textColor;
            }
        } else {
            _bannerIsAnimatingFadeIn = false;
        }
    }

    private void FadeBanner(bool startFade = false) {
        if (startFade) _bannerIsAnimatingFadeIn = true;

        if (_bannerIsAnimatingFadeIn) {
            FadeInBanner();
        } else {
            FadeOutBanner();
        }
    }
}
