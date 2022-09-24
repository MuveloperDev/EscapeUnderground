using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// brick의 HP를 전체 합산한 함수
public delegate void HpManager();
public class BrickListManager : MonoBehaviourPunCallbacks
{
    
	public List<Brick> listBrick = new List<Brick>();
	[SerializeField] private float fullHP;
	[SerializeField] private float fullCurHP;
    [SerializeField] private UIManager uiManager;
    
    // 최대 체력과 현재 체력을 넘겨줄 프로퍼티
    public float FullHP { get { return fullHP; } }
    public float FullCurHP { get { return fullCurHP; } }

    public HpManager hpManager;


    private void Awake()
    {
        hpManager = MaxHP;
        //uiManager.SliderBarSetting();
        uiManager.SildbarSeting();
        
    }
    private void Start()
    {
        fullCurHP = fullHP;
    }
    private void Update()
    {
        if (listBrick.Count <= 0)
        {
            if (photonView.IsMine)
            {
                uiManager.ShowWinText();
                Invoke("LoadWin", 2f);
            }
            else
            { 
                uiManager.ShowLoseText();
                Invoke("LoadLose", 2f);
            } 
        }

    }

    void LoadLose()
    {
        WalletManager.Instance.LoseMoney();
        photonView.RPC("LoadStartScene", RpcTarget.All);
    }
    void LoadWin()
    {
        WalletManager.Instance.GetMoney();
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
        photonView.RPC("ReceiveDamage", RpcTarget.All, damage);
    }

    // 충돌시 Slider로 전달
    [PunRPC]
    public void ReceiveDamage(float Damage)
    {
        uiManager.CallUpdateHPSlider(Damage);

    }

    // 게임이 끝나면 모든 클라이언트들에게 LoadStartScene을 호출한다.
    // 플레이어 모두가 방을 나가게 되면 OnLeftRoom이 콜백이 되는데 
    // 그 시점에 StartScene을 로드레벨 시킨다.
    [PunRPC]
    void LoadStartScene()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        // 자기자신을 호출하여 방에 싱크된 사람들을 호출한다.
        PhotonNetwork.LoadLevel("StartScene");
    }
}
