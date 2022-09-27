using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Button StartBtn = null;
    [SerializeField] JsonDataController jsonDataController = null;
    [SerializeField] StartSceneAudioManager startSceneAudioManager = null;
    private void Awake()
    {
        startSceneAudioManager = FindObjectOfType<StartSceneAudioManager>();
        if (SceneManager.GetActiveScene().name == "TitleScene")
        { 
            jsonDataController = FindObjectOfType<JsonDataController>();
            StartBtn.onClick.AddListener(delegate 
            {
                if (jsonDataController.PlayerName.Length <= 0) return;
                startSceneAudioManager.SoundPlay(startSceneAudioManager.ClickSound);
                LoadScene(); 
            });
        
        }


    }
    void LoadScene() => SceneManager.LoadScene("StartScene");

}
