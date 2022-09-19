using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringRoom : MonoBehaviourPunCallbacks
{
    Action<List<RoomInfo>> callback = null;
    LoadBalancingClient client = null;

    private void Update()
    {
        if (client != null)
        {
            // 반복문에서 service 함수를 호출해줘야 서브클라이언트에 연결이 유지됨
            client.Service();
        }
    }

    public void OnGetRoomsInfo(Action<List<RoomInfo>> callback)
    {
        // 서브클라이언트를 마스터 서버에 접속시킨다.
        this.callback = callback;
        client = new LoadBalancingClient();
        client.AddCallbackTarget(this);
        client.StateChanged += OnStateChanged;
        client.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
        client.AppVersion = PhotonNetwork.NetworkingClient.AppVersion;
        client.EnableLobbyStatistics = true;

        // 포톤 세팅에서 접속 지역을 설정해주어야 한다. (FixedRegion 세팅하기)
        client.ConnectToRegionMaster(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
    }

    void OnStateChanged(ClientState previousState, ClientState state)
    {
        Debug.Log("서브 클라이언트 상태 : " + state);

        // 서브클라이언트가 마스터 서버에 접속하면 로비로 접속 시킨다.
        if (state == ClientState.ConnectedToMasterServer)
        {
            client.OpJoinLobby(null);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("서브 클라이언트 룸리스트 업데이트");

        if (callback != null)
        {
            callback(roomList);
        }
        // 모든 작업이 끝나면 서브 클라이언트 연결 해제
        client.Disconnect();
    }
}
