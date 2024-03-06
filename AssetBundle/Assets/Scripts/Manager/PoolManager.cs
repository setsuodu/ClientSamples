using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Get;

    // key:object_name, value:object_pool
    private Dictionary<string, ObjectPool<GameObject>> pools;
    private Dictionary<string, Stack<GameObject>> recycle;
    private string _prefabName;

    void Awake()
    {
        Get = this;

        pools = new Dictionary<string, ObjectPool<GameObject>>();
        recycle = new Dictionary<string, Stack<GameObject>>();
        //PreLoad("PlayerA");
    }

    void Update()
    {
        // ����UI�ı�����

        if (Input.GetKeyDown(KeyCode.D))
        {
            //var obj = LoadWithPool("PlayerA");
            //obj.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            //UnloadWithPool("PlayerA");
        }
    }


    // ����ʱԤ����
    public void PreLoad(string prefabName)
    {
        _prefabName = prefabName;

        ObjectPool<GameObject> _pool = null;
        Stack<GameObject> _temp = null;
        if (pools.TryGetValue(prefabName, out _pool) == false)
        {
            // ��δ���������
            _pool = new ObjectPool<GameObject>(_createFunc, _actionOnGet, _actionOnRelease, _actionOnDestroy, true, 10, 1000);
            _temp = new Stack<GameObject>();
            Debug.Log($"����{prefabName}�Ķ����");

            pools.Add(prefabName, _pool);
            recycle.Add(prefabName, _temp);
        }
    }
    public GameObject LoadWithPool(string prefabName)
    {
        _prefabName = prefabName;

        ObjectPool<GameObject> _pool = null;
        Stack<GameObject> _temp = null;
        if (pools.TryGetValue(prefabName, out _pool) == false)
        {
            Debug.LogError($"{prefabName}������û�ж����");
            return null;
        }
        if (recycle.TryGetValue(prefabName, out _temp) == false)
        {
            Debug.LogError("������û�л��ճ�");
            return null;
        }
        var m = _pool.Get();
        _temp.Push(m);

        return m;
    }
    public void UnloadWithPool(string prefabName)
    {
        _prefabName = prefabName;

        ObjectPool<GameObject> _pool = null;
        Stack<GameObject> _temp = null;
        if (pools.TryGetValue(prefabName, out _pool) == false)
        {
            Debug.LogError($"δ����{prefabName}�Ķ����");
            return;
        }
        if (recycle.TryGetValue(prefabName, out _temp) == false)
        {
            Debug.LogError($"δ����{prefabName}�Ļ��ճ�");
            return;
        }


        if (_temp.Count > 0)
        {
            var m = _temp.Pop();
            _pool.Release(m);
        }
    }

    GameObject _createFunc()
    {
        //Debug.Log($"����_createFunc()");
        GameObject prefab = ABManager.LoadPrefab($"Prefabs/{_prefabName}");
        var obj = Instantiate(prefab, transform);
        return obj;
    }
    void _actionOnGet(GameObject obj)
    {
        obj.SetActive(true);
        obj.name = $"{_prefabName}_{pools[_prefabName].CountActive}";
        obj.transform.position = Vector3.zero;
        obj.transform.parent = null;
    }
    void _actionOnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }
    void _actionOnDestroy(GameObject obj)
    {
        Destroy(obj);
    }
}