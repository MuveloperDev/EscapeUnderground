using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Button StartBtn = null;
    [SerializeField] JsonDataController jsonDataController = null;
    [SerializeField] AudioManager startSceneAudioManager = null;
    [SerializeField] DappxAPIDataConroller dappxAPIDataConroller = null;

    private void Awake()
    {
        startSceneAudioManager = FindObjectOfType<AudioManager>();
        dappxAPIDataConroller = FindObjectOfType<DappxAPIDataConroller>();
        if (SceneManager.GetActiveScene().name == "TitleScene")
        { 
            jsonDataController = FindObjectOfType<JsonDataController>();
            StartBtn.onClick.AddListener(delegate 
            {
                if (jsonDataController.PlayerName.Length <= 0) return;
                startSceneAudioManager.SoundPlay(startSceneAudioManager.ClickSound);
                //Dappx ¿¬°á
                dappxAPIDataConroller.OnClick_StartUserSetting();
                LoadScene(); 
            });
        
        }


    }
    void LoadScene() => SceneManager.LoadScene("StartScene");

}
