using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private ObjectPool<GameObject> pool;

    public GameObject prefab;
    public Stack<GameObject> temp;

    void Awake()
    {
        pool = new ObjectPool<GameObject>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 10, 1000);
        temp = new Stack<GameObject>();
    }

    GameObject createFunc()
    {
        var obj = Instantiate(prefab, transform);
        return obj;
    }

    void actionOnGet(GameObject obj)
    {
        obj.SetActive(true);
        obj.name = pool.CountActive + "";
        obj.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    void actionOnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }

    void actionOnDestroy(GameObject obj)
    {
        Destroy(obj);
    }

    private void Spawn()
    {
        var m = pool.Get();
        temp.Push(m);
    }

    private void Despawn(GameObject elem)
    {
        pool.Release(elem);
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM

#else
        if (Input.GetKeyDown(KeyCode.D))
        {
            Spawn();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (temp.Count > 0)
            {
                var m = temp.Pop();
                Despawn(m);
            }
        }
#endif
    }
}