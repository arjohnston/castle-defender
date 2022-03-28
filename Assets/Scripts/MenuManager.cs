using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() {
        SoundManager.Instance.Play(Sounds.BACKGROUND_MENU);
    }
}
