using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider sliHP;
    [SerializeField] private Text textHP;
    [SerializeField] private GameObject gameOverPrefab;
    [SerializeField] private GameObject sliderHpPrefab;
    [SerializeField] private BrickListManager brickListManager;
    
    private void Awake()
    {
        sliHP = FindObjectOfType<Slider>();
    }
    // 최대체력 업데이트
    private void Start()
    {
        sliHP.maxValue = brickListManager.FullHP;
        UpdateHPSlider(brickListManager.FullCurHP);
    }

    public void SildbarSeting()
    {
        GameObject sliderHp = Instantiate(sliderHpPrefab, transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
        sliderHp.transform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x, -5.1f));
    }

    // Brick의 위치에 curHP를 표시
    public void UpdateHPText(RectTransform obj, float curHP)
    {
        obj.GetComponent<Text>().text = curHP.ToString();
    }
    // slider의 값이 변할때마다 호출
    public void UpdateHPSlider(float curHP)
    {
        sliHP.value = curHP;
    }
    // 게임오버시 텍스트를 출력 시키는 함수
    public void GameOverText()
    {
        GameObject gameOver = Instantiate(gameOverPrefab, transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
        gameOver.transform.position = Camera.main.WorldToScreenPoint(new Vector2(0, 0));
    }
}
