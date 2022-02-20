using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Singletons;
using Unity.Netcode;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public GameObject inGameMenu;
    public GameObject errorJoiningPanel;
    public GameObject waitingForOthersPanel;
    public TextMeshProUGUI waitingForOthersPanelJoinCodeText;
    private int _playersConnected = 0;

    void Awake() {
        // Cap the frame rate
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    async void Start() {
        if (RelayManager.Instance.IsRelayEnabled) {
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
        }

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just connected.");
            _playersConnected++;

            if (NetworkManager.Singleton.IsHost) {
                waitingForOthersPanel.SetActive(false);
            } 
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            Logger.Instance.LogInfo($"player {id + 1} just disconnected.");
            _playersConnected--;
        };

        if (Application.isEditor) {
            // While in editor mode, don't wait for others
            waitingForOthersPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame) {
            inGameMenu.SetActive(true);
        }
    }
}
