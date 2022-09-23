using System.Collections.Generic;
using UnityEngine;

// brick의 HP를 전체 합산한 함수
public delegate void HpManager();
public class BrickListManager : MonoBehaviour
{
    
	List<Brick> listBrick = new List<Brick>();
	[SerializeField] private float fullHP;
	[SerializeField] private float fullCurHP;
    [SerializeField] private UIManager uiManager;
    
    // 최대 체력과 현재 체력을 넘겨줄 프로퍼티
    public float FullHP { get { return fullHP; } }
    public float FullCurHP { get { return fullCurHP; } }

    public HpManager hpManager;
    private void Awake()
    {
        hpManager = MaxHP;
        uiManager.SildbarSeting();
    }
    private void Start()
    {
        fullCurHP = fullHP;
    }
    // 전체 체력이 0이 되면 게임오버 호출
    private void Update()
    {
        if (fullCurHP <= 0)
        {
            uiManager.GameOverText();
            GameManager.Instance.SetGameOver(true);
        }
    }
    // 생성된 Brick을 List에 담아준다
    public void AddBrick(Brick brick)
	{
		listBrick.Add(brick);
	}
    // List에 담겨진 Brick의 HP 총합계를 구한다
    public void MaxHP()
    {
        foreach (Brick brick in listBrick)
        {
            fullHP += brick.MaxHP;
        }
    }
    // 충돌시 Slider로 전달
    void ReceiveDamage(float Damage)
    {
        fullCurHP -= Damage;
        uiManager.UpdateHPSlider(fullCurHP);   // 충돌할 때 마다 슬라이더 값 변경
    }
}
