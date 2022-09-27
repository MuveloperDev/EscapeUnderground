using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Button StartBtn = null;
    [SerializeField] JsonDataController jsonDataController = null;
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        { 
            jsonDataController = FindObjectOfType<JsonDataController>();
            StartBtn.onClick.AddListener(delegate 
            {
                if (jsonDataController.PlayerName.Length <= 0) return;
                LoadScene(); 
            });
        
        }


    }
    void LoadScene() => SceneManager.LoadScene("StartScene");

}
