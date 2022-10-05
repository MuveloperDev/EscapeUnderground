using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;

public class UIManager : MonoBehaviourPun
{
    [SerializeField] private Slider sliHP;
    [SerializeField] private Text textHP;
    [SerializeField] private GameObject WinPrefab;
    [SerializeField] private GameObject sliderHpPrefab;
    [SerializeField] private BrickListManager brickListManager;

    Text birckHPText;

    public Action ShowWinText;
    public Action ShowLoseText;

    [SerializeField] TextMeshProUGUI winText = null;
    [SerializeField] TextMeshProUGUI loseText= null;

    public TextMeshProUGUI WinTextProperty { get { return winText; } }
    public TextMeshProUGUI LoseTextProperty { get { return loseText; } }
    private void Awake()
    {
        sliHP = FindObjectOfType<Slider>();
        winText = GameObject.FindGameObjectWithTag("Win").GetComponent<TextMeshProUGUI>();
        loseText = GameObject.FindGameObjectWithTag("Lose").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        CallSetSliderValue(brickListManager.FullHP);
        Invoke("SetActiveFalseText", 0.5f);

        ShowWinText = delegate
        {
            WinText();
            GameManager.Instance.SetGameOver(true);
        };
        ShowLoseText = delegate
        {
            LoseText();
            GameManager.Instance.SetGameOver(true);
        };

    }
    public void CallSetSliderValue(float fullHP)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetSliderValue", RpcTarget.All, fullHP);
        }
    }

    [PunRPC]
    public void SetSliderValue(float fullHP)
    {
        sliHP.maxValue = fullHP;
        sliHP.value = fullHP;
    }

    void SetActiveFalseText()
    {
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
    }
    public void SildbarSeting()
    {
        GameObject sliderHp = Instantiate(sliderHpPrefab, transform.position, Quaternion.identity, GameObject.Find("Canvas/HPSliders").transform);
        sliderHp.transform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x-2.45f, -5.1f));
    }
    public void CallUpdateHpText(Transform obj, float curHP)
    {
        birckHPText = obj.GetComponent<Text>();
        photonView.RPC("UpdateHPText", RpcTarget.All, curHP);
    }

    [PunRPC]
    // Brick의 위치에 curHP를 표시
    public void UpdateHPText(float curHP)
    {
        birckHPText.text = curHP.ToString();
    }
    // slider의 값이 변할때마다 호출

    public void CallUpdateHPSlider(float damage)
    {
        if (photonView.IsMine)
            photonView.RPC("UpdateHPSlider", RpcTarget.All, damage);
    }
    
    [PunRPC]
    public void UpdateHPSlider(float damage) => sliHP.value -= damage;

    // 게임오버시 텍스트를 출력 시키는 함수
    public void WinText()
    {
        Image Panel = GameObject.FindGameObjectWithTag("GameOverPanel").GetComponent<Image>();
        Panel.gameObject.SetActive(true);
        TextMeshProUGUI text = Panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        Panel.GetComponent<CanvasGroup>().alpha = 1f;
    }
    public void LoseText()
    {
        Image Panel = GameObject.FindGameObjectWithTag("GameOverPanel").GetComponent<Image>();
        Panel.gameObject.SetActive(true);
        TextMeshProUGUI text = Panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        Panel.GetComponent<CanvasGroup>().alpha = 1f;
    }
}
