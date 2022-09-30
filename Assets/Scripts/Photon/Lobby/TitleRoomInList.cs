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
    public TextMeshProUGUI costText             = null;     // 플레이어 수 텍스트

    AudioManager audioManager   = null;   // audioManager
    DappxAPIDataConroller dappxAPIDataConroller = null;
    private void OnEnable()
    {
        audioManager            = FindObjectOfType<AudioManager>();
        dappxAPIDataConroller   = FindObjectOfType<DappxAPIDataConroller>();
    }
    private void Update()
    {
        if (roomTitleTxt.text == "RoomTitle") roomTitleTxt.text = roomName;
        RoomInPlayerCntText.text = roomInPlayer + " / " + maxRoomInPlayer;
    }
    public void OnClickJoinRoom()
    {
        // Cost를 지불할 수 없다면 리턴.
        if (dappxAPIDataConroller.ZeraBalanceInfo.data.balance < dappxAPIDataConroller.BetSettings.data.bets[0].amount) return;

        audioManager.SoundPlay(audioManager.ClickSound);
        // 방에 들어간다면 코스트를 지불하고 들어가야 한다.
        PhotonNetwork.JoinRoom(roomName);
    }

}
