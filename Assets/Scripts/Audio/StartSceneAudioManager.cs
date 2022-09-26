using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneAudioManager : MonoBehaviour
{
    [Header("[ AudioSources ]")]
    [SerializeField] AudioSource oneShotAudioSource = null;

    [Header("[ AudioClips ]")]
    [SerializeField] AudioClip clickSound = null;

    public AudioClip ClickSound { get { return clickSound; } }

    // 클립을 교체해서 플레이하는 함수.
    public void SoundPlay(AudioClip clip)
    { 
        oneShotAudioSource.Stop();
        oneShotAudioSource.PlayOneShot(clip);
    }
}
