using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DappxAPIDataConroller : MonoBehaviour
{
    public static DappxAPIDataConroller Instance;

    [SerializeField] string[] sessionIdArr = new string[2];

    [Header("[��ϵ� ������Ʈ���� ȹ�氡���� API Ű]")]
    [Tooltip("�̰��� http://odin-registration-sat.browseosiris.com/# �� ��ϵ� ������Ʈ�� ���ؼ� ȹ���� �� �ִ� API Key �̴�.\nhttps://odin-registration.browseosiris.com/ �� Production URL")]
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
    /// ������¡ �ܰ��̱⶧���� "https://odin-api-sat.browseosiris.com" ���.
    /// ���δ��� �ܰ��̸� "https://odin-api.browseosiris.com" ���.
    /// </remarks>
    string GetBaseURL()
    {
        // ���δ��� �ܰ��� ProductionURL�� ���.
        // return FullAppsProductionURL;
        // ���δ��� �ܰ谡 �ƴϱ� ������ Staging URL�� ���.
        return FullAppsStagingURL;
    }
    #endregion

    #region Variables
    // Json ������ ���� ������ ���� Class.
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

    // zera �ܰ� Ȯ��
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

    // �ڶ����� ���� ����
    IEnumerator ProcessRequestBettingZera(string[] sessionIdArr)
    {
        ResponseBettingPlaceBet responseBettingPlaceBet = null;

        // ������ Json���Ϸ� �Ѱ��ֱ� ���� requestBettingPlcaeBet ������ ������ �̿��� �Ѱ��ش�.
        RequestBettingPlcaeBet requestBettingPlcaeBet = new RequestBettingPlcaeBet();

        // ������ �÷��̾���� SessionID �迭�� �Ҵ��Ѵ�.
        requestBettingPlcaeBet.player_session_id = sessionIdArr;
        Debug.Log("####################### Retrun Sucsess sessionIDARR ");
        Debug.Log("##Sucsess sessionIDARR " + requestBettingPlcaeBet.player_session_id);

        // ���� ����.
        requestBettingPlcaeBet.bet_id = betSettings.data.bets[0]._id;
        Debug.Log("####################### Retrun Sucsess BettingSettings ");

        yield return RequestCoinPlaceBet("zera", requestBettingPlcaeBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("### CoinPlaceBet : " + response.message);
                responseBettingPlaceBet = response;
            }
        });
    }

    // ���� �ڶ����� ȸ��
    IEnumerator ProcessRequestBettingZara_DeclareWinner()
    { 
        ResponseBettingDeclareWinner responseBettingDeclareWinner = null;

        // ������ Json���Ϸ� �Ѱ��ֱ� ���� RequestBettingDeclareWinner ������ ������ �̿��� �Ѱ��ش�.
        RequestBettingDeclareWinner requestBettingDeclareWinner = new RequestBettingDeclareWinner();
        // ���� id�� �¸��� userProfile_id�� �Ѱ��ֱ� ���� �����Ѵ�.
        requestBettingDeclareWinner.betting_id = betSettings.data.bets[0]._id;
        requestBettingDeclareWinner.winner_player_id = getUserProfile.userProfile._id;

        yield return RequestCoinDeclareWinner("Zara", requestBettingDeclareWinner, (response) => {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                // �̷��� ������
                responseBettingDeclareWinner = response;
            }
        
        });
    }

    //// ���ñݾ� ��ȯ
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
        // ���� �������� �����´�.
        // UnityWebRequest : �� ������ ����� �����Ѵ�.
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile");
        // SendWebRequest : ���ݼ����� ����� �����ϴ� �Լ��̴�.
        yield return www.SendWebRequest();

        //FromJson : Json�� Array������ ���� GetUserProfile���� ��ȯ���ش�.
        // downloadHandler ������ API JSON ������ �ٿ�ε� �Ѵ�.
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

    #region CoinBalance
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
    #endregion

    #region BettingCoroutine
    /// <summary>
    /// Bet Coin
    /// </summary>
    IEnumerator RequestCoinPlaceBet(string coinStorage, RequestBettingPlcaeBet request, Action<ResponseBettingPlaceBet> callback)
    {
        // ���ι��� �ڷ�ƾ

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/place-bet";

        // ������ �� �÷��̾���� sessionId�� Bet_id�� Json���� �Ѱ��ش�.
        string requestJsonData = JsonUtility.ToJson(request);
        Debug.Log(requestJsonData);

        UnityWebRequest www = UnityWebRequest.Post(url, requestJsonData);

        // Post�� �������� ���� ��û�� �ϱ� ������ UTF8�� Json���� ���ڵ����־�� �Ѵ�.
        // ������ �����͸� �ְ����� �� ������ byte�������� �ޱ⶧����
        // byte�� ���� ���·� ��ȯ�ؼ� �Ѱ��־���Ѵ�.
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(requestJsonData);

        // ������ �ӽ������ ���·� jsom�����͸� �����Ͽ� buffer�� ������ Upload���ش�.
        www.uploadHandler = new UploadHandlerRaw(buff);

        www.SetRequestHeader("api-key", API_KEY);

        // ������� �ٵ����Ͱ� json�����̶�� ���� �������־�� �Ѵ�.
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
        // ���� ���� ȸ�� �ڷ�ƾ

        string url = GetBaseURL() + "/v1/betting/" + coinStorage + "/declare-winner";

        // ������ ������ Json���·� ����ش�.
        string requestJsonData = JsonUtility.ToJson(request);
        Debug.Log(requestJsonData);

        UnityWebRequest www = UnityWebRequest.Post(url, requestJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(requestJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);

        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        ResponseBettingDeclareWinner responseBettingDeclareWinner = JsonUtility.FromJson<ResponseBettingDeclareWinner>(www.downloadHandler.text);
        callback(responseBettingDeclareWinner);

    }

    // �� ȸ�� �Լ� ���� X ���� ��ɻ� �ʿ� X
    /// <summary>
    /// Coin DisConnect
    /// </summary>
    IEnumerator RequestCoinDisConnect(string coinStorage, RequestBettingDisconnect request, Action<ResponseBettingDisconnect> callback)
    {
        // ���ñ� ������ ��ȯ�ϴ� �ڷ�ƾ.

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