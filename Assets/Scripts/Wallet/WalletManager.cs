using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : SingleTon<WalletManager>
{
    [SerializeField] static float moneyBet;
    [SerializeField] ToggleGroup setBetMoneyTogglesGroup = null;
    [SerializeField] Toggle[] betMoneyToggles = null;
    [SerializeField] MyWallet myWallet = null;

    private void Awake()
    {
        // MyWallet은 파괴되지 않는다.
        // MyWallet이 하나 이상이면 파괴하고 하나만 남긴다.
        WalletManager[] obj = FindObjectsOfType<WalletManager>();
        if (obj.Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);


    }

    // 방을 생성할 때 활성화된 배팅금 설정 토글로 cost를 결정한다.
    public float SetCost()
    {
        
        setBetMoneyTogglesGroup = FindObjectOfType<ToggleGroup>();
        betMoneyToggles = setBetMoneyTogglesGroup.GetComponentsInChildren<Toggle>();
        Toggle activeBetToggle = Array.Find(betMoneyToggles, delegate (Toggle toggle) { return toggle.isOn == true; });
        float cost = float.Parse(activeBetToggle.name);
        return cost;
    }

    // 돈을 배팅 한다.
    public void BetMoney(float cost)
    {
        myWallet.MyMoney = -cost;
        moneyBet += cost;
        myWallet.moneyUpdate();
    }

    // 돈을 돌려 받는다.
    public void GiveBackMoney()
    {
        myWallet.MyMoney = moneyBet;
        moneyBet = 0;
        myWallet.moneyUpdate();
    }

    // 승리시 돈을 가진다.
    public void GetMoney()
    {
        myWallet.MyMoney = moneyBet * 2;
        moneyBet = 0;
        myWallet.moneyUpdate();
    }

    // 패배시 돈을 잃는다.
    public void LoseMoney() => moneyBet = 0;
}
