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
    [SerializeField] TextMeshProUGUI walletTxt = null;

    public Action moneyUpdate;
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

        moneyUpdate = delegate () { walletTxt.text = "MyMoney : " + money.ToString(); };
    }

    private void Start()
    {
        money = 10000;
        moneyUpdate();
    }

    private void Update()
    {
        if (walletTxt == null) walletTxt = GameObject.FindGameObjectWithTag("Wallet").GetComponent<TextMeshProUGUI>();
        
    }

}
