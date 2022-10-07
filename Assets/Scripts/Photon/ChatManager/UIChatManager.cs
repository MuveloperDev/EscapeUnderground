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

    // 연결, 채널 및 메시지 처리를 위한 Photon Chat API의 중앙 클래스.
    // 정기적으로 Service를 호출하여 게임 루프에 통합. 
    // Photon Chat 애플리케이션으로 설정된 AppId로 Connect를 호출. 
    // 참고: Connect는 여러 클라이언트와 서버 간의 메시지. 짧은 워크플로를 통해 채팅 서버에 연결.

    ChatClient chatClient = null;

    private void Awake()
    {
        ConnectedMyChat = ConnectedChat;
        ClearText = delegate
        {
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

            // 
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
        // 메세지수가 50개가 넘어가면 첫번째 메세지부터 삭제.
        if (content.transform.childCount > 15)
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
        obj.transform.localScale = Vector3.one;
    }

    #region Chat_CallBacks

    //
    // 채팅 시스템의 모든 정보와 로그를 얻을 수 있다.
    // DebugLevel 에 정의 된 enum 타입에 따라 메세지를 출력한다
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
        //Debug.Log(message);
    }

    //
    // 현재 클라이언트의 상태를 출력
    // OnConnected 및 OnDisconnected는 응답하는 콜백함수.
    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
        addChatLine("[System]", "OnChatStateChange : " + state);
    }

    //
    // 서버에 연결시 호출되는 콜백 함수.
    // 클라이언트는 상태를 보내고 채널을 구독하고 메시지를 보내기 전에 연결되어 있야 한다.
    public void OnConnected()
    {
        addChatLine("[System]", "OnConnected");

        //
        // 단일 채널을 구독하고 선택적으로 채널이 생성되는 경우 잘 알려진 채널 속성을 설정.
        // 
        // <param name="channel">구독할 채널 이름</param>
        // <param name="lastMsgId">부재중 메시지만 수신하도록 재구독할 때 이 채널에서 마지막으로 수신된 메시지의 ID, 기본값은 0.</param>
        // <param name="messagesFromHistory">기록에서 수신할 누락된 메시지 수, 기본값은 -1(사용 가능한 기록).
        // 0은 아무것도 반환되지 않는다. 양수 값은 서버 측 제한으로 제한됩니다.</param>
        // <param name="creationOptions">구독할 채널이 생성될 경우 사용할 옵션.</param>
        // 
        chatClient.Subscribe("public", 0);
    }

    // 서버 연결 해제시 호출되는 콜백 함수.
    public void OnDisconnected()
    {
        addChatLine("[System]", "DisConnected");
    }

    //
    // 상대방이 보낸 채팅 메세지를 받아오는 함수
    //클라이언트가 서버에서 새 메시지를 받았음을 앱에 알려준다.
    // 보낸 사람의 수는 'messages'의 메시지 수와 같다.
    //
    // <param name="channelName">메시지가 온 채널</param>
    // <param name="senders">메시지를 보낸 사용자 목록</param>
    // <param name="messages">자체 메시지 목록</param>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        for (int i = 0; i < messages.Length; i++)
            addChatLine(senders[i], messages[i].ToString());
    }

    // 
    // 클라이언트에게 개인 메시지를 알려준다.
    // 
    // <param name="sender">이 메시지를 보낸 사용자</param>
    // <param name="message">자신에게 메시지 보내기</param>
    // <param name="channelName">비공개 메시지의 channelName(자신에게 보낸 메시지는 대상 사용자 이름별로 채널에 추가됨)</param>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    //
    // 다른 사용자의 새 상태(친구 목록에 설정된 사용자에 대한 업데이트를 받는다).
    // 
    // <param name="user">사용자의 이름.</param>
    // <param name="status">해당 사용자의 새 상태.</param>
    // <param name="gotMessage">상태에 로컬로 캐시해야 하는 메시지가 포함되어 있으면 참. False: 이 상태 업데이트에는 메시지가 포함되지 않는다(가지고 있는 메시지는 유지).</param>
    // <param name="message">사용자가 설정한 메시지.</param>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    //
    // 사용자가 공개 채팅 채널을 구독했을때 호출되는 콜백 함수.
    // 
    // <param name="channel">채팅 채널 이름</param>
    // <param name="user">구독한 사용자의 UserId</param>
    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
        addChatLine("[system]", string.Format("OnSubscribed({0})<{1}>", string.Join(",", channels), string.Join(",", results)));
    }

    // 
    // 구독 취소가 되었을때 호출되는 콜백 함수.
    // 채널이 현재 구독 취소된 경우 채널 이름을 반환.
    //
    // Unsubscribe 작업에서 여러 채널이 전송된 경우 OnUnsubscribed는 여러 번 호출된다.
    // 각 호출은 전송된 배열의 일부 또는 "channels" 매개변수의 단일 채널을 사용.
    // "channels" 매개변수의 호출 순서 및 채널 순서는 Unsubscribe 작업의 "channel" 매개변수의 채널 순서와 다를 수 있다.
    // <param name="channels">더 이상 구독하지 않는 채널 이름의 배열이다.</param>

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
        addChatLine("[system]", string.Format("OnSubscribed({0})", string.Join(",", channels)));
    }


    //
    // 사용자가 공개 채팅 채널을 구독했을 때 호출되는 함수.
    // </요약>
    // <param name="channel">채팅 채널 이름</param>
    // <param name="user">구독한 사용자의 UserId</param>
    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }


    //
    // 사용자가 공개 채팅 채널을 구독 취소했을 때 호출되는 콜백 함수.
    // </요약>
    // <param name="channel">채팅 채널 이름</param>
    // <param name="user">구독을 취소한 사용자의 UserId</param>
    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}
