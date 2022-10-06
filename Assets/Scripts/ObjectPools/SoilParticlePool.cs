using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilParticlePool : BaseObjectPool<SoilParticlePool, GameObject>
{
    [SerializeField] private GameObject prefabs;

    private void Awake()
    {
        prefabs = Resources.Load<GameObject>("Prefabs/Particles/SoilBrokenParticle");
    }
    protected override GameObject getPrefab()
    {
        return prefabs;
    }

    public override GameObject Get(Vector3 position)
    {
        return base.Get(position);
    }

    public override void Release(GameObject obj)
    {
        obj.transform.SetParent(transform);
        base.Release(obj);
    }
}
