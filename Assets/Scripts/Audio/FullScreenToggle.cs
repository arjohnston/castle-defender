using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public void Fullscene(bool is_fullscene)
    {
        Screen.fullScreen = is_fullscene;
    }
}
