using System.Collections.Generic;
using UnityEngine;

public class BrickPool : SingleTon<BrickPool>
{
    [SerializeField] Brick[] BrickType = null;
    [SerializeField] Brick playerBrick = null;
    private Queue<Brick> brickQueue = new Queue<Brick>();

    private void Awake()
    {
        BrickType = new Brick[3];
    }
    // 블럭 생성
    private Brick Create(Vector2 pos)
    {
        int ranNum = Random.Range(0, 3);
        playerBrick = BrickType[ranNum];
        Brick brick = null;
        brick = Instantiate(playerBrick, pos, Quaternion.identity, transform);
        return brick;
    }

    public Brick GetBrick(Vector2 pos)
    {
        Brick brick = null;
        if (brickQueue.Count == 0)
        {
            brick = Create(pos);
        }
        else brick = brickQueue.Dequeue();
        brick.gameObject.SetActive(true);
        return brick;
    }

    public void Release(Brick usebrick)
    {
        usebrick.gameObject.SetActive(false);
        brickQueue.Enqueue(usebrick);
    }

}
