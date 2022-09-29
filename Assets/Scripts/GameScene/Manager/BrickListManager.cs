using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using TMPro;

// brick의 HP를 전체 합산한 함수
public delegate void HpManager();
public class BrickListManager : MonoBehaviourPunCallbacks
{

    public List<Brick> listBrick = new List<Brick>();
    [SerializeField] private float fullHP;
    [SerializeField] private float fullCurHP;
    [SerializeField] private UIManager uiManager;

    [SerializeField] string sessionId = null;

    // 최대 체력과 현재 체력을 넘겨줄 프로퍼티
    public float FullHP { get { return fullHP; } }
    public float FullCurHP { get { return fullCurHP; } }

    public HpManager hpManager;

    [SerializeField] AudioManager audioManager = null;
    [SerializeField] LoadSceneStart loadSceneStart = null;
    // 댑엑스 api 값 접근
    [SerializeField] DappxAPIDataConroller dappxAPIDataConroller;


    public string SessionID { get { return sessionId; } }

    

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

        Invoke("StartBetting", 2f);
    }

    // ----------------------------------배팅관련-----------------------------------------------------
    [PunRPC]
    void SetSessionID(string sessionId)
    {
        this.sessionId = sessionId;
    }

    void StartBetting()
    {
        Debug.Log("################## StartBettings");
        // 객체별 세션아이디 설정
        if (photonView.IsMine)
            photonView.RPC("SetSessionID", RpcTarget.Others, dappxAPIDataConroller.GetSessionID.sessionId);

        // 마스터 클라이언트만 실행.
        if (PhotonNetwork.IsMasterClient) Invoke("SetSessionIDArr", 1f);
    }

    void SetSessionIDArr()
    {
        Debug.Log("################## SetSesstionIDArr");
        BrickListManager[] brickListManager = FindObjectsOfType<BrickListManager>();
        string[] sessionId = new string[2];

        sessionId[0] = brickListManager[0].SessionID;
        sessionId[1] = brickListManager[1].SessionID;

        //dappxAPIDataConroller.SessionIdArr = sessionId;
        // 배팅 시작.
        dappxAPIDataConroller.BettingCoinToZera(sessionId);
    }
    // ---------------------------------------------------------------------------------------------------------


    private void Update()
    {
        // 방에서 플레이어가 중도에 나갔을 시
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            EndGame(0, audioManager.WinSound);


        if (listBrick.Count <= 0)
        {
            if (photonView.IsMine)
                EndGame(0, audioManager.WinSound);
            if (!photonView.IsMine)
                EndGame(1, audioManager.LoseSound);
        }
    }

    void EndGame(int trigger, AudioClip clip)
    {
        if (uiManager.WinTextProperty != null || uiManager.LoseTextProperty != null)
            if (uiManager.WinTextProperty.IsActive() || uiManager.LoseTextProperty.IsActive()) return;

        audioManager.BGMSound(clip);
        if (trigger == 0)
        {
            // 승자 돈 회수
            dappxAPIDataConroller.BettingZara_DeclareWinner();

            uiManager.ShowWinText();
            Invoke("LoadWin", 3f);
        }
        if (trigger == 1)
        {
            uiManager.ShowLoseText();
            Invoke("LoadLose", 3f);
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
}
