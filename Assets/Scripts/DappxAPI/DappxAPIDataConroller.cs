using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DappxAPIDataConroller : MonoBehaviour
{
    public static DappxAPIDataConroller Instance;

    [SerializeField] string betId = null;

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
    ResponseBettingPlaceBet         responseBettingPlaceBet         = null;     // 배팅 정보

    public string[] userProfileID = new string[2];
    public string betting_id { get; set; }

    public string[] SessionIdArr { get { return sessionIdArr; } set { sessionIdArr = value; } }
    public GetUserProfile   GetUserProfile      { get { return getUserProfile; }}
    public GetSessionID     GetSessionID        { get { return getSessionID; }}
    public BetSettings      BetSettings         { get { return betSettings; }}
    public BalanceInfo      ZeraBalanceInfo     { get { return zeraBalanceInfo; }}
    public BalanceInfo      AceBalanceInfo      { get { return aceBalanceInfo; }}
    public BalanceInfo      DappXBalanceInfo    { get { return dappXBalanceInfo; }}
    public ResponseBettingPlaceBet ResponseBettingPlaceBet { get { return responseBettingPlaceBet; }}
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
    public void BettingZara_DeclareWinner(int value)
    {
        string userProfileId = userProfileID[value];
        Debug.Log("UserProfileID : " + userProfileId);
        StartCoroutine(ProcessRequestBettingZara_DeclareWinner(userProfileId));
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
            else SceneManager.LoadScene("TitleScene");
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
                betId = betSettings.data.bets[0]._id;
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

    // 제라코인 배팅 시작
    IEnumerator ProcessRequestBettingZera(string[] sessionIdArr)
    {
        

        // 서버에 Json파일로 넘겨주기 위해 requestBettingPlcaeBet 데이터 구조를 이용해 넘겨준다.
        RequestBettingPlcaeBet requestBettingPlcaeBet = new RequestBettingPlcaeBet();

        // 저장한 플레이어들의 SessionID 배열을 할당한다.
        requestBettingPlcaeBet.players_session_id = sessionIdArr;
        Debug.Log("####################### Retrun Sucsess sessionIDARR ");
        Debug.Log("##Sucsess sessionIDARR " + requestBettingPlcaeBet.players_session_id);

        // 배팅 설정.
        requestBettingPlcaeBet.bet_id = betId;
        Debug.Log("####################### Retrun Sucsess BettingSettings ");

        yield return RequestCoinPlaceBet("zera", requestBettingPlcaeBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("### CoinPlaceBet : " + response.message);
                responseBettingPlaceBet = response;

                // 플레이어들에게 BettingId를 저장하게 한다.
                BrickListManager[] brickListManager = FindObjectsOfType<BrickListManager>();
                foreach (BrickListManager brickManager in brickListManager)
                {
                    brickManager.CallSetBettingId(responseBettingPlaceBet.data.betting_id);
                }
            }
        });
    }


    // 승자 제라코인 회수
    IEnumerator ProcessRequestBettingZara_DeclareWinner(string userProfile_id)
    {
        Debug.Log("UserProfileID : " + userProfile_id);
        ResponseBettingDeclareWinner responseBettingDeclareWinner = null;

        // 서버에 Json파일로 넘겨주기 위해 RequestBettingDeclareWinner 데이터 구조를 이용해 넘겨준다.
        RequestBettingDeclareWinner requestBettingDeclareWinner = new RequestBettingDeclareWinner();
        // 배팅 id와 승리한 userProfile_id를 넘겨주기 위해 저장한다.
        // 배팅후 반환되는 배팅 정보의 아이디를 할당한다.
        // 승자 유저 프로필 아이디 할당.



        Debug.Log("######## bettingID : " + betting_id);
        // 다른 플레이어들과 전부 동기화 시켜둔 bettingId를 담아준다
        requestBettingDeclareWinner.betting_id = betting_id;
        Debug.Log("######## requestBettingDeclareWinner.betting_id : " + requestBettingDeclareWinner.betting_id);

        Debug.Log(requestBettingDeclareWinner.betting_id);
        //requestBettingDeclareWinner.winner_player_id = getUserProfile.userProfile._id;
        requestBettingDeclareWinner.winner_player_id = userProfile_id;
        Debug.Log("############## winner Player ID : "  + requestBettingDeclareWinner.winner_player_id);

        yield return RequestCoinDeclareWinner("zera", requestBettingDeclareWinner, (response) => {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                // 이러면 웹으로
                responseBettingDeclareWinner = response;
            }
        
        });
    }

    // 베팅금액 반환
    public void Betting_Zera_Disconnect()
    {
        StartCoroutine(processRequestBetting_Zera_Disconnect());
    }
    IEnumerator processRequestBetting_Zera_Disconnect()
    {
        ResponseBettingDisconnect resBettingDisconnect = null;
        RequestBettingDisconnect reqBettingDisconnect = new RequestBettingDisconnect();
        reqBettingDisconnect.betting_id = responseBettingPlaceBet.data.betting_id;// resSettigns.data.bets[1]._id;
        yield return RequestCoinDisConnect("zera", reqBettingDisconnect, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDisconnect : " + response.message);
                resBettingDisconnect = response;
            }
        });
    }

    //----------------------------------------------------------

    #region LocalHostApi
    /// <summary>
    /// Bring UserProfile to DappxAPI
    /// </summary>
    IEnumerator RequestGetUserInfo(Action<GetUserProfile> callback)
    {
        // 유저 프로필을 가져온다.
        // UnityWebRequest : 웹 서버와 통신을 제공한다.
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile"))
        {
            // SendWebRequest : 원격서버와 통신을 시작하는 함수이다.
            yield return www.SendWebRequest();

            //FromJson : Json의 Array형태의 값을 GetUserProfile으로 반환해준다.
            // downloadHandler 서버의 API JSON 정보를 다운로드 한다.
            GetUserProfile getUserProfile = JsonUtility.FromJson<GetUserProfile>(www.downloadHandler.text);
            callback(getUserProfile);
            Debug.Log(getUserProfile.ToString());
            //Debug.Log("StatusCode" + getUserProfile.StatusCode);
        }
    }

    /// <summary>
    /// Bring SessionID to DappxAPI
    /// </summary>
    IEnumerator RequestGetSessionID(Action<GetSessionID> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getsessionid"))
        { 
            yield return www.SendWebRequest();
            GetSessionID getSessionID = JsonUtility.FromJson<GetSessionID>(www.downloadHandler.text);
            callback(getSessionID);
        }
    }

    #endregion

    /// <summary>
    /// Bring BettSetting Info to DappxAPI
    /// </summary>
    IEnumerator RequestBetSettings(Action<BetSettings> callback)
    {
        string url = GetBaseURL() + "/v1/betting/settings";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        { 
            www.SetRequestHeader("api-key", API_KEY);
            yield return www.SendWebRequest();
            BetSettings settings = JsonUtility.FromJson<BetSettings>(www.downloadHandler.text);
            callback(settings);
        }
    }

    #region CoinBalance
    /// <summary>
    /// Check amount coin to Ace, Zera, Dappx to DappxAPI
    /// </summary>
    IEnumerator RequestCoinBalance(string coinStorage, string sessionID, Action<BalanceInfo> callback)
    {
        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/balance/" + sessionID;
        Debug.Log(url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("api-key", API_KEY);
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            BalanceInfo balanceInfo = JsonUtility.FromJson<BalanceInfo>(www.downloadHandler.text);
            callback(balanceInfo);
        }
    }
    #endregion

    #region BettingCoroutine
    /// <summary>
    /// Bet Coin
    /// </summary>
    IEnumerator RequestCoinPlaceBet(string coinStorage, RequestBettingPlcaeBet request, Action<ResponseBettingPlaceBet> callback)
    {
        // 코인배팅 코루틴

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/place-bet";

        // 배팅을 한 플레이어들의 sessionId와 Bet_id를 Json으로 넘겨준다.
        string requestJsonData = JsonUtility.ToJson(request);
        Debug.Log(requestJsonData);
        using (UnityWebRequest www = UnityWebRequest.Post(url, requestJsonData))
        { 

            // Post는 웹서버에 생성 요청을 하기 때문에 UTF8로 Json값을 인코딩해주어야 한다.
            // 서버와 데이터를 주고받을 때 서버는 byte형식으로 받기때문에
            // byte의 버퍼 형태로 변환해서 넘겨주어야한다.
            byte[] buff = System.Text.Encoding.UTF8.GetBytes(requestJsonData);

            // 버퍼의 임시저장소 형태로 jsom데이터를 저장하여 buffer를 서버에 Upload해준다.
            www.uploadHandler = new UploadHandlerRaw(buff);

            www.SetRequestHeader("api-key", API_KEY);

            // 헤더에는 바디데이터가 json형식이라는 것을 명시해주어야 한다.
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            ResponseBettingPlaceBet responseBettingPlaceBet = JsonUtility.FromJson<ResponseBettingPlaceBet>(www.downloadHandler.text);
            callback(responseBettingPlaceBet);
        }

    }

    /// <summary>
    /// Coin Declare Winner
    /// </summary>
    IEnumerator RequestCoinDeclareWinner(string coinStorage, RequestBettingDeclareWinner request, Action<ResponseBettingDeclareWinner> callback)
    {
        // 승자 코인 회수 코루틴

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/declare-winner";

        Debug.Log("############## winner Player ID : " + request.winner_player_id);

        // 저장한 정보를 Json형태로 담아준다.
        string requestJsonData = JsonUtility.ToJson(request);
        Debug.Log("requestJsonData : " + requestJsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(url, requestJsonData))
        { 
            byte[] buff = System.Text.Encoding.UTF8.GetBytes(requestJsonData);
            www.uploadHandler = new UploadHandlerRaw(buff);

            www.SetRequestHeader("api-key", API_KEY);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            ResponseBettingDeclareWinner responseBettingDeclareWinner = JsonUtility.FromJson<ResponseBettingDeclareWinner>(www.downloadHandler.text);
            callback(responseBettingDeclareWinner);
        }
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

        using (UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData))
        { 
            byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
            www.uploadHandler = new UploadHandlerRaw(buff);

            www.SetRequestHeader("api-key", API_KEY);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            ResponseBettingDisconnect responseBettingDisconnect = JsonUtility.FromJson<ResponseBettingDisconnect>(www.downloadHandler.text);
            callback(responseBettingDisconnect);
        }

    }
    #endregion
}
