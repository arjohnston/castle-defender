using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : MonoBehaviour {
    public void HostGame() {
        GameSettings.isLaunchingAsHost = true;
        SceneManager.LoadScene(Scenes.GAME);
    }

    public void ExitGame() {
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE) 
            Application.Quit();
        #endif
    }

    public void QuitToMainMenu() {
        GameManager.Instance.ResetScene();
        GameSettings.isLaunchingAsHost = false;
        GameSettings.clientJoinCode = "";
        SceneManager.LoadScene(Scenes.MAIN_MENU);
    }

    public void ForfeitGame() {
        GameManager.Instance.PlayerQuitServerRpc(NetworkManager.Singleton.LocalClientId);
    }
}
