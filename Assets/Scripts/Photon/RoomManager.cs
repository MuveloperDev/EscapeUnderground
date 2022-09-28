using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("( Texts )")]
    [SerializeField] private TextMeshProUGUI masterClientNameText = null;          // 마스터 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI challengerClientNameText = null;      // 챌린저 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI cntPlayersTxt = null;                 // 플레이어 수 Text
    [SerializeField] private TextMeshProUGUI roomTitleTxt = null;                  // 방 제목 Text


    public string ChallengerClientNameText { set { challengerClientNameText.text = value; } }
    public string CntPlayersTxt { set { cntPlayersTxt.text = value; } }
    // 사운드 매니저
    AudioManager audioManager = null;
    private void OnEnable()
    {
        audioManager = FindObjectOfType<AudioManager>();
        roomTitleTxt.text = PhotonNetwork.CurrentRoom.Name;
        SetRoomInfo();
    }

    // 게임씬으로 이동한다.
    void OnClickLoadGameScene()
    {
        // 서버를 경유해야 데이터 손실이 나지 않기 위함이다.
        // 또한 마스터 클라이언트가 아닐시 게임 시작을 해도 레벨이 동기화 되지 않는다.
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedROom");
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2) PhotonNetwork.LoadLevel("GameScene");
    }

    // 방정보를 업데이트 한다.
    void SetRoomInfo()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                masterClientNameText.text = "MasterClient : " + player.NickName.ToString();
                return;
            }
            if (player.IsMasterClient) masterClientNameText.text = "MasterClient : " + player.NickName.ToString();
        }
    }
}
