using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JoinGame : MonoBehaviour
{
    public GameObject inputField;
    public GameObject joinGameScene;
    public GameObject errorScene;

    public void TryJoinGame() {
        string text = inputField.GetComponent<TMP_InputField>().text;
 
        GameSettings.clientJoinCode = text;
        SceneManager.LoadScene(Scenes.GAME);
    }
}
