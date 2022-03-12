using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Singletons;
using Unity.Netcode;
using TMPro;
using System.IO;

public class GameManager : NetworkSingleton<GameManager>
{
    public GameObject inGameMenu;
    public GameObject errorJoiningPanel;
    public GameObject waitingForOthersPanel;
    public TextMeshProUGUI waitingForOthersPanelJoinCodeText;
    private int _playersConnected = 0;

    public TextMeshProUGUI currentPlayerHp;
    public TextMeshProUGUI opposingPlayerHp;
    public TextMeshProUGUI currentPlayerResources;
    public TextMeshProUGUI opposingPlayerResources;

    void Awake() {
        // Cap the frame rate
        Application.targetFrameRate = 60;
        Application.logMessageReceived += HandleException;

        // Don't display waiting for others if:
        // Unity is in editor mode
        // or the client is joining (e.g., Player 2)
        if (Application.isEditor || !string.IsNullOrEmpty(GameSettings.clientJoinCode)) {
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
        if (RelayManager.Instance.IsRelayEnabled && !Application.isEditor) {
            if (GameSettings.isLaunchingAsHost) {
                await RelayManager.Instance.SetupRelay();
                NetworkManager.Singleton.StartHost();
                waitingForOthersPanelJoinCodeText.text = GameSettings.clientJoinCode;
            } else if (!string.IsNullOrEmpty(GameSettings.clientJoinCode)) {
                await RelayManager.Instance.JoinRelay(GameSettings.clientJoinCode);
                NetworkManager.Singleton.StartClient();
                waitingForOthersPanel.SetActive(false);
            } else {
                // Show an error scene
                Logger.Instance.LogError("Unable to start or join the relay server");
                errorJoiningPanel.SetActive(true);
            }
        } else {
            Logger.Instance.LogInfo("Detected Unity editor mode. Starting the game as a host without Relay enabled.");
            Logger.Instance.LogInfo("To enable multiplayer (Relay), first build, then run the executable.");
            NetworkManager.Singleton.StartHost();
        }

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just connected.");
            _playersConnected++;

            if (NetworkManager.Singleton.IsHost) {
                waitingForOthersPanel.SetActive(false);
            }

            CameraController.Instance.SetStartingPosition();
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just disconnected.");
            _playersConnected--;
        };
    }

    // Update is called once per frame
    void Update() {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame) {
            inGameMenu.SetActive(true);
        }

        UpdatePlayerStats();
    }

    private void UpdatePlayerStats() {
        currentPlayerHp.text = "40";
        opposingPlayerHp.text = "40";
        currentPlayerResources.text = ResourceManager.Instance.GetResourcesForCurrentPlayer().ToString();
        opposingPlayerResources.text = ResourceManager.Instance.GetResourcesForOpposingPlayer().ToString();
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
}
