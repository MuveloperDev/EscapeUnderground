using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;


public class LoadSceneStart : MonoBehaviourPunCallbacks
{

    AudioManager audioManager = null;
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.BGMSound(audioManager.GameSceneSoundBGM);
    }
    // 게임이 끝나면 모든 클라이언트들에게 LoadStartScene을 호출한다.
    // 플레이어 모두가 방을 나가게 되면 OnLeftRoom이 콜백이 되는데 
    // 그 시점에 StartScene을 로드레벨 시킨다.

    public override void OnLeftRoom()
    {
        audioManager.BGMSound(audioManager.StartSceneSoundBGM);
        // 자기자신을 호출하여 방에 싱크된 사람들을 호출한다.
        PhotonNetwork.LoadLevel("StartScene");
    }

}
