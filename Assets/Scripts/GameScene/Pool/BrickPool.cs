using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BrickPool : BaseObjectPool<BrickPool, GameObject>
{
    [SerializeField] GameObject[] brickType = null;
    [SerializeField] int idx = 0;

    private void Awake()
    {
        brickType = new GameObject[3];
        brickType[0] = Resources.Load<GameObject>("Prefabs/Brick/Iron Brick");
        brickType[1] = Resources.Load<GameObject>("Prefabs/Brick/Soil Brick");
        brickType[2] = Resources.Load<GameObject>("Prefabs/Brick/Stone Brick");
    }

    protected override GameObject getPrefab()
    {
        idx = Random.RandomRange(0, 3);
        return brickType[idx];
    }
    public override GameObject Get(Vector3 position)
    {
        return base.Get(position);
    }

    public override void Release(GameObject obj)
    {
        base.Release(obj);
    }
}
