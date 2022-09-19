using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleRoomInList : MonoBehaviourPun
{

    public string roomName { get; set; }
    public string roomInPlayer { get; set; }
    public string maxRoomInPlayer { get; set; }

    public TextMeshProUGUI roomTitleTxt = null;
    public TextMeshProUGUI RoomInPlayerCntText = null;

    private void OnEnable()
    {
        //roomTitleTxt.text = roomName;
        //RoomInPlayerCntText.text = roomInPlayer + " / " + maxRoomInPlayer;
    }

    private void Update()
    {
        roomTitleTxt.text = roomName;
        RoomInPlayerCntText.text = roomInPlayer + " / " + maxRoomInPlayer;
    }
    public void OnClickJoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
