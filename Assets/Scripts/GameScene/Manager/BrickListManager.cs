using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using TMPro;
using System.Security.Cryptography;
using Photon.Realtime;

// brick의 HP를 전체 합산한 함수
public delegate void HpManager();
public class BrickListManager : MonoBehaviourPunCallbacks,IInRoomCallbacks
{

    public List<Brick> listBrick = new List<Brick>();
    [SerializeField] private float fullHP;
    [SerializeField] private float fullCurHP;
    [SerializeField] private UIManager uiManager;
    [SerializeField] bool changedMasterClinet = false;

    // 최대 체력과 현재 체력을 넘겨줄 프로퍼티
    public float FullHP { get { return fullHP; } }
    public float FullCurHP { get { return fullCurHP; } }

    public HpManager hpManager;

    [SerializeField] AudioManager audioManager = null;
    [SerializeField] LoadSceneStart loadSceneStart = null;



    // DappXAPI---------------------------------------------------------
    // 댑엑스 api 값 접근
    [SerializeField] DappxAPIDataConroller dappxAPIDataConroller;
    [SerializeField] BrickListManager[] brickListManager = null;
    [SerializeField] string sessionId = null;
    [SerializeField] string userProfileId = null;
    [SerializeField] string[] userProfileIds = new string[2];
    public string SessionID { get { return sessionId; } }
    public string UserProfileID { get { return userProfileId; } }

    

    private void Awake()
    {
        dappxAPIDataConroller = FindObjectOfType<DappxAPIDataConroller>();
        audioManager = FindObjectOfType<AudioManager>();
        hpManager = MaxHP;
    }
    private void OnEnable()
    {
        loadSceneStart = FindObjectOfType<LoadSceneStart>();
        uiManager.SildbarSeting();
    }
    private void Start()
    {
        fullCurHP = fullHP;

        // 나의 객체면 sseionID 할당. 아니라면 리턴
        if (photonView.IsMine)
        { 
            sessionId = dappxAPIDataConroller.GetSessionID.sessionId;
            userProfileId = dappxAPIDataConroller.GetUserProfile.userProfile._id;
        } 
        else return;

        // 상대 로컬의 나의 객체의 SessionID를 갱신한다.
        photonView.RPC("SetSessionID", RpcTarget.Others, dappxAPIDataConroller.GetSessionID.sessionId);
        // 상대 로컬의 나의 객체의 userProfile_id를 갱신한다.
        photonView.RPC("SetUserProfileID", RpcTarget.Others, userProfileId);
        
        // 마스터 클라이언트만 배팅 관련 서버처리.
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("IsMasterClient : " + PhotonNetwork.IsMasterClient);

            // 스타트베팅은 2초뒤에 실행. 플레이어 생성 및 서버동기화를 기다려준다.
            Invoke("StartBetting", 1f);
        }

    }

    // ----------------------------------배팅관련-----------------------------------------------------
    [PunRPC]
    void SetSessionID(string sessionId)
    {
        this.sessionId = sessionId;
        Debug.Log("SessionId In SetSessionID : " + this.sessionId);
    }
    [PunRPC]
    void SetUserProfileID(string userProfile_id)
    {
        this.userProfileId = userProfile_id;
        Debug.Log("SessionId In SetSessionID : " + this.userProfileId);
    }


    void StartBetting()
    {
        Debug.Log("################## StartBettings");
        // 2초 뒤에 SetSessionIDArr 를 실행한다.
        Invoke("SetSessionIDArr", 1f);
    }

    void SetSessionIDArr()
    {
        Debug.Log("################## SetSesstionIDArr");

        // 플레이어의 SessionID를 받아오기 위해 배열로 찾아준다.
        brickListManager = FindObjectsOfType<BrickListManager>();

        Debug.Log("BrickListManager[] : " + dappxAPIDataConroller.GetSessionID.sessionId + " / " + brickListManager[1].sessionId);

        // 플레이어들의 SessionId를 담을 배열 객체를 생성한다.
        // sessionID[0]은 나의 객체 세션 아이디를 할당한다.
        // sessionID[1]은 상대 객체 세션 아이디를 할당한다.
        // UserProfileID[0]은 나의 객체 userId를 할당한다.
        // UserProfileID[1]은 상대 객체 userId를 할당한다.
        string[] sessionId = new string[2];
        string[] userIds = new string[2];
        sessionId[0] = brickListManager[0].SessionID;
        userIds[0] = brickListManager[0].UserProfileID;
        sessionId[1] = brickListManager[1].SessionID;
        userIds[1] = brickListManager[1].UserProfileID;

        photonView.RPC("SetUserIds", RpcTarget.All, userIds);

        Debug.Log("######## # ######## sessionId[0] : " + sessionId[0]);
        Debug.Log("######## # ######## sessionId[1] : " + sessionId[1]);
        Debug.Log("######## # ######## userIds[0] : " + userIds[0]);
        Debug.Log("######## # ######## userIds[1] : " + userIds[1]);

        // 배열을 인자로 넘겨준다.
        // 배팅 시작.
        userProfileIds = userIds;
        Debug.Log("######## # ######## userProfileIds[0] : " + userProfileIds[0]);
        Debug.Log("######## # ######## userProfileIds[1] : " + userProfileIds[1]);
        dappxAPIDataConroller.BettingCoinToZera(sessionId);
    }
    public void CallSetBettingId(string betting_id)
    {
        // 다른 로컬의 자신들에게 betting Id를 할당하게 한다.
        photonView.RPC("SetBettingID", RpcTarget.All, betting_id);
        Debug.Log("######## bettingID : " + betting_id);

    }

    [PunRPC]
    void SetBettingID(string betting_id)
    {
        dappxAPIDataConroller.betting_id = betting_id;
        Debug.Log("######## bettingID : " + dappxAPIDataConroller.betting_id);

    }
    // ---------------------------------------------------------------------------------------------------------

    [PunRPC]
    void SetUserIds(string[] userIds)
    {
        dappxAPIDataConroller.userProfileID = userIds;
    }

    private void Update()
    {
        // 방에서 플레이어가 중도에 나갔을 시
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2 && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartEndGame(0, audioManager.WinSound));
            return;
        }
 
        if (listBrick.Count <= 0)
        {
            if (photonView.IsMine)
                EndGame(0, audioManager.WinSound);
            if (!photonView.IsMine)
                EndGame(1, audioManager.LoseSound);
        }
    }


    IEnumerator StartEndGame(int trigger, AudioClip clip)
    {
        yield return new WaitForSeconds(2f);
        EndGame(0, audioManager.WinSound);
    }
    void EndGame(int trigger, AudioClip clip)
    {
        if (uiManager.WinTextProperty != null || uiManager.LoseTextProperty != null)
            if (uiManager.WinTextProperty.IsActive() || uiManager.LoseTextProperty.IsActive()) return;

        audioManager.BGMSound(clip);
        
        if (trigger == 0)
        {
            // 게임서버에 플레어가 2명 미만이고 마스터클라이언트가 변경되었다면
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2 && changedMasterClinet)
            {
                // 원래의 마스터클라이언트가 아닌 클라이언트의 인덱스를 호출하여 준다.
                // 아래 함수들이 실행되지 않게 return 해준다.
                dappxAPIDataConroller.BettingZara_DeclareWinner(0);
                uiManager.ShowWinText();
                Invoke("LoadWin", 3f);
                return;
            }
            // 승자가 나일 때 배팅금 회수
            Debug.Log("UserPrfileIDs : " + dappxAPIDataConroller.userProfileID[1]);

            uiManager.ShowWinText();
            Invoke("LoadWin", 3f);

            if (!PhotonNetwork.IsMasterClient) return;
            dappxAPIDataConroller.BettingZara_DeclareWinner(1);
        }
        if (trigger == 1)
        {
            // 승자가 상대일 때 배팅금 회수
            Debug.Log("UserPrfileIDs : " + dappxAPIDataConroller.userProfileID[0]);
            uiManager.ShowLoseText();
            Invoke("LoadLose", 3f);

            if (!PhotonNetwork.IsMasterClient) return;
            dappxAPIDataConroller.BettingZara_DeclareWinner(0);
        }
    }

    void LoadLose()
    {
        photonView.RPC("LoadStartScene", RpcTarget.All);
    }
    void LoadWin()
    {
        photonView.RPC("LoadStartScene", RpcTarget.All);
    }

    // 생성된 Brick을 List에 담아준다
    public void AddBrick(Brick brick)
    {
        listBrick.Add(brick);
    }
    // List에 담겨진 Brick의 HP 총합계를 구한다
    public void MaxHP()
    {
        foreach (Brick brick in listBrick)
        {
            fullHP += brick.MaxHP;
        }
    }
    public void CallReceveDamage(float damage)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ReceiveDamage", RpcTarget.All, damage);
        }
    }

    // 충돌시 Slider로 전달
    [PunRPC]
    public void ReceiveDamage(float Damage) => uiManager.CallUpdateHPSlider(Damage);

    // 게임이 끝나면 모든 클라이언트들에게 LoadStartScene을 호출한다.
    // 플레이어 모두가 방을 나가게 되면 OnLeftRoom이 콜백이 되는데 
    // 그 시점에 StartScene을 로드레벨 시킨다.
    [PunRPC]
    void LoadStartScene()
    {
        if (!PhotonNetwork.InRoom) return;
        PhotonNetwork.LeaveRoom();
    }

    void OnMasterClientSwitched(Player newMasterClient)
    {
        changedMasterClinet = true;
    }
}
