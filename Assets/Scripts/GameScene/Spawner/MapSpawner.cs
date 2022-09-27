using Photon.Pun;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapSpawner : MonoBehaviourPunCallbacks
{
    //[SerializeField] private GameObject[] mapPrefab = new GameObject[3];
    [SerializeField] private GameObject playerMap = null;
    [SerializeField] private GameObject challMap = null;
    [SerializeField] int idx;

    BrickListManager BrickListManager;

    List<string> mapPrefabList = new List<string>() { "Prefabs/Map/Map1", "Prefabs/Map/Map2", "Prefabs/Map/Map3" };
    List<Vector3> offsetPos = new List<Vector3>() { new Vector3(-9.5f, -1.5f, 0) , new Vector3(-0.5f, -1.5f, 0) };

    MapSelectManager mapSelectManager = null;

    private void Awake()
    {
        mapSelectManager = FindObjectOfType<MapSelectManager>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            idx = mapSelectManager.GetMapSelect();
            photonView.RPC("SetIdx", RpcTarget.All, idx);
        }
        Invoke("CalllSettingMap", 0.2f);
    }

    void CalllSettingMap()
    {
        if (PhotonNetwork.IsConnected) SetingMap();
    }

    // idx에 저장된 랜덤한 맵을 지정한 position에 위치시켜 호출
    // 맵의 Brick HP를 저장시킬 BrickListManager 호출
    public void SetingMap()
    {
        // 위치 동기화
        if (PhotonNetwork.IsMasterClient)
            playerMap = PhotonNetwork.Instantiate(mapPrefabList[idx], transform.position + offsetPos[0], Quaternion.identity);
        else playerMap = PhotonNetwork.Instantiate(mapPrefabList[idx], transform.position + offsetPos[1], Quaternion.identity);
        BrickListManager = playerMap.GetComponent<BrickListManager>();
        BrickListManager.hpManager();
    }

    [PunRPC]
    void SetIdx(int idx) => this.idx = idx;

}
