using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("( Texts )")]
    [SerializeField] private TextMeshProUGUI masterClientNameText;          // 마스터 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI challengerClientNameText;      // 챌린저 클라이언트 NameText
    [SerializeField] private TextMeshProUGUI cntPlayersTxt;                 // 플레이어 수 Text
    [SerializeField] private TextMeshProUGUI roomTitleTxt;                  // 방 제목 Text

    [Header("( Buttons )")]
    [SerializeField] private Button leaveRoomBtn = null;
    [SerializeField] private Button gameStartBtn = null;

    // 사운드 매니저
    StartSceneAudioManager audioManager = null;
    private void OnEnable()
    {
        audioManager = FindObjectOfType<StartSceneAudioManager>();
        roomTitleTxt.text = PhotonNetwork.CurrentRoom.Name;
        leaveRoomBtn.onClick.AddListener(delegate { onClickLeaveRoom(); audioManager.SoundPlay(audioManager.ClickSound); });
        gameStartBtn.onClick.AddListener(delegate { OnClickLoadGameScene(); audioManager.SoundPlay(audioManager.ClickSound); });
    }

    private void Update()
    {
        cntPlayersTxt.text = $"[ {PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers} ]";
        SetRoomInfo();
    }

    // 방을 떠난다.
    void onClickLeaveRoom()
    {
        if (!PhotonNetwork.InRoom) return;
        WalletManager.Instance.GiveBackMoney();
        PhotonNetwork.LeaveRoom();
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
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsMasterClient)
                masterClientNameText.text = "MasterClient : " + PhotonNetwork.PlayerList[i].NickName.ToString() ;

            else
                challengerClientNameText.text = "ChallengerClient : " +  PhotonNetwork.PlayerList[i].NickName.ToString();
        }
    }
}
