using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelect : MonoBehaviour
{
    public void SelectDeckOne () {
        DeckManager.Instance.InitializeDeck(new Ranger());
        GameManager.Instance.SetUIPortraits(DeckTypes.RANGER);
    }

    public void SelectDeckTwo () {
        DeckManager.Instance.InitializeDeck(new Warrior());
        GameManager.Instance.SetUIPortraits(DeckTypes.WARRIOR);
    }

    public void SelectDeckThree () {
        DeckManager.Instance.InitializeDeck(new Wizard());
        GameManager.Instance.SetUIPortraits(DeckTypes.WIZARD);
    }
}
