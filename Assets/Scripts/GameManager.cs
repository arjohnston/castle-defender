using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Singletons;
using Unity.Netcode;
using TMPro;
using System.IO;
using System;
using UnityEngine.UI;

public class GameManager : NetworkSingleton<GameManager> {
    public GameObject inGameMenu;
    public GameObject errorJoiningPanel;
    public GameObject waitingForOthersPanel;
    public GameObject winConditionPanel;
    public TextMeshProUGUI waitingForOthersPanelJoinCodeText;
    public TextMeshProUGUI winConditionText;
    public TextMeshProUGUI winConditionSubText;
    private int _playersConnected = 0;

    public GameObject ToastMessage;
    private bool _messageIsAnimatingFadeIn = false;
    private float _messageFadeAnimationTiming = 6.0f;
    private float _messageState = 0.0f;
    private float _messageAlphaDefault = 0.6f;

    public TextMeshProUGUI currentPlayerHp;
    public TextMeshProUGUI opposingPlayerHp;
    public TextMeshProUGUI currentPlayerResources;
    public TextMeshProUGUI opposingPlayerResources;

    public Image currentPlayerImage;
    public Image opposingPlayerImage;

    public Sprite knight;
    public Sprite wizard;
    public Sprite sorceress;

    public bool allowMultiplayerInEditor = false;

    private DateTime _startTime;

    void Awake() {
        // Cap the frame rate
        Application.targetFrameRate = 60;

        // Push error messages to the custom logger
        Application.logMessageReceived += HandleException;

        // Don't display waiting for others if:
        // Unity is in editor mode
        // or the client is joining (e.g., Player 2)
        if ((Application.isEditor && !allowMultiplayerInEditor) || !string.IsNullOrEmpty(GameSettings.clientJoinCode)) {
            waitingForOthersPanel.SetActive(false);
        }
    }

    private void HandleException(string logString, string stackTrace, LogType type) {
        if (type == LogType.Exception) {
            Logger.Instance.LogError(logString);

            string path = "debug.txt";

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(logString);
            writer.Write(stackTrace + "\n");
            writer.Close();
        }
    }

    // Start is called before the first frame update
    async void Start() {
        if (RelayManager.Instance.IsRelayEnabled && (!Application.isEditor || allowMultiplayerInEditor)) {
            if (GameSettings.isLaunchingAsHost) {
                await RelayManager.Instance.SetupRelay();
                NetworkManager.Singleton.StartHost();
                waitingForOthersPanelJoinCodeText.text = GameSettings.clientJoinCode;
            } else if (!string.IsNullOrEmpty(GameSettings.clientJoinCode)) {
                try {
                    await RelayManager.Instance.JoinRelay(GameSettings.clientJoinCode);
                    NetworkManager.Singleton.StartClient();
                    waitingForOthersPanel.SetActive(false);
                    _startTime = DateTime.Now;
                } catch {
                    Logger.Instance.LogError("Unable to start or join the relay server");
                    errorJoiningPanel.SetActive(true);
                }
            } else {
                // Show an error scene
                Logger.Instance.LogError("Unable to start or join the relay server");
                errorJoiningPanel.SetActive(true);
            }
        } else {
            Logger.Instance.LogInfo("Detected Unity editor mode. Starting the game as a host without Relay enabled.");
            Logger.Instance.LogInfo("To enable multiplayer (Relay), first build, then run the executable.");
            NetworkManager.Singleton.StartHost();
            CameraController.Instance.SetStartingPosition(Players.PLAYER_ONE);
        }


        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just connected.");
            _playersConnected++;

            if (NetworkManager.Singleton.IsHost) {
                waitingForOthersPanel.SetActive(false);
                _startTime = DateTime.Now;
            }

            CameraController.Instance.SetStartingPosition(GetCurrentPlayer());
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just disconnected.");
            _playersConnected--;

            PlayerDisconnectedClientRpc(id);
        };

        SoundManager.Instance.Play(Sounds.BACKGROUND_GAME);
    }

    [ClientRpc] 
    public void PlayerDisconnectedClientRpc(ulong id) {
        if ((id == 0 || id == 1) && TurnManager.Instance.GetGameState() != GameState.WIN_CONDITION) {
            TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

            winConditionSubText.text = "Opponent disconnected";
            winConditionPanel.SetActive(true);

            if (GetCurrentPlayer() == Players.SPECTATOR) {
                winConditionSubText.text = "Player " + (id + 1) + " disconnected";

                if (id == 0) {
                    winConditionText.text = "Player 2 won";
                } else {
                    winConditionText.text = "Player 1 won";
                }
            }

            if (GetCurrentPlayer() == Players.PLAYER_ONE) AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player One");
            else AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player Two");

            FormatAndStoreTimeElapsed();
        }
        

    }

    // Update is called once per frame
    void Update() {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame) {
            inGameMenu.SetActive(true);
        }

        UpdatePlayerStats();
        CheckWinCondition();
        FadeMessage();
    }

    public void SetUIPortraits(DeckTypes type) {
        SetUIPortaitsServerRpc(GetCurrentPlayer(), type);
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetUIPortaitsServerRpc(Players player, DeckTypes type) {
        SetUIPortraitsClientRpc(player, type);
    }

    [ClientRpc]
    public void SetUIPortraitsClientRpc(Players player, DeckTypes type) {
        Sprite image = knight;

        switch (type) {
            case DeckTypes.WARRIOR:
                image = knight;
                break;

            case DeckTypes.RANGER:
                image = sorceress;
                break;

            case DeckTypes.WIZARD:
                image = wizard;
                break;
        }

        if (GetCurrentPlayer() == player) {
            currentPlayerImage.sprite = image;
        } else {
            opposingPlayerImage.sprite = image;
        }
    }

    private void FormatAndStoreTimeElapsed() {
        DateTime currentTime = DateTime.Now;
        TimeSpan diff = currentTime - _startTime;

        string hours = diff.Hours > 9 ? diff.Hours.ToString() : "0" + diff.Hours;
        string minutes = diff.Minutes > 9 ? diff.Minutes.ToString() : "0" + diff.Minutes;
        string seconds = diff.Seconds > 9 ? diff.Seconds.ToString() : "0" + diff.Seconds;

        AnalyticsManager.Instance.SetAnalytic(Analytics.TOTAL_TIME, String.Format(hours + ":" + minutes + ":" + seconds));
    }

    private void UpdatePlayerStats() {
        currentPlayerHp.text = Math.Max(GameboardObjectManager.Instance.GetCurrentPlayerCastleHealth(), 0).ToString();
        opposingPlayerHp.text = Math.Max(GameboardObjectManager.Instance.GetOpposingPlayerCastleHealth(), 0).ToString();
        currentPlayerResources.text = Math.Max(ResourceManager.Instance.GetResourcesForCurrentPlayer(), 0).ToString();
        opposingPlayerResources.text = Math.Max(ResourceManager.Instance.GetResourcesForOpposingPlayer(), 0).ToString();
    }

    public Players GetCurrentPlayer() {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.LocalClientId == 0) {
            return Players.PLAYER_ONE;
        }
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClientId == 1) {
            return Players.PLAYER_TWO;
        }

        return Players.SPECTATOR;
    }

    public Players GetOpposingPlayer() {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.LocalClientId == 0) {
            return Players.PLAYER_TWO;
        }
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClientId == 1) {
            return Players.PLAYER_ONE;
        }

        return Players.SPECTATOR;
    }

    private void CheckWinCondition() {
        int currentPlayerCastleHealth = GameboardObjectManager.Instance.GetCurrentPlayerCastleHealth();
        int opposingPlayerCastleHealth = GameboardObjectManager.Instance.GetOpposingPlayerCastleHealth();

        if (TurnManager.Instance.GetGameState() != GameState.SETUP && TurnManager.Instance.GetGameState() != GameState.WIN_CONDITION && !Application.isEditor) {
            if (currentPlayerCastleHealth <= 0) {
                // You lose
                TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

                winConditionText.text = "You lost.";
                winConditionPanel.SetActive(true);

                if (GetCurrentPlayer() == Players.PLAYER_ONE) AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player Two");
                else AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player One");

                FormatAndStoreTimeElapsed();
            }

            if (opposingPlayerCastleHealth <= 0) {
                // Opponent loses
                TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

                winConditionText.text = "You won!";
                winConditionPanel.SetActive(true);

                if (GetCurrentPlayer() == Players.PLAYER_ONE) AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player One");
                else AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, "Player Two");

                FormatAndStoreTimeElapsed();
            }

            // TODO: Handle spectator message
        }
    }

    public void ResetScene() {
        waitingForOthersPanel.SetActive(true);
    }

    [ServerRpc(RequireOwnership=false)]
    public void PlayerQuitServerRpc(ulong clientId) {
        SetPlayerQuitClientRpc(clientId);
    }

    [ClientRpc]
    public void SetPlayerQuitClientRpc(ulong clientId) {
        if (clientId <= 1) {
            winConditionText.text = NetworkManager.Singleton.LocalClientId != clientId ? "You won!" : "You lost.";
            winConditionSubText.text = NetworkManager.Singleton.LocalClientId != clientId ? "Opponent forfeited" : "You forfeited";
            winConditionPanel.SetActive(true);
            TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

            Players playerWon = NetworkManager.Singleton.LocalClientId == clientId ? GetOpposingPlayer() : GetCurrentPlayer();
            AnalyticsManager.Instance.SetAnalytic(Analytics.PLAYER_WON, playerWon == Players.PLAYER_ONE ? "Player One" : "Player Two");

            FormatAndStoreTimeElapsed();
        } else {
            // TODO: Spectator message
            //winConditionText.text = "Spectator";
            //winConditionSubText.text = "Left Game";
            //winConditionPanel.SetActive(true);
        }
    }

    public void ShowToastMessage(String message) {
        TextMeshProUGUI text = ToastMessage.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        FadeMessage(true);
    }

    private void FadeOutMessage() {
        Image image = ToastMessage.GetComponentInChildren<Image>();
        TextMeshProUGUI text = ToastMessage.GetComponentInChildren<TextMeshProUGUI>();
        Color color = image.color;
        Color textColor = text.color;

        if (_messageState > 0) {
            _messageState -= Time.deltaTime * _messageFadeAnimationTiming;

            if (_messageState <= _messageAlphaDefault) {
                color.a = _messageState;
                image.color = color;
            }

            // Text should always go from 0 -> 100%
            if (_messageState <= 1.0f) {
                textColor.a = _messageState;
                text.color = textColor;
            }
        } else {
            ToastMessage.SetActive(false);
        }
    }

    private void FadeInMessage() {
        Image image = ToastMessage.GetComponentInChildren<Image>();
        TextMeshProUGUI text = ToastMessage.GetComponentInChildren<TextMeshProUGUI>();
        Color color = image.color;
        Color textColor = text.color;

        ToastMessage.SetActive(true);

        if (_messageState <= _messageFadeAnimationTiming) {
            _messageState += Time.deltaTime * _messageFadeAnimationTiming;

            if (_messageState <= _messageAlphaDefault) {
                color.a = _messageState;
                image.color = color;
            }

            // Text should always go from 0 -> 100%
            if (_messageState <= 1.0f) {
                textColor.a = _messageState;
                text.color = textColor;
            }
        } else {
            _messageIsAnimatingFadeIn = false;
        }
    }

    private void FadeMessage(bool startFade = false) {
        if (startFade) _messageIsAnimatingFadeIn = true;

        if (_messageIsAnimatingFadeIn) {
            FadeInMessage();
        } else {
            FadeOutMessage();
        }
    }
}
