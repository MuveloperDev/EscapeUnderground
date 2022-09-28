using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DappxAPIDataStructs : MonoBehaviour
{

}
/// <summary>
/// DappX의 유저 정보를 가져온다.
/// </summary>
[System.Serializable]
public class GetUserProfile
{
    public string Status;
    public string StatusCode;
    public string Message;

    [System.Serializable]
    public class UserProfile
    {
        public string referral_by;
        public string referral_code;
        public string username;
        public string email_id;
        public string public_address;
        public string _id;
        public string upline;
    }
    public UserProfile userProfile;
}
