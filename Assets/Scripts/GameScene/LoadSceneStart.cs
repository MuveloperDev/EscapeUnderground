using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;


public class LoadSceneStart : MonoBehaviourPunCallbacks
{
    [SerializeField] Button loadSceneBtn = null;
    [SerializeField] Button winBtn = null;
    [SerializeField] Button loseBtn = null;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        loadSceneBtn.onClick.AddListener(delegate 
        {
            LoadStartScene();
        });
        winBtn.onClick.AddListener(delegate
        {
            WalletManager.Instance.GetMoney();
            photonView.RPC("LoadStartScene", RpcTarget.All);
        });
        
        loseBtn.onClick.AddListener( delegate
        {

            WalletManager.Instance.LoseMoney();
            photonView.RPC("LoadStartScene", RpcTarget.All);

        });
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
