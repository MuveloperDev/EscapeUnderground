using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("[ AudioSources ]")]
    [SerializeField] AudioSource oneShotAudioSource = null;

    [SerializeField] AudioSource bgmAudioSource = null;

    [Header("[ AudioClips ]")]
    [SerializeField] AudioClip clickSound = null;
    [SerializeField] AudioClip boundSound = null;
    [SerializeField] AudioClip winSound = null;
    [SerializeField] AudioClip loseSound = null;
    [SerializeField] AudioClip startSceneSoundBGM = null;
    [SerializeField] AudioClip gameSceneSoundBGM = null;

    public AudioClip ClickSound { get { return clickSound; } }
    public AudioClip BoundSound { get { return boundSound; } }
    public AudioClip WinSound { get { return winSound; } }
    public AudioClip LoseSound { get { return loseSound; } }
    public AudioClip StartSceneSoundBGM { get { return startSceneSoundBGM; } }
    public AudioClip GameSceneSoundBGM { get { return gameSceneSoundBGM; } }

    private void Awake()
    {
        AudioManager[] obj = FindObjectsOfType<AudioManager>();
        if (obj.Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
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


}
