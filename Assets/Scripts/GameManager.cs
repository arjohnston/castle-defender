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

    public bool allowMultiplayerInEditor = false;

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

        // TODO: Fade.StartAnimation(); // to defer after try/catch

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just connected.");
            _playersConnected++;

            if (NetworkManager.Singleton.IsHost) {
                waitingForOthersPanel.SetActive(false);
            }

            CameraController.Instance.SetStartingPosition(GetCurrentPlayer());
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just disconnected.");
            _playersConnected--;

            if (id == 0 || id == 1) {
                TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

                winConditionSubText.text = "Opponent disconnected";
                winConditionPanel.SetActive(true);
            }
        };
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

    private void CheckWinCondition() {
        int currentPlayerCastleHealth = GameboardObjectManager.Instance.GetCurrentPlayerCastleHealth();
        int opposingPlayerCastleHealth = GameboardObjectManager.Instance.GetOpposingPlayerCastleHealth();

        if (TurnManager.Instance.GetGameState() != GameState.SETUP && !Application.isEditor) {
            if (currentPlayerCastleHealth <= 0) {
                // You lose
                TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

                winConditionText.text = "You lost.";
                winConditionPanel.SetActive(true);
            }

            if (opposingPlayerCastleHealth <= 0) {
                // Opponent loses
                TurnManager.Instance.SetGameState(GameState.WIN_CONDITION);

                winConditionText.text = "You won!";
                winConditionPanel.SetActive(true);
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
        // TODO: Handle spectator leaving
        if (NetworkManager.Singleton.LocalClientId != clientId) {
            winConditionSubText.text = "Opponent forfeited";
            winConditionPanel.SetActive(true);
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
