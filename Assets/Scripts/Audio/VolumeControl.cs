using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
    [SerializeField] Slider volumeSlider;

    void Awake() {
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 1);

        Load();
    }

    public void ChangeVolume() {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
    
    private void Load() {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }
}
