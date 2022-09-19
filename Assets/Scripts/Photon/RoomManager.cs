using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("( Texts )")]
    [SerializeField] private TextMeshProUGUI masterClientNameText;
    [SerializeField] private TextMeshProUGUI challengerClientNameText;
    [SerializeField] private TextMeshProUGUI cntPlayersTxt;
    [SerializeField] private TextMeshProUGUI roomTitleTxt;


    private void OnEnable()
    {
        roomTitleTxt.text = PhotonNetwork.CurrentRoom.Name;
        SetRoomInfo();
    }




    private void Update()
    {
        cntPlayersTxt.text = $"[ {PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers} ]";
        SetRoomInfo();
    }

    // 방정보를 업데이트 한다.
    void SetRoomInfo()
    {
        //if(!PhotonNetwork.InRoom) return;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsMasterClient)
            {
                masterClientNameText.text = "MasterClient : " + PhotonNetwork.PlayerList[i].NickName.ToString() ;
            }
            else
            {
                challengerClientNameText.text = "ChallengerClient : " +  PhotonNetwork.PlayerList[i].NickName.ToString();
            }
        }
    }
}
