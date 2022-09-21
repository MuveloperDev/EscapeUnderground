using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    [Header("[ Panels ]")]
    [SerializeField] private Image createRoomPanel = null;
    [SerializeField] private Image findRoomPanel = null;

    [Header("[ InputFields ]")]
    [SerializeField] private TMP_InputField roomNameInputField = null;
    [SerializeField] private TMP_InputField findRoomNameInputField = null;

    [Header("[ ScrollView ]")]
    [SerializeField] private GridLayoutGroup roomListPanel;

    [Header("[ Texts ]")]
    [SerializeField] private TextMeshProUGUI cntPlayers;

    [Header("[ Buttons ]")]
    [SerializeField] private Button openCreateRoomBtn = null;     // 방 생성패널 열기 버튼
    [SerializeField] private Button CreateRoomBtn = null;     // 방 생성패널 열기 버튼
    [SerializeField] private Button joinOrCreateRoomBtn = null;     // 방 접속 혹은 생성 버튼
    [SerializeField] private Button leaveLobbyBtn = null;     // 로비 접속 해제 버튼
    [SerializeField] private Button findEnterRoomBtn = null;     // 방 찾기 버튼

    private void OnEnable()
    {
        Init();
        //버튼 연결.
        openCreateRoomBtn.onClick.AddListener(delegate { OnClickActiveCreateRoomPanel(); });
        CreateRoomBtn.onClick.AddListener(delegate { OnClickCreateRoom(); });
        joinOrCreateRoomBtn.onClick.AddListener(delegate { OnClickJoinOnCreateRoom(); });
        leaveLobbyBtn.onClick.AddListener(delegate { OnClickLeaveLobby(); });
        findEnterRoomBtn.onClick.AddListener(delegate { OnClickOpenFindRoomPanel(); });
        // InputField 연결
        roomNameInputField.onEndEdit.AddListener(delegate (string roomName) { SetRoomName(roomName); });
        findRoomNameInputField.onEndEdit.AddListener(delegate (string roomName) { SetFindRoomName(roomName); });
    }

    void Init()
    {
        findRoomPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 현재 들어온 플레이어 수 업데이트
        cntPlayers.text = "Players : " + PhotonNetwork.CountOfPlayers.ToString();

    }

    #region FindRoomMethod

    // FindRoomPanel 열기
    void OnClickOpenFindRoomPanel() => findRoomPanel.gameObject.SetActive(true);
    // FindRoomPanel 닫기
    void OnClickCloseFindRoomPanel() => findRoomPanel.gameObject.SetActive(false);

    // 입력한 룸으로 접속.
    void OnClickFindEnterRoom()
    {
        if (findRoomNameInputField.text.Length == 0) return;
        PhotonNetwork.JoinRoom(findRoomNameInputField.text);
    }
    // 방 이름 입력 받는 함수.
    void SetFindRoomName(string roomName) => findRoomNameInputField.text = roomName;
    #endregion

    // 방이 있으면 Join 없으면 방 생성
    public void OnClickJoinOnCreateRoom()
    {
        // 룸이 존재한다면 랜덤 접속
        if (PhotonNetwork.CountOfRooms > 0)
        { 
            PhotonNetwork.JoinRandomRoom(); 
            return; 
        }
        OnClickActiveCreateRoomPanel();
    }

    // 로비 접속 해제.
    public void OnClickLeaveLobby()
    {
        if (!PhotonNetwork.InLobby) return;
        PhotonNetwork.LeaveLobby();
    }

    // 방생성 패널 활성화.
    public void OnClickActiveCreateRoomPanel() => createRoomPanel.gameObject.SetActive(true);
    // 방 생성
    void OnClickCreateRoom() => PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions { MaxPlayers = 2 });

    public void SetRoomName(string name) => roomNameInputField.text = name;

}
