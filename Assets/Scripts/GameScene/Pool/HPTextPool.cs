using UnityEngine;

public class HPTextPool : BaseObjectPool<HPTextPool,GameObject>
{
    [SerializeField] GameObject prefab = null;

    private void Awake()
    {
        prefab = Resources.Load<GameObject>("Prefabs/UI/HP Text");
    }
    
    protected override GameObject getPrefab()
    {
        return prefab;
    }

    public override GameObject Get(Vector3 position)
    {
        GameObject obj = base.Get(position);
        obj.transform.SetParent(GameObject.Find("Canvas").transform);
        return obj;
    }

    public override void Release(GameObject obj)
    {
        base.Release(obj);
    }
}
