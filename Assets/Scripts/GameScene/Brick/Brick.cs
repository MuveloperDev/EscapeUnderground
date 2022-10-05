using UnityEngine;
using Photon.Pun;

public class Brick : MonoBehaviourPun
{
    [SerializeField] private float curHP = 0;  // 벽돌의 현재 체력
    [SerializeField] private float maxHP = 10;  // 설정된 벽돌의 최대 체력
    [SerializeField] private BrickListManager brickListManager;
    [SerializeField] private UIManager uiManager;

    public float MaxHP { get { return maxHP; } }
    public float CurHP { get { return curHP; } }
    Transform textHP;  // HP Text의 위치 -> 2D좌표 이기 때문에 RectTransform으로 설정

    private void OnEnable()
    {
        brickListManager = FindObjectOfType<BrickListManager>();
        uiManager = FindObjectOfType<UIManager>();
        curHP = maxHP;  // 시작시 현재 체력값을 벽돌의 최대 체력으로 설정
        brickListManager.AddBrick(this);
    }

    public void CallReceveDamage(float damage)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ReceiveDamage", RpcTarget.All, damage);
        }
    }

    // 데미지를 받았을 때 실행되는 함수
    // 체력 1감소 체력이 0이 되었을 때 해당 오브젝트 꺼줌
    [PunRPC]
    public void ReceiveDamage(float Damage)
    {
        curHP -= Damage;
        if (curHP <= 0)
        {
            curHP = 0;
            photonView.RPC("RelaseBrick", RpcTarget.All);
        }
    }

    // Brick 회수 함수
    [PunRPC]
    void RelaseBrick()
    {
        brickListManager.listBrick.Remove(this);
        BrickPool.Instance.Release(this.gameObject);
    }
}
