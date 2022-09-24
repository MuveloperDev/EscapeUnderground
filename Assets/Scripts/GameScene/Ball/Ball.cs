using UnityEngine;
using Photon.Pun;


public class Ball : MonoBehaviourPun
{
    [Header("RigidBody")]
    [SerializeField] private Rigidbody2D rigid2D = null;                // 플레이어의 rigidbody2D를 담을 변수
    [SerializeField] private Transform arrow = null;                    // 자식개체인 arrow를 담을 변수
    [SerializeField] private Transform[] childrenObject = null;         // GetComponentsInChildren은 배열로 가져오기 때문에 그것을 담아줄 변수
    [SerializeField] private BrickListManager brickListManager;

    [Header("Mouse")]
    [SerializeField] Vector3 mouseDir;                      // mouse의 position값을 저장할 변수

    [Header("Player Seting")]
    [SerializeField] float pushPower;                       // Ball에게 가하는 힘
    [SerializeField] bool move;                             // 이동 여부를 확인하는 bool값
    [SerializeField] float attackPower;                       // Ball의 공격력을 저장할 변수
    [SerializeField] float[] wallCount;
    [SerializeField] int idx;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        childrenObject = GetComponentsInChildren<Transform>();
        if (childrenObject[1] != null) arrow = childrenObject[1];           // GetComponentsInChildren은 부모를 포함한 모든 자식개체를 불러와 배열로 다시한번 저장
    }

    private void Start()
    {
        mouseDir = Vector3.zero;
        pushPower = 500;
        move = false;
        attackPower = 10f;
        wallCount = new float[2];
        idx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        if (!move)
        {
            
            PlayerMove();           // Ball의 움직임
        }
    }

    // Ball의 이동 및 회전
    private void PlayerMove()
    {
        if (!GameManager.Instance.GetGameOver())
        {
            mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);         // window화면에 맞춰져있는 mousePosition값을 ScreenToWorldPoint함수를 이용해 유니티화면으로 맞춰준다mouseDir.z = 0f;
            mouseDir.z = 0f;
            Vector3 posVector = mouseDir - transform.position;                      // Ball의 현재위치와 MousePoint의 방향벡터를 저장
            float angle = Mathf.Atan2(posVector.y, posVector.x) * Mathf.Rad2Deg;    // 방향벡터를 사용해 각도를 구한다

            if (!move)
            {
                if (mouseDir.y > -4.7f)
                {
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);          // 계산된 각도를 이용해 Z축을 회전
                }
                if (Input.GetMouseButtonDown(0))
                {
                    ClickMouse();
                }
            } 
        }
    }

    // Ball 발사
    void ClickMouse()
    {
        move = true;
        photonView.RPC("SetActiveArrow", RpcTarget.All, false);
        //arrow.gameObject.SetActive(false);
        rigid2D.AddRelativeForce(Vector2.right * pushPower, ForceMode2D.Force);         // 오브젝트의 기준으로 이동, ForceMode2D.Force = 일정한 속도로 이동
    }

    // 충돌시 태그에 따른 조건문
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Brick")
        {
            //photonView.RPC("Hit", RpcTarget.All, collision);
            Hit(collision); 
        }
        if (collision.gameObject.tag == "Wall")
        {
            if (idx < 2)
            {
                wallCount[idx] = transform.position.y;
                idx++;
            }
            if (idx >= 1)
            {
                if (wallCount[0] == wallCount[1])
                {
                    rigid2D.AddRelativeForce(Vector2.up * 30);
                }
            }
            else
            {
                idx = 0;
            }
        }
        if (collision.gameObject.tag == "Place")
        {
            this.rigid2D.velocity = Vector2.zero;           // Ball이 지면에 도착하면 정지
            move = false;
            photonView.RPC("SetActiveArrow", RpcTarget.All, true);
            //arrow.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    void SetActiveArrow(bool set)
    { 
        arrow.gameObject.SetActive(set);
    }

    void  Hit(Collision2D target)
    {

        brickListManager.ReceiveDamage(attackPower);
        target.gameObject.GetComponent<Brick>().CallReceveDamage(attackPower);
    }
}
