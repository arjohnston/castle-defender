using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public void NextButtonPressed() {
        Tutorial.Instance.BtnNext();
    }

    public void PrevButtonPressed() {
        Tutorial.Instance.BtnPrev();
    }
}
