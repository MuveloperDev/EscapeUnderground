using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("[ Panels ]")]
    [SerializeField] private Image createRoomPanel = null;

    [Header("[ InputFields ]")]
    [SerializeField] private TMP_InputField roomNameInputField = null;

    [Header("[ ScrollView ]")]
    [SerializeField] private GridLayoutGroup roomListPanel;

    [Header("[ Texts ]")]
    [SerializeField] private TextMeshProUGUI cntPlayers;

    private void OnEnable()
    {
        createRoomPanel.gameObject.SetActive(false);

    }

    private void Update()
    {
        // 현재 들어온 플레이어 수 업데이트
        cntPlayers.text = "Players : " + PhotonNetwork.CountOfPlayers.ToString();

        // 클라우드 서버의 룸 개수와 룸리스트패널 자식객체의 개수를 비교하여 다르다면 업데이트.
        if (roomListPanel.transform.childCount != PhotonNetwork.CountOfRooms)
        {
            RoomListUpdate();
        }
    }

    // 로비 상에 있을 때 RoomList의 정보를 받아올 수 있다.
    // 룸에서RoomList의 정보를 확인하고 싶을 때는 LoadBanancingClient를 사용하여 룸안에서 리스트를 가져올 수 있는데
    // LoadBanancingClient는 서브클라이언트라고 생각하면 이해하기 쉽다.

    // 로비매니저에서는 받아오지를 못한다.
    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    Debug.Log("RoomListCnt : "+roomList.Count);
    //    for (int i = 0; i < roomList.Count; i++)
    //    {
    //        RoomInfo room = roomList[i];
    //        TitleRoomInList roomObj = RoomsPool.Instance.Get(transform.position).GetComponent<TitleRoomInList>();
    //        Debug.Log("Clone room : " + roomObj.name);
    //        roomObj.transform.SetParent(roomListPanel.transform);
    //        roomObj.roomName = room.Name;
    //        roomObj.roomInPlayer = room.PlayerCount.ToString();
    //        roomObj.maxRoomInPlayer = room.MaxPlayers.ToString();
    //    }
    //}

    // 룸 패널의 자식 객체를 룸 리스트와 동기화
    public void RoomListUpdate()
    {
        int roomCnt = roomListPanel.transform.childCount;

        Debug.Log("RoomListPanelChildCNt : " + roomCnt);
        for (int i = 0; i < roomCnt; i++)
        {
            if (roomListPanel.transform.GetChild(i).GetComponent<TitleRoomInList>().roomInPlayer == "0")
            {
                RoomsPool.Instance.Release(roomListPanel.transform.GetChild(i).gameObject);
            }
        }
    }


    public void OnClickActiveCreateRoomPanel()
    { 
        createRoomPanel.gameObject.SetActive(true);
    }

    public void SetRoomName(string name)
    {
        roomNameInputField.text = name;
    }
}
