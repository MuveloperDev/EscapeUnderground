using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DappxAPIDataConroller : MonoBehaviour
{
    public static DappxAPIDataConroller Instance;

    [SerializeField] string[] sessionIdArr = new string[2];

    [Header("[등록된 프로젝트에서 획득가능한 API 키]")]
    [Tooltip("이것은 http://odin-registration-sat.browseosiris.com/# 에 등록된 프로젝트를 통해서 획득할 수 있는 API Key 이다.\nhttps://odin-registration.browseosiris.com/ 는 Production URL")]
    [SerializeField] string API_KEY = "3eg6tzd40ytTvxZJrDXSu9";


    [Header("[Betting Backend Base URL")]
    [SerializeField] string FullAppsProductionURL = "https://odin-api.browseosiris.com";
    [SerializeField] string FullAppsStagingURL = "https://odin-api-sat.browseosiris.com";


    #region Don't_Destroy_Awake
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    #region BaseURL
    /// <summary>
    /// BaseURL
    /// </summary>
    /// <remarks>
    /// 스테이징 단계이기때문에 "https://odin-api-sat.browseosiris.com" 사용.
    /// 프로덕션 단계이면 "https://odin-api.browseosiris.com" 사용.
    /// </remarks>
    string GetBaseURL()
    {
        // 프로덕션 단계라면 ProductionURL을 사용.
        // return FullAppsProductionURL;
        // 프로덕션 단계가 아니기 때문에 Staging URL을 사용.
        return FullAppsStagingURL;
    }
    #endregion

    #region Variables
    // Json 형태의 유저 정보를 담을 Class.
    GetUserProfile  getUserProfile      = null;
    GetSessionID    getSessionID        = null;
    BetSettings     betSettings         = null;
    BalanceInfo     zeraBalanceInfo     = null;     // Save CoinStorageInfo 
    BalanceInfo     aceBalanceInfo      = null;     // Save CoinStorageInfo 
    BalanceInfo     dappXBalanceInfo    = null;     // Save CoinStorageInfo 

    public string[] SessionIdArr { get { return sessionIdArr; } set { sessionIdArr = value; } }
    public GetUserProfile   GetUserProfile      { get { return getUserProfile; }}
    public GetSessionID     GetSessionID        { get { return getSessionID; }}
    public BetSettings      BetSettings         { get { return betSettings; }}
    public BalanceInfo      ZeraBalanceInfo     { get { return zeraBalanceInfo; }}
    public BalanceInfo      AceBalanceInfo      { get { return aceBalanceInfo; }}
    public BalanceInfo      DappXBalanceInfo    { get { return dappXBalanceInfo; }}
    #endregion


    #region Function_for_Connectting_UI
    public void OnClick_StartUserSetting() => StartCoroutine(ProcessRequestGetUserInfo());

    // zera 잔고 확인
    public void Check_ZeraCoinBalance() => StartCoroutine(ProcessRequestCoinBalance("zera"));
    public void Check_AceCoinBalance() => StartCoroutine(ProcessRequestCoinBalance("ace"));
    public void Check_DappXCoinBalance() => StartCoroutine(ProcessRequestCoinBalance("dappx"));
    #endregion

    #region Bet_Function
    /// <summary>
    /// Betting Zera Coin
    /// </summary>
    public void BettingCoinToZera(string[] sessionIdArr)
    { 
        StartCoroutine(ProcessRequestBettingZera(sessionIdArr));
    }
    /// <summary>
    /// Declare Winner
    /// </summary>
    public void BettingZara_DeclareWinner()
    {
        StartCoroutine(ProcessRequestBettingZara_DeclareWinner());
    }

    #endregion

    // Get UserProfile Info to DappxAPI
    IEnumerator ProcessRequestGetUserInfo()
    {
        yield return RequestGetUserInfo((response) =>
        {
            if (response != null)
            {
                getUserProfile = response;
                Debug.Log("## " + getUserProfile.ToString());
                // UserInfo
                StartCoroutine(ProcessRequestGetSessionID());
            }
        });
    }

    // Get SessionID Info to DappxAPI
    IEnumerator ProcessRequestGetSessionID()
    {
        yield return RequestGetSessionID((response) => {
            if (response != null)
            {
                getSessionID = response;
                Debug.Log("## GetSessionID : " + getSessionID.ToString());
                StartCoroutine(ProcessRequestBetSettings());
            }
            
        });
    }

    // Bring BettSetting Info to DappxAPI Until Now 
    IEnumerator ProcessRequestBetSettings()
    {
        yield return RequestBetSettings((response) => {
            if (response != null)
            {
                betSettings = response;
                Debug.Log("## BetSettings " + betSettings.ToString());
                Debug.Log("## BetSettings.settings : " + betSettings.data.settings.ToString());
                for (int i = 0; i < betSettings.data.bets.Length; i++)
                {
                    Debug.Log("## BetSettings.bets : " + i);
                    Debug.Log("## BetSettings.bets : " + betSettings.data.bets[i].ToString());
                }
                Check_ZeraCoinBalance();
                Check_AceCoinBalance();
                Check_DappXCoinBalance();
            }
        });
    }

    // Check amount coin to Ace, Zera, Dappx to DappxAPI
    IEnumerator ProcessRequestCoinBalance(string coinStorage)
    {
        
        yield return RequestCoinBalance(coinStorage, getSessionID.sessionId, (response) =>
        {
            if (response != null)
            {
                if (coinStorage == "zera") zeraBalanceInfo = response;
                else if (coinStorage == "ace") aceBalanceInfo = response;
                else dappXBalanceInfo = response;

                Debug.Log("## Response " + coinStorage + " Balance : " + response.ToString());
            }
        });
    }

    // 자라코인 배팅 시작
    IEnumerator ProcessRequestBettingZera(string[] sessionIdArr)
    { 
        ResponseBettingPlaceBet responseBettingPlaceBet = null;
        RequestBettingPlcaeBet requestBettingPlcaeBet = new RequestBettingPlcaeBet();
        // 저장한 SessionID를 넣어준다.
        requestBettingPlcaeBet.player_session_id = sessionIdArr;
        // 배팅 설정.
        requestBettingPlcaeBet.bet_id = betSettings.data.bets[0]._id;
        yield return RequestCoinPlaceBet("zera", requestBettingPlcaeBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("### CoinPlaceBet : " + response.message);
                responseBettingPlaceBet = response;
            }
        });
    }

    // 승자 자라코인 회수
    IEnumerator ProcessRequestBettingZara_DeclareWinner()
    { 
        ResponseBettingDeclareWinner responseBettingDeclareWinner = null;
        RequestBettingDeclareWinner requestBettingDeclareWinner = new RequestBettingDeclareWinner();
        requestBettingDeclareWinner.betting_id = betSettings.data.bets[0]._id;
        requestBettingDeclareWinner.winner_player_id = getUserProfile.userProfile._id;
        yield return RequestCoinDeclareWinner("Zara", requestBettingDeclareWinner, (response) => {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                // 이러면 웹으로
                responseBettingDeclareWinner = response;
            }
        
        });
    }

    //// 베팅금액 반환
    //public void OnClick_Betting_Zera_Disconnect()
    //{
    //    StartCoroutine(processRequestBetting_Zera_Disconnect());
    //}
    //IEnumerator processRequestBetting_Zera_Disconnect()
    //{
    //    ResBettingDisconnect resBettingDisconnect = null;
    //    ReqBettingDisconnect reqBettingDisconnect = new ReqBettingDisconnect();
    //    reqBettingDisconnect.betting_id = selectedBettingID;// resSettigns.data.bets[1]._id;
    //    yield return requestCoinDisconnect(reqBettingDisconnect, (response) =>
    //    {
    //        if (response != null)
    //        {
    //            Debug.Log("## CoinDisconnect : " + response.message);
    //            resBettingDisconnect = response;
    //        }
    //    });
    //}

    //----------------------------------------------------------

    #region LocalHostApi
    /// <summary>
    /// Bring UserProfile to DappxAPI
    /// </summary>
    IEnumerator RequestGetUserInfo(Action<GetUserProfile> callback)
    {
        // 유저 프로필을 가져온다.
        // UnityWebRequest : 웹 서버와 통신을 제공한다.
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile");
        // SendWebRequest : 원격서버와 통신을 시작하는 함수이다.
        yield return www.SendWebRequest();

        //FromJson : Json의 Array형태의 값을 GetUserProfile으로 반환해준다.
        // downloadHandler 서버의 API JSON 정보를 다운로드 한다.
        GetUserProfile getUserProfile = JsonUtility.FromJson<GetUserProfile>(www.downloadHandler.text);
        callback(getUserProfile);
        Debug.Log(getUserProfile.ToString());
        //Debug.Log("StatusCode" + getUserProfile.StatusCode);
    }

    /// <summary>
    /// Bring SessionID to DappxAPI
    /// </summary>
    IEnumerator RequestGetSessionID(Action<GetSessionID> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getsessionid");
        yield return www.SendWebRequest();
        GetSessionID getSessionID = JsonUtility.FromJson<GetSessionID>(www.downloadHandler.text);
        callback(getSessionID);

    }

    #endregion

    /// <summary>
    /// Bring BettSetting Info to DappxAPI
    /// </summary>
    IEnumerator RequestBetSettings(Action<BetSettings> callback)
    {
        string url = GetBaseURL() + "/v1/betting/settings";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        BetSettings settings = JsonUtility.FromJson<BetSettings>(www.downloadHandler.text);
        callback(settings);
    }
    
    // ???? 문서에는 apiKey 필요없다고 했는데..
    /// <summary>
    /// Check amount coin to Ace, Zera, Dappx to DappxAPI
    /// </summary>
    IEnumerator RequestCoinBalance(string coinStorage, string sessionID, Action<BalanceInfo> callback)
    {
        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/balance/" + sessionID;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        BalanceInfo balanceInfo = JsonUtility.FromJson<BalanceInfo>(www.downloadHandler.text);
        callback(balanceInfo);
    }

    #region BettingCoroutin
    /// <summary>
    /// Bet Coin
    /// </summary>
    IEnumerator RequestCoinPlaceBet(string coinStorage, RequestBettingPlcaeBet request, Action<ResponseBettingPlaceBet> callback)
    {
        // 코인배팅 코루틴

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/place-bet";

        // ??? 이거 왜하는 거지
        string reqJsonData = JsonUtility.ToJson(request);
        Debug.Log(reqJsonData);

        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        // Post는 웹에 생성 요청을 하기 때문에 UTF8로 Json값을 인코딩해주어야 한다.
        // 서버와 데이터를 주고받을 때 서버는 byte형식으로 받기때문에
        // byte로 변환해서 넘겨주어야한다.
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        // ???? 이건 무엇?
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        // ????? 문서 헤더에는 없는데...?
        // 바디데이터가 json형식이라는 것을 명시
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        ResponseBettingPlaceBet responseBettingPlaceBet = JsonUtility.FromJson<ResponseBettingPlaceBet>(www.downloadHandler.text);
        callback(responseBettingPlaceBet);

    }

    /// <summary>
    /// Coin Declare Winner
    /// </summary>
    IEnumerator RequestCoinDeclareWinner(string coinStorage, RequestBettingDeclareWinner request, Action<ResponseBettingDeclareWinner> callback)
    {
        // 승자 코인 회수 코루틴

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/declare-winner";

        string reqJsonData = JsonUtility.ToJson(request);
        Debug.Log(reqJsonData);

        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);

        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        ResponseBettingDeclareWinner responseBettingDeclareWinner = JsonUtility.FromJson<ResponseBettingDeclareWinner>(www.downloadHandler.text);
        callback(responseBettingDeclareWinner);

    }

    // 돈 회수 함수 구현 X 게임 기능상 필요 X
    /// <summary>
    /// Coin DisConnect
    /// </summary>
    IEnumerator RequestCoinDisConnect(string coinStorage, RequestBettingDisconnect request, Action<ResponseBettingDisconnect> callback)
    {
        // 배팅괸 코인을 반환하는 코루틴.

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/disconnect";

        string reqJsonData = JsonUtility.ToJson(request);
        Debug.Log(reqJsonData);

        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);


        www.uploadHandler = new UploadHandlerRaw(buff);

        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        ResponseBettingDisconnect responseBettingDisconnect = JsonUtility.FromJson<ResponseBettingDisconnect>(www.downloadHandler.text);
        callback(responseBettingDisconnect);

    }
    #endregion
}
