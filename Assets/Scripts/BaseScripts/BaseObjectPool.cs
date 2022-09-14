using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectPool<T1, T2> : SingleTon<BaseObjectPool<T1, GameObject>>
{
    Queue<GameObject> pool = new Queue<GameObject>();

    // 상속받는 클래스에서 프리펩을 할당한다.
    protected virtual GameObject getPrefab()
    {
        return null;
    }

    // 프리펩을 생성한다.
    protected virtual GameObject Create()
    {
        GameObject obj = Instantiate(getPrefab());
        return obj;
    }

    // 사용자측에서 프리펩을 pool에서 가져온다.
    public virtual GameObject Get(Vector3 position)
    {
        GameObject obj = pool.Count == 0 ? Create() : pool.Dequeue();
        obj.SetActive(true);
        obj.transform.SetParent(null);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        return obj;
    }

    // 풀로 회수한다.
    public virtual void Release(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}