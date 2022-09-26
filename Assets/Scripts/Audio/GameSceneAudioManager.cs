using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSceneAudioManager : MonoBehaviour
{
    [Header("[ AudioSources ]")]
    [SerializeField] AudioSource oneShotAudioSource = null;
    [SerializeField] AudioSource bgmAudioSource = null;

    [Header("[ AudioClips ]")]
    [SerializeField] AudioClip boundSound = null;
    [SerializeField] AudioClip winSound = null;
    [SerializeField] AudioClip loseSound = null;


    public AudioClip BoundSound { get { return boundSound; }}
    public AudioClip WinSound { get { return winSound; } }
    public AudioClip LoseSound { get { return loseSound; } }


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
