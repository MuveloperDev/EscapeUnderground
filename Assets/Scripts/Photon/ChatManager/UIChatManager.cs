using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIChatManager : MonoBehaviour, IChatClientListener
{
    // 로비에 접속시 커넥트 실행
    public Action ConnectedMyChat;
    public Action ClearText;

    [Header("[ RectView ]")]
    [SerializeField] GridLayoutGroup content = null;
    [Header("[ InputFields ]")]
    [SerializeField] TMP_InputField inputChat= null;

    ChatClient chatClient = null;


    List<GameObject> chatsList = new List<GameObject>();

    private void Awake()
    {
        ConnectedMyChat = ConnectedChat;
        ClearText = delegate
        {
            Debug.Log("ClearText");
            for (int i = 0; i < content.transform.childCount; i++)
                MyChatPool.Instance.Release(content.transform.GetChild(i).gameObject);
        };
    }

    void ConnectedChat()
    {
        // 로비 접속시 대화내용 전체 회수
        Image[] texts =  content.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < texts.Length; i++)
            MyChatPool.Instance.Release(texts[i].gameObject);

        if (chatClient == null)
        {
            //ChatClient에 IChatClientListener를 넘겨주어야 한다.
            chatClient = new ChatClient(this);

            // 호환 플랫폼의 경우 PhotonServerSettings에서 "백그라운드에서 실행"을 사용하도록 설정해야 한다. 
            //이렇게 해 주지 않을경우 어플리케이션이 백그라운드로 전환되면 연결이 끊어진다.
            chatClient.UseBackgroundWorkerForSending = true;

            // 채팅에 필요한 객체를 검사해준다.
            chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);

            chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        }

    }
    void Update()
    {
        if (chatClient != null)
        {
            // 보냈는지 확인한다.
            chatClient.Service();
        }
        if (content.transform.childCount > 50)
        {
            MyChatPool.Instance.Release(content.transform.GetChild(0).gameObject);
        }
    }

    // 채팅창 입력
    public void OnEndEdit(string inStr)
    {
        // 채팅 입력이 아무것도 없으면 리턴
        if (inStr.Length <= 0) return;

        // 포톤 채팅으로 내가 입력한 내용을 보내기
        chatClient.PublishMessage("public", inStr);
        inputChat.text = "";
    }

    void addChatLine(string userName, string ChatLine)
    {
        GameObject obj = MyChatPool.Instance.Get(transform.position);
        obj.transform.SetParent(content.transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = userName;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ChatLine;
    }

    //void ClearText()
    //{
    //    for (int i = 0; i < content.transform.childCount; i++)
    //        MyChatPool.Instance.Release(content.transform.GetChild(i).gameObject);
    //}

    #region Chat_CallBacks
    // 채팅 시스템의 모든 정보와 로그를 얻을 수 있다.
    // DebugLevel 에 정의 된 enum 타입에 따라 메세지를 출력한다
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
        //Debug.Log(message);
    }

    // 현재 클라이언트의 상태를 출력
    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
        addChatLine("[System]", "OnChatStateChange : " + state);
    }

    public void OnConnected()
    {
        addChatLine("[System]", "OnConnected");
        //throw new System.NotImplementedException();
        chatClient.Subscribe("public", 0);
    }

    public void OnDisconnected()
    {
        addChatLine("[System]", "DisConnected");
        //throw new System.NotImplementedException();
    }
    // 상대방이 보낸 채팅 메세지를 받아오는 함수
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        for (int i = 0; i < messages.Length; i++)
            addChatLine(senders[i], messages[i].ToString());
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
        addChatLine("[system]", string.Format("OnSubscribed({0})<{1}>", string.Join(",", channels), string.Join(",", results)));
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
        addChatLine("[system]", string.Format("OnSubscribed({0})", string.Join(",", channels)));
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}
