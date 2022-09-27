using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneAudioManager : MonoBehaviour
{
    //public static StartSceneAudioManager instance;
    [Header("[ AudioSources ]")]
    [SerializeField] AudioSource oneShotAudioSource = null;

    [Header("[ AudioClips ]")]
    [SerializeField] AudioClip clickSound = null;

    public AudioClip ClickSound { get { return clickSound; } }

    private void Awake()
    {
        StartSceneAudioManager[] obj = FindObjectsOfType<StartSceneAudioManager>();
        if (obj.Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
    }

    // 클립을 교체해서 플레이하는 함수.
    public void SoundPlay(AudioClip clip)
    { 
        oneShotAudioSource.Stop();
        oneShotAudioSource.PlayOneShot(clip);
    }
}
