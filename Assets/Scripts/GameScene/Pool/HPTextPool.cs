using System.Collections.Generic;
using UnityEngine;

public class HPTextPool : BaseObjectPool<HPTextPool, GameObject>
{
    [SerializeField] GameObject HPPrefab = null;
    [SerializeField] GameObject HPTexts = null;

    protected override GameObject getPrefab()
    {
        return HPPrefab;
    }

    public override GameObject Get(Vector3 position)
    {
        GameObject obj = base.Get(position);
        obj.gameObject.SetActive(true);
        obj.transform.SetParent(HPTexts.transform);
        return obj;
    }

    public override void Release(GameObject obj)
    {
        base.Release(obj);
    }
}
