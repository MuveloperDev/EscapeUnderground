using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyWallet : MonoBehaviour
{
    public static MyWallet Instance;

    [SerializeField] static float money;
    [SerializeField] TextMeshProUGUI zeraCoinTxt = null;
    [SerializeField] TextMeshProUGUI aceCoinTxt = null;
    [SerializeField] TextMeshProUGUI dappXCoinTxt = null;

    [SerializeField] DappxAPIDataConroller dappxAPIDataConroller = null;
    public float MyMoney { get { return money; } set { money += value; } }
    private void Awake()
    {
        // MyWallet은 파괴되지 않는다.
        // MyWallet이 하나 이상이면 파괴하고 하나만 남긴다.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            zeraCoinTxt = GameObject.FindGameObjectWithTag("Wallet").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            aceCoinTxt = GameObject.FindGameObjectWithTag("Wallet").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            dappXCoinTxt = GameObject.FindGameObjectWithTag("Wallet").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        dappxAPIDataConroller = FindObjectOfType<DappxAPIDataConroller>();
    }

    private void Start()
    {
        //MoneyUpdate();
    }

    public void MoneyUpdate()
    {
        zeraCoinTxt.text = "ZeraCoin : " + dappxAPIDataConroller.ZeraBalanceInfo.data.balance;
        aceCoinTxt.text = "AceCoin : " + dappxAPIDataConroller.AceBalanceInfo.data.balance;
        dappXCoinTxt.text = "DappXCoin : " + dappxAPIDataConroller.DappXBalanceInfo.data.balance;
    }


}
