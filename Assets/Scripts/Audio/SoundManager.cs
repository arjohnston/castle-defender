using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;


public class SoundManager : Singleton<SoundManager> {
    // The sound manager will be used to control all sounds in the game (UI elements, monster attacks, etc)
    private Dictionary<int, Audio> ui;
    public float masterVolume = 0.4f;

    private AudioSource backgroundSource;
    private AudioSource singlePlaySource;
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;
    public AudioClip menuButtonHover;
    public AudioClip spawn;
    public AudioClip spell;
    public AudioClip enchant;
    public AudioClip attack;
    public AudioClip trap;
    public AudioClip invalid;

    // Start is called before the first frame update
    void Awake() {
        InitializeBackground();
        InitializeSingleSound();
    }

    void Update() {
        masterVolume = PlayerPrefs.GetFloat("musicVolume");
        backgroundSource.volume = masterVolume;
        singlePlaySource.volume = masterVolume;
    }

    private void InitializeBackground() {
        GameObject gameManager = GameObject.Find("GameManager");
        backgroundSource = gameManager.AddComponent<AudioSource>();
        backgroundSource.volume = masterVolume;
    }

    private void InitializeSingleSound() {
        GameObject gameManager = GameObject.Find("GameManager");
        singlePlaySource = gameManager.AddComponent<AudioSource>();
        singlePlaySource.volume = masterVolume;
    }

    public void Play(Sounds sound) {
        if (backgroundSource == null) InitializeBackground();
        if (singlePlaySource == null) InitializeSingleSound();

        switch (sound) {
            case Sounds.BACKGROUND_GAME:
                backgroundSource.clip = backgroundMusic;
                backgroundSource.loop = true;
                backgroundSource.Play();
                break;

            case Sounds.BACKGROUND_MENU:
                backgroundSource.clip = menuMusic;
                backgroundSource.loop = true;
                backgroundSource.Play();
                break;

            case Sounds.MENU_BUTTON_HOVER:
                singlePlaySource.clip = menuButtonHover;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.SPAWN:
                singlePlaySource.clip = spawn;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.SPELL:
                singlePlaySource.clip = spell;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.ENCHANT:
                singlePlaySource.clip = enchant;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.ATTACK:
                singlePlaySource.clip = attack;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.TRAP:
                singlePlaySource.clip = trap;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;

            case Sounds.INVALID:
                singlePlaySource.clip = invalid;
                singlePlaySource.loop = false;
                singlePlaySource.Play();
                break;
        }
    }
}