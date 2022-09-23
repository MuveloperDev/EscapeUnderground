using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("[ Texts ]")]
    [SerializeField] private TextMeshProUGUI    ServerStateTxt              = null;     // 서버 상태 텍스트
    [SerializeField] private TextMeshProUGUI    clentStateTxt               = null;     // 클라이언트 상태 텍스트
    [SerializeField] private TextMeshProUGUI    clentNickNameTxt            = null;     // 클라이언트 닉네임 텍스트
    [SerializeField] private TextMeshProUGUI    countOfRoomsTxt             = null;     // 룸 개수 텍스트
    [SerializeField] private TextMeshProUGUI    countOfPlayersOnMasterTxt   = null;     // 마스터 서버 상에 있는 플레이어 텍스트
    [SerializeField] private TextMeshProUGUI    countOfPlayersInRoomsTxt    = null;     // 게임 서버 상에 있는 플레이어 텍스트
    [SerializeField] private TextMeshProUGUI    countOfPlayersTxt           = null;     // 플레이어 수 텍스트
    [SerializeField] private TextMeshProUGUI    lobbyStateTxt               = null;     // 로비 상태 텍스트
    [SerializeField] private TextMeshProUGUI    roomStateTxt                = null;     // 방 상태 텍스트


    [Header("[ InputFields ]")]
    [SerializeField] private TMP_InputField     nickNameInput               = null;     // 닉네임 인풋 필드

    [Header("[ Panels ]")]
    [SerializeField] private LobbyManager       lobbyPanel                  = null;     // 로비 패널
    [SerializeField] private RoomManager        roomPanel                   = null;     // 룸 패널
    [SerializeField] private GridLayoutGroup    roomListPanel               = null;     // 룸리스트 패널

    [Header("[ Buttons ]")]
    [SerializeField] private Button connectServerBtn        = null;     // 서버 연결 버튼
    [SerializeField] private Button disConnectServerBtn     = null;     // 서버 연결 해제 버튼
    [SerializeField] private Button joinLobbyBtn            = null;     // 로비 접속 버튼

    // 채팅 매니저
    UIChatManager chatManager = null;

    // 룸리스트 UI 관리 리스트.
    List<RoomInfo> uiRoomList = new List<RoomInfo>();

    // 월렛
    MyWallet myWallet = null;
    private void Awake()
    {
        // 게임 시작시 스크린 사이즈를 맞춰줌 16 : 9 사이즈 마지막 인자값은 전체화면 유무
        Screen.SetResolution(800, 600, false);

        //AutomaticallySyncScene 은 방에 있는 모든 클라이언트들을 자동적으로 마스터 클라이언트와 동일한 레벨을 로드시킨다.
        PhotonNetwork.AutomaticallySyncScene = true;

        // UIChatManager
        chatManager = FindObjectOfType<UIChatManager>();

        
        myWallet = FindObjectOfType<MyWallet>();
    }
    void Start()
    {
        
        OnClickConnectToMasterServer();
        ServerStateTxt.text = "ServerState : DisConnected";
        clentNickNameTxt.text = "Client NickName : None";

        connectServerBtn.onClick.AddListener(delegate { OnClickConnectToMasterServer(); });
        disConnectServerBtn.onClick.AddListener(delegate { OnClickDiconnectToMasterServer(); });
        joinLobbyBtn.onClick.AddListener(delegate { OnClickJoinLobby(); });

        nickNameInput.onEndEdit.AddListener(delegate (string name) {
            // 연결이 되어있지 않으면 리턴.
            if (!PhotonNetwork.IsConnected) return;
            // 인풋필드를 막는다.
            nickNameInput.enabled = false;
            SetNickName(name); 
        });


    }

    void Update()
    {

        // 서버 상태 업데이트
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
        // 방 상태 업데이트
        roomStateTxt.text = "Room_State : " + PhotonNetwork.InRoom;
        // 로비 상태 업데이트
        lobbyStateTxt.text = "Lobby_State : " + PhotonNetwork.InLobby;
    }

    // 마스터 서버 접속 요청.
    public void OnClickConnectToMasterServer() => PhotonNetwork.ConnectUsingSettings();

    // 마스터 서버 연결 끊기.
    public void OnClickDiconnectToMasterServer() => PhotonNetwork.Disconnect();

    // 로비에 접속하기.
    public void OnClickJoinLobby()
    {
        if (nickNameInput.text.Length == 0) return;
        PhotonNetwork.JoinLobby();
        
        // 챗 연결
        chatManager.ConnectedMyChat();
    }


    // Photon Cloud Server에 접속에 성공시 불리는 콜백 함수.
    public override void OnConnectedToMaster() { 
        ServerStateTxt.text = "ServerState : Sucess Connected Master Server";
        // 월렛 텍스트 업데이트
        myWallet.moneyUpdate();
    }

    // 마스터 서버 연결이 끊겼을 때 호출되는 함수.
    public override void OnDisconnected(DisconnectCause cause) =>
        ServerStateTxt.text = "ServerState : DisConnected Master Server  ->" + cause;


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
    #region Lobby
    // 로비에 접속시 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        chatManager.ClearText();
        lobbyPanel.gameObject.SetActive(true);
    } 

    // 로비를 나가면 호출 되는 콜백 함수
    public override void OnLeftLobby() => lobbyPanel.gameObject.SetActive(false);
    #endregion

    #region Room
    // 방에 접속시 호출되는 콜백 함수.
    public override void OnJoinedRoom()
    {
        lobbyPanel.gameObject.SetActive(false);
        roomPanel.gameObject.SetActive(true);
        WalletManager.Instance.BetMoney((float)PhotonNetwork.CurrentRoom.CustomProperties["Cost"]);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Fuck");
        return;
    }

    // Room을 나가면 호출 되는 함수
    public override void OnLeftRoom()
    {
        roomPanel.gameObject.SetActive(false);
        if (!PhotonNetwork.InLobby)
            Debug.Log("Not In Lobby");
        
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
    #endregion

    #region RoomListUI
    // 계속 업데이트를 받아야 하기 때문에 계속 활성화 되어있는 매니저에서 체크한다.
    // 서버상의 룸 리스트를 받아오는 콜백 함수이다.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 업데이트 시기 : 로비로 접속시기, 다른 클라이언트가 방을 들어갔을 시, 방이 하나 사라졋을시.
        // Room Tag를 가진 오브젝트를 찾아서 회수한다. 
        // 회수를 안하면 방을 나갈시 계속해서 남아있게 된다.

        // UIRoom이 업데이트 되는 시기마다 room프리펩 전체 회수
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Room"))
            RoomsPool.Instance.Release(obj);


        // 룸 업데이트 콜백 함수 정보를 기준으로 UIroomList 업데이트 
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log("RoomUI Create");
            // 서버상 룸의 제거예정 상태인지를 확인하여 UIroomList에서 삭제
            // 아니라면 룸에 포함되어있는지 확인하여 생성.
            if (roomInfo.RemovedFromList) uiRoomList.Remove(roomInfo);
            else if (!uiRoomList.Contains(roomInfo)) uiRoomList.Add(roomInfo);
        }

        // UIroomList 수 만큼 프리펩을 새로 생성한다.
        foreach (RoomInfo roomInfo in uiRoomList) CreateRoomUI(roomInfo);

    }

    // UI 프리펩 추가
    void CreateRoomUI(RoomInfo roomInfo)
    {
        TitleRoomInList roomObj = RoomsPool.Instance.Get(transform.position).GetComponent<TitleRoomInList>();
        roomObj.transform.SetParent(roomListPanel.transform);
        roomObj.roomName = roomInfo.Name;
        roomObj.roomInPlayer = roomInfo.PlayerCount.ToString();
        roomObj.maxRoomInPlayer = roomInfo.MaxPlayers.ToString();

        // 커스텀 프로퍼티로 방의 값을 추가한다.
        //roomInfo.CustomProperties.Add("cost", WalletManager.Instance.SetCost());
        roomObj.cost = (float)roomInfo.CustomProperties["Cost"];
    }

    #endregion


    // 인스펙터상의 옵션 추가.
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
