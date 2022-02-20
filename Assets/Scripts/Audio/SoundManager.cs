using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;

public class SoundManager : Singleton<SoundManager> {
    // The sound manager will be used to control all sounds in the game (UI elements, monster attacks, etc)
    private Dictionary<int, Audio> ui;
    public float MasterVolume = 100.0f;

    private AudioSource source;
    public AudioClip backgroundMusic;

    // Start is called before the first frame update
    void Start() {
        Initialize();
    }

    private void Initialize() {
        // Debug.Log("Sound init");
        GameObject gameManager = GameObject.Find("GameManager");
        source = gameManager.AddComponent<AudioSource>();
        source.clip = backgroundMusic;
        source.loop = true;
        source.volume = MasterVolume;
        source.Play();
    }
}