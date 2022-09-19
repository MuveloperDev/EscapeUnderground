using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("[ Texts ]")]
    [SerializeField] private TextMeshProUGUI ServerStateTxt = null;
    [SerializeField] private TextMeshProUGUI clentStateTxt = null;
    [SerializeField] private TextMeshProUGUI clentNickNameTxt = null;
    [SerializeField] private TextMeshProUGUI countOfRoomsTxt = null;
    [SerializeField] private TextMeshProUGUI countOfPlayersOnMasterTxt = null;
    [SerializeField] private TextMeshProUGUI countOfPlayersInRoomsTxt = null;
    [SerializeField] private TextMeshProUGUI countOfPlayersTxt = null;
    [SerializeField] private TextMeshProUGUI lobbyStateTxt = null;
    [SerializeField] private TextMeshProUGUI roomStateTxt = null;


    [Header("[ InputFields ]")]
    [SerializeField] private TMP_InputField roomInput = null;
    [SerializeField] private TMP_InputField nickNameInput = null;

    [Header("[ Panels ]")]
    [SerializeField] private LobbyManager lobbyPanel = null;
    [SerializeField] private RoomManager roomPanel = null;
    [SerializeField] private GridLayoutGroup roomListPanel;

    void Start()
    {


        ServerStateTxt.text = "ServerState : DisConnected";
        clentNickNameTxt.text = "Client NickName : None";

    }

    void Update()
    {


        ServerStateTxt.text = "ServerState : " + PhotonNetwork.Server;

        // 클라이언트의 현재 상태를 가져온다.
        clentStateTxt.text = "Clent_State : " + PhotonNetwork.NetworkClientState.ToString();
        // 라이브 룸의 개수를 가져온다.
        countOfRoomsTxt.text = "CountOfRooms : " + PhotonNetwork.CountOfRooms.ToString();
        // 룸에 참여하지 않은 플레이어의 수를 가져온다.
        countOfPlayersOnMasterTxt.text = "CountOfPlayersOnMaster : " + PhotonNetwork.CountOfPlayersOnMaster.ToString();
        // 룸 안에 있는 플레이어의 수를 가져온다.
        countOfPlayersInRoomsTxt.text = "CountOfPlayersInRooms : " + PhotonNetwork.CountOfPlayersInRooms.ToString();
        // 연결되어 있는 플레이어의 총 수를 가져온다.
        countOfPlayersTxt.text = "CountOfPlayers : " + PhotonNetwork.CountOfPlayers.ToString();


        //lobbyStateTxt.text = PhotonNetwork.InLobby ? "Lobby_State : In Lobby" : "Lobby_State : Not Lobby";
        //roomStateTxt.text = PhotonNetwork.InRoom ? "Room_State : In Room" : "Room_State : Not Room";

    }




    #region Photon_Method

    // 마스터 서버 접속 요청.
    public void OnClickConnectToMasterServer() => PhotonNetwork.ConnectUsingSettings();

    // 마스터 서버 연결 끊기.
    public void OnClickDiconnectToMasterServer() => PhotonNetwork.Disconnect();

    // 로비에 접속하기.
    public void OnClickJoinLobby()
    {
        if (nickNameInput.text.Length == 0) return;
        PhotonNetwork.JoinLobby();
    }

    // 방이 있으면 Join 없으면 방 생성
    public void OnClickJoinOnCreateRoom()
    {
        // 룸 이름을 정하지 않았다면
        if (roomInput.text.Length == 0)
        {
            Debug.Log("###### Input Room Name ");
            return;
        }
        PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null); // 로비에 접속하기.

    }

    // 방을 나간다.
    public void OnClickLeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("###### Not In Room.");
            return;
        }
        PhotonNetwork.LeaveRoom();
    }

    // 로비를 나간다.
    public void OnClickLeaveLobby()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("###### Not In Lobby.");
            return;
        }

        PhotonNetwork.LeaveLobby();
    }

    // 방을 참가하려면, Connect 되어있거나 Lobby에 참가해있어야 한다.

    public void OnClickCreateRoom()
    {
        // 방 생성하고, 참가.
        // 방 이름, 최대 플레이어 수, 비공개 등을 지정 가능.
        // 왜 CreateRoom은 임의의 방이름이 주어지고 JoinOnCreate는 주어지지 않는지?
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    }



    public void SetNickName(string nickName)
    {
        if (!PhotonNetwork.IsConnected)
        {
            clentNickNameTxt.text = "Client_NickName : Disconnected";
            return;
        }
        // 클라이언트의 닉네임을 설정한다.
        // LoadBalancingClient 클래스 상에 있는 유저 정보중 nickname을 재정의한다.
        PhotonNetwork.LocalPlayer.NickName = nickName;
        clentNickNameTxt.text = "Client_NickName : " + PhotonNetwork.LocalPlayer.NickName;
    }
    #endregion




    #region Photon_CallBack_Functions

    /// <summary>
    /// Photon Cloud Server에 접속에 성공시 불리는 콜백 함수.
    /// PhotonNetwork.ConnectUsingSettings()가 성공하면 불린다.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        ServerStateTxt.text = "ServerState : Sucess Connected Master Server";

    }
    // 마스터 서버 연결이 끊겼을 때 호출되는 함수.
    public override void OnDisconnected(DisconnectCause cause)
    {
        
        ServerStateTxt.text = "ServerState : DisConnected Master Server " + cause;
    }

    // 로비에 접속시 호출되는 함수
    public override void OnJoinedLobby()
    {
        lobbyStateTxt.text = "Lobby_State : Joined Lobby";
        lobbyPanel.gameObject.SetActive(true);

        //Debug.Log("#########RoomCnt : " + GameObject.FindGameObjectsWithTag("Room").Length);
        //foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Room"))
        //{
        //    RoomsPool.Instance.Release(obj);
        //}
    }

    // 방에 접속시 호출되는 함수
    public override void OnJoinedRoom()
    {
        lobbyPanel.gameObject.SetActive(false);
        roomPanel.gameObject.SetActive(true);
        roomStateTxt.text = "Room_State : Joined Room";
    }



    // 로비를 나가면 호출 되는 함수
    public override void OnLeftLobby()
    {
        lobbyPanel.gameObject.SetActive(false);
        lobbyStateTxt.text = "Lobby_State : Left Lobby";
    }

    // 로비를 나가면 호출 되는 함수
    public override void OnLeftRoom()
    {
        roomPanel.gameObject.SetActive(false);
        //lobbyPanel.gameObject.SetActive(true);
       
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Not In Lobby");
        }
        StartCoroutine(ConnetcedLobby());
        roomStateTxt.text = "Room_State : Left Room";


    }

    // 룸에서 나온후 마스터서버에 접속되자마자 로비로 접속 요청을 위한 코루틴
    IEnumerator ConnetcedLobby()
    {
        while (true)
        {
            yield return null;
            if (PhotonNetwork.NetworkClientState.ToString() == "ConnectedToMasterServer")
            {
                PhotonNetwork.JoinLobby();
                yield break;
            }
        }

    }

    // 계속 업데이트를 받아야 하기 때문에 계속 활성화 되어있는 매니저에서 체크한다.
    // 서버상의 룸 리스트를 받아오는 콜백 함수이다.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Room Tag를 가진 오브젝트를 찾아서 회수한다. 
        // 회수를 안하면 방을 나갈시 계속해서 남아있게 된다.
        Debug.Log("RoomListCnt : " + roomList.Count);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Room"))
        {
            RoomsPool.Instance.Release(obj);
        }
        // 룸 프리펩 생성 
        foreach (RoomInfo roomInfo in roomList)
        {
            //RoomInfo room = roomList[i];
            TitleRoomInList roomObj = RoomsPool.Instance.Get(transform.position).GetComponent<TitleRoomInList>();
            Debug.Log("Clone room : " + roomObj.name);
            roomObj.transform.SetParent(roomListPanel.transform);
            roomObj.roomName = roomInfo.Name;
            roomObj.roomInPlayer = roomInfo.PlayerCount.ToString();
            roomObj.maxRoomInPlayer = roomInfo.MaxPlayers.ToString();
        }
    }





    // 룸안에서 로비의 룸리스트 정보를 얻기 위한 스켈레톤 코드
    public void PullRoomList()
    {
        BringRoom roomPoller = gameObject.AddComponent<BringRoom>();
        roomPoller.OnGetRoomsInfo
        (
            (roomInfos) =>
            {
                // 룸리스트를 받고나서 작업 코드 넣기
                Debug.Log($"현재 방 갯수 : {roomInfos.Count} \n 현재 방 이름 : {roomInfos[0].Name}");

                // 마지막엔 오브젝트 제거해주기
                Destroy(roomPoller);
            }
        );
    }


    #endregion


    [ContextMenu("[Info]")]
    public void RoomInfo()
    {
        Debug.Log("ClickInfo");
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("현재 방 이름 : " + PhotonNetwork.CurrentRoom);
            Debug.Log("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("현재 방 최대 인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);
            Debug.Log("로비에 있는 가? : " + PhotonNetwork.InLobby);
            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
                playerStr += PhotonNetwork.PlayerList[i].NickName + ",";
            Debug.Log(playerStr);

            
        }
        else
        {
            Debug.Log("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            // 룸에 참여하지 않은 플레이어의 수를 가져온다.
            Debug.Log("방 개수 : " + PhotonNetwork.CountOfRooms);
            // 룸 안에 있는 플레이어의 수를 가져온다.
            Debug.Log("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            // 연결되어 있는 플레이어의 총 수를 가져온다.
            Debug.Log("로비에 있는 가? : " + PhotonNetwork.InLobby);
            Debug.Log("연결 되었는 지 ? : " + PhotonNetwork.IsConnected);
        }
    }
}
