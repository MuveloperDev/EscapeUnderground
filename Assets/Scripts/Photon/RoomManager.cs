using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("( Texts )")]
    [SerializeField] private TextMeshProUGUI masterClientNameText = null;          // 마스터 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI challengerClientNameText = null;      // 챌린저 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI cntPlayersTxt = null;                 // 플레이어 수 Text
    [SerializeField] private TextMeshProUGUI roomTitleTxt = null;                  // 방 제목 Text


    // 사운드 매니저
    AudioManager audioManager = null;
    private void OnEnable()
    {
        audioManager = FindObjectOfType<AudioManager>();
        roomTitleTxt.text = PhotonNetwork.CurrentRoom.Name;



    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        cntPlayersTxt.text = $"[ {PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers} ]";
        SetRoomInfo();

        // 두명 이상 모이면 바로 시작.
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            OnClickLoadGameScene();
        }
    }


    // 게임씬으로 이동한다.
    void OnClickLoadGameScene()
    {
        // 서버를 경유해야 데이터 손실이 나지 않기 위함이다.
        // 또한 마스터 클라이언트가 아닐시 게임 시작을 해도 레벨이 동기화 되지 않는다.
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        PhotonNetwork.LoadLevel("GameScene");
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
            else challengerClientNameText.text = "ChallengerClient : " + player.NickName.ToString();
        }
    }
}
