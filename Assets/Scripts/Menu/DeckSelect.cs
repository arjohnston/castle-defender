using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelect : MonoBehaviour
{
    public void SelectDeckOne () {
        DeckManager.Instance.InitializeDeck(new Ranger());
    }

    public void SelectDeckTwo () {
        DeckManager.Instance.InitializeDeck(new Warrior());
    }

    public void SelectDeckThree () {
        DeckManager.Instance.InitializeDeck(new Wizard());
    }
}
