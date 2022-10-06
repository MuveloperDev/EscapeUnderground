using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("[ AudioSources ]")]
    [SerializeField] AudioSource oneShotAudioSource = null;
    [SerializeField] AudioSource brickOneShotAudioSource = null;
    [SerializeField] AudioSource bgmAudioSource = null;

    [Header("[ AudioClips ]")]
    [SerializeField] AudioClip clickSound = null;
    [SerializeField] AudioClip boundSound = null;
    [SerializeField] AudioClip winSound = null;
    [SerializeField] AudioClip loseSound = null;
    [SerializeField] AudioClip startSceneSoundBGM = null;
    [SerializeField] AudioClip gameSceneSoundBGM = null;
    [SerializeField] AudioClip titleFxSound = null;
    [SerializeField] AudioClip joinRoomSound = null;
    [SerializeField] AudioClip soilBrokenSound = null;
    [SerializeField] AudioClip stoneBrokenSound = null;
    [SerializeField] AudioClip ironBrokenSound = null;


    // 프로퍼티
    public AudioClip ClickSound { get { return clickSound; } }
    public AudioClip BoundSound { get { return boundSound; } }
    public AudioClip WinSound { get { return winSound; } }
    public AudioClip LoseSound { get { return loseSound; } }
    public AudioClip StartSceneSoundBGM { get { return startSceneSoundBGM; } }
    public AudioClip GameSceneSoundBGM { get { return gameSceneSoundBGM; } }
    public AudioClip TitleFxSound { get { return titleFxSound; } }
    public AudioClip JoinRoomSound { get { return joinRoomSound; } }
    public AudioClip SoilBrokenSound { get { return soilBrokenSound; } }
    public AudioClip StoneBrokenSound { get { return stoneBrokenSound; } }
    public AudioClip IronBrokenSound { get { return ironBrokenSound; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BGMSound(AudioClip clip)
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }
    public void SoundPlay(AudioClip clip)
    {
        oneShotAudioSource.Stop();
        oneShotAudioSource.PlayOneShot(clip);
    }
    public void BrickSoundPlay(AudioClip clip)
    {
        brickOneShotAudioSource.PlayOneShot(clip);
    }

}
