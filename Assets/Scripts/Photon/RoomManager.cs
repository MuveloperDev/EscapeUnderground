using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("( Texts )")]
    [SerializeField] private TextMeshProUGUI cntPlayersTxt = null;                 // 플레이어 수 Text
    [SerializeField] private TextMeshProUGUI roomTitleTxt = null;                  // 방 제목 Text

    [Header("( Panels )")]
    [SerializeField] private Image masterPanel = null;
    [SerializeField] private Image challengerPanel = null;


    public Image MasterPanel { get { return masterPanel; } set { challengerPanel = value; } }
    public Image ChallengerPanel { get { return challengerPanel; } set { challengerPanel = value; } }
    public string CntPlayersTxt { set { cntPlayersTxt.text = value; } }
    // 사운드 매니저
    AudioManager audioManager = null;
    private void OnEnable()
    {
        audioManager = FindObjectOfType<AudioManager>();
        roomTitleTxt.text = PhotonNetwork.CurrentRoom.Name;

        // MasterInfoPanel 업데이트
        MasterPanel.gameObject.SetActive(true);
        TextMeshProUGUI mastertext = MasterPanel.GetComponentInChildren<TextMeshProUGUI>();
        mastertext.text = PhotonNetwork.MasterClient.NickName;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedROom");
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2) PhotonNetwork.LoadLevel("GameScene");
    }


}
