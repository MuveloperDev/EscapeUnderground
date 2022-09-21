using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleRoomInList : MonoBehaviourPun
{
    public string roomName          { get; set; }           // 룸 이름 프로퍼티
    public string roomInPlayer      { get; set; }           // 룸 플레이어 수 프로퍼티
    public string maxRoomInPlayer   { get; set; }           // 최대 플레이어 수

    public TextMeshProUGUI roomTitleTxt         = null;     // 룸 타이틀 텍스트
    public TextMeshProUGUI RoomInPlayerCntText  = null;     // 플레이어 수 텍스트

    private void Update()
    {
        roomTitleTxt.text = roomName;
        RoomInPlayerCntText.text = roomInPlayer + " / " + maxRoomInPlayer;
    }
    public void OnClickJoinRoom() => PhotonNetwork.JoinRoom(roomName);

}
