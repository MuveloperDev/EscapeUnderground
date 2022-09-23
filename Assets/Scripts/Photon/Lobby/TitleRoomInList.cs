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
    public float cost   { get; set; }                       // 방의 cost

    public TextMeshProUGUI roomTitleTxt         = null;     // 룸 타이틀 텍스트
    public TextMeshProUGUI RoomInPlayerCntText  = null;     // 플레이어 수 텍스트
    public TextMeshProUGUI costText             = null;     // 플레이어 수 텍스트

    MyWallet myWallet = null;   // myWallet
    private void OnEnable()
    {
        myWallet = FindObjectOfType<MyWallet>();
    }
    private void Update()
    {
        costText.text = "Cost : " + cost.ToString();
        roomTitleTxt.text = roomName;
        RoomInPlayerCntText.text = roomInPlayer + " / " + maxRoomInPlayer;
    }
    public void OnClickJoinRoom()
    {
        // Cost를 지불할 수 없다면 리턴.
        if (myWallet.MyMoney < cost) return;

        // 방에 들어간다면 코스트를 지불하고 들어가야 한다.
        //WalletManager.Instance.BetMoney(cost);
        PhotonNetwork.JoinRoom(roomName);
    }

}
