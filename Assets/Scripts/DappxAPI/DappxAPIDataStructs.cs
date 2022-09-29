
/// <summary>
/// Bring UserProfile to DappxAPI
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

    public override string ToString()
    {
        return $"Status :{Status} StatusCode :{StatusCode} Message :{Message} UserName : {userProfile.username}";
    }
}

/// <summary>
/// Bring SessionID to DappxAPI
/// </summary>
[System.Serializable]
public class GetSessionID
{
    public string Status;
    public string StatusCode;
    public string Message;
    public string sessionId;

    public override string ToString()
    {
        return $"Status :{Status} StatusCode :{StatusCode} Message :{Message} sessionId : {sessionId}";
    }
}

/// <summary>
/// Bring BettSetting Info Until Now to DappxAPI
/// </summary>
[System.Serializable]
public class BetSettings
{
    public string message;

    [System.Serializable]
    public class Settings
    { 
        public string _id;
        public string game_id;
        public bool betting;
        public bool maintenance;
        public string createdAt;
        public string updatedAt;
        public int __v;
        public override string ToString()
        {
            return $"_id: {_id} game_id: {game_id} betting: {betting} maintenance: {maintenance}  createdAt: {createdAt}  updatedAt: {updatedAt}  __v: {__v} ";
        }
    }

    [System.Serializable]
    public class Bets
    { 
        public string _id;
        public string game_id;
        public int amount;
        public int platform_fee;
        public int developer_fee;
        public int win_reward;
        public int win_amount;
        public string createdAt;
        public string updatedAt;
        public int __v;
        public override string ToString()
        {
            return $"_id: {_id} game_id: {game_id} amount: {amount} platform_fee: {platform_fee}  developer_fee: {developer_fee}  win_reward: {win_reward}  win_amount: {win_amount} createdAt: {createdAt} updatedAt: {updatedAt} __v: {__v} ";
        }
    }

    [System.Serializable]
    public class Data
    {
        public Settings settings;
        public Bets[] bets;
    }

    public Data data;
    public override string ToString()
    {
        return $"message : {message}";
    }
}

/// <summary>
/// Check amount coin to Ace, Zera, Dappx to DappxAPI
/// </summary>
[System.Serializable]
public class BalanceInfo
{
    public string message;

    [System.Serializable]
    public class Data
    {
        public int balance;
    }

    public Data data;

    public override string ToString()
    {
        return $"message:{message} Balance : {data.balance}";
    }
}


// Request Place Bet
public class RequestBettingPlcaeBet
{
    public string[] player_session_id;
    public string bet_id;
}
/// <summary>
/// Bet Coin
/// </summary>
[System.Serializable]
public class ResponseBettingPlaceBet
{
    // 코인배팅
    public string message;

    [System.Serializable]
    public class Data
    {
        public string betting_id;
    }
    public Data data;
}

// Request Declare Winner
public class RequestBettingDeclareWinner
{
    public string betting_id;
    public string winner_player_id;
    public object match_details;
}

/// <summary>
/// Declare Winner
/// </summary>
[System.Serializable]
public class ResponseBettingDeclareWinner
{
    // 배팅 코인을 획득하게 될 승자.
    public string message;

    [System.Serializable]
    public class Data
    {
        public int amount_won;
    }
    public Data data;
}

// Request Disconnect
public class RequestBettingDisconnect
{
    public string betting_id;
}
/// <summary>
/// Disconnect
/// </summary>
[System.Serializable]
public class ResponseBettingDisconnect
{
    // 배팅된 코인 반환
    public string message;

    [System.Serializable]
    public class Data
    { 
    }
    public Data data;
}
