using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void HostGame() {
        // RelayManager.Instance.IsLaunchingAsHost = true;
        GameSettings.isLaunchingAsHost = true;
        SceneManager.LoadScene(Scenes.GAME);
    }

    public void ExitGame() {
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE) 
            Application.Quit();
        #elif (UNITY_WEBGL)
            Application.OpenURL("about:blank");
        #endif
    }

    public void QuitToMainMenu() {
        SceneManager.LoadScene(Scenes.MAIN_MENU);
    }
}
