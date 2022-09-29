using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("[ Panels ]")]
    [SerializeField] private LobbyManager       lobbyPanel                  = null;     // 로비 패널
    [SerializeField] private RoomManager        roomPanel                   = null;     // 룸 패널
    [SerializeField] private GridLayoutGroup    roomListPanel               = null;     // 룸리스트 패널
    [SerializeField] private CanvasGroup        walletPanel                 = null;     // 룸리스트 패널

    [Header("[ Buttons ]")]
    [SerializeField] private Button walletBtn = null;

    [Header("[ Components ]")]
    [SerializeField] UIChatManager          chatManager             = null;         // 채팅 매니저
    [SerializeField] AudioManager           audioManager            = null;         // 사운드 매니저
    [SerializeField] JsonDataController     jsonDataController      = null;         // Json Data
    [SerializeField] MyWallet               myWallet                = null;         // 월렛
    [SerializeField] DappxAPIDataConroller  dappxAPIDataConroller   = null;         // DappXApi                 


    [Header("[ Lists ]")]
    [SerializeField] List<RoomInfo>      uiRoomList              = new List<RoomInfo>();     // 룸리스트 UI 관리 리스트.

    private void Awake()
    {
        // 게임 시작시 스크린 사이즈를 맞춰줌 16 : 9 사이즈 마지막 인자값은 전체화면 유무
        Screen.SetResolution(1920, 1080, false);

        //AutomaticallySyncScene 은 방에 있는 모든 클라이언트들을 자동적으로 마스터 클라이언트와 동일한 레벨을 로드시킨다.
        PhotonNetwork.AutomaticallySyncScene = true;

        
        chatManager             = FindObjectOfType<UIChatManager>();            // UIChatManager
        audioManager            = FindObjectOfType<AudioManager>();             // audioManager
        myWallet                = FindObjectOfType<MyWallet>();                 // myWallet
        jsonDataController      = FindObjectOfType<JsonDataController>();       // Json 데이타
        dappxAPIDataConroller   = FindObjectOfType<DappxAPIDataConroller>();    // dappxAPIDataConroller

        walletBtn.onClick.AddListener(delegate { OnClick_OpenCloseWalletPanel(); });
    }
    void Start()
    {
        OnClickConnectToMasterServer(); // 마스터 서버 연결

    }

    // WalletPanel 알파값 조절
    void OnClick_OpenCloseWalletPanel()
    {
        walletPanel.alpha = walletPanel.alpha >= 1 ? 0 : 1;
    }

    // 마스터 서버 접속 요청.
    public void OnClickConnectToMasterServer() => PhotonNetwork.ConnectUsingSettings();

    // 마스터 서버 연결 끊기.
    public void OnClickDiconnectToMasterServer() => PhotonNetwork.Disconnect();

    // Photon Cloud Server에 접속에 성공시 불리는 콜백 함수.
    public override void OnConnectedToMaster() {
        // 닉네임 설정
        PhotonNetwork.LocalPlayer.NickName = dappxAPIDataConroller.GetUserProfile.userProfile.username;
        PhotonNetwork.JoinLobby();
        chatManager.ConnectedMyChat();  // 챗 연결
                                        // 월렛 텍스트 업데이트
        Invoke("UserInfoUpdate", 2f);
    }

    void UserInfoUpdate()
    {
        
        myWallet.MoneyUpdate();
    } 

    // 마스터 서버 연결이 끊겼을 때 호출되는 함수.
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log("OnDisConnected : " + cause);
  
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

    }
    // 플레이어가 방에 입장시 정보 업데이트
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 방 인원수 업데이트
        roomPanel.CntPlayersTxt = string.Format("[ {0} / {1} ]", PhotonNetwork.CurrentRoom.Players.Count, PhotonNetwork.CurrentRoom.MaxPlayers);

        // Challenger Info Panel 업데이트
        photonView.RPC("OpenChallengerPanelInRoom", RpcTarget.All);


        // 방을 닫는다.
        PhotonNetwork.CurrentRoom.IsOpen = false;
        // 2초뒤에 시작.
        Invoke("WaitForLoadLevel", 2f);
    }

    [PunRPC]
    void OpenChallengerPanelInRoom(Player newPlayer)
    {
        // Challenger Info Panel 업데이트
        roomPanel.ChallengerPanel.gameObject.SetActive(true);
        TextMeshProUGUI challengerText = roomPanel.ChallengerPanel.GetComponentInChildren<TextMeshProUGUI>();
        challengerText.text = newPlayer.NickName;
    }
    void WaitForLoadLevel() => PhotonNetwork.LoadLevel("GameScene");


    // 랜덤조인이 실패했을 경우
    public override void OnJoinRandomFailed(short returnCode, string message) => Debug.Log("OnJoinRandomFailed : " + message);

    // Room을 나가면 호출 되는 함수
    public override void OnLeftRoom()
    {
        if (!PhotonNetwork.InLobby) return;
        roomPanel.gameObject.SetActive(false);
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
        //roomObj.cost = (float)roomInfo.CustomProperties["Cost"];
    }

    #endregion

    // 타이틀 씬 상태 텍스트 출력.
    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
        gUIStyle.fontSize = 20;
        gUIStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(0f, 0f, 350f, 50f), "ServeState : " + PhotonNetwork.Server.ToString(), gUIStyle);
    }

    #region ContextMenu
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
    #endregion
}
