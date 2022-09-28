using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DappxAPIDataConroller : MonoBehaviour
{
    public static DappxAPIDataConroller Instance;

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
    GetUserProfile getUserProfile = null;
    #endregion

    public void OnClick_StartUserSetting()
    {
        StartCoroutine(ProcessRequestGetUserInfo());
    }

    IEnumerator ProcessRequestGetUserInfo()
    {
        yield return RequestGetUserInfo((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                getUserProfile = response;
                Debug.Log("StatusCode" + getUserProfile.StatusCode);
            }

        });
    }


    IEnumerator RequestGetUserInfo(Action<GetUserProfile> callback)
    {
        // 유저 프로필을 가져온다.
        // UnityWebRequest : 웹 서버와 통신을 제공한다.
        UnityWebRequest www = UnityWebRequest.Get(" http://localhost:8546/api/getuserprofile");
        // SendWebRequest : 원격서버와 통신을 시작하는 함수이다.
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        //FromJson : Json의 Array형태의 값을 GetUserProfile으로 반환해준다.
        // downloadHandler 서버의 API JSON 정보를 다운로드 한다.
        GetUserProfile getUserProfile = JsonUtility.FromJson<GetUserProfile>(www.downloadHandler.text);
        callback(getUserProfile);
    }
}
