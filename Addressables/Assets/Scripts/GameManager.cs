using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    public Button btn_Create;
    public Button btn_Release;

    void Awake()
    {
        btn_Create?.onClick.AddListener(Load);
        btn_Release?.onClick.AddListener(Unload);
    }

    public string address = "Assets/Bundles/Prefabs/Player.prefab";

    AsyncOperationHandle<GameObject> loadHandle; //对象加载过程内存

    IEnumerator LoadCoroutine(string address)
    {
        // 模拟 AssetDatabase.LoadAssetAtPath 从Editor加载
        loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
        loadHandle.Completed += LoadHandle_Completed;
        loadHandle.Destroyed += LoadHandle_Destroyed;
        yield return loadHandle;
        Instantiate(loadHandle.Result);
    }

    // 加载回调
    void LoadHandle_Completed(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log($"Created : {obj.Status} 已加载");
    }

    // 释放回调
    void LoadHandle_Destroyed(AsyncOperationHandle obj)
    {
        Debug.Log($"Released : {obj.Status} 已释放");
    }

    public void Load()
    {
        StartCoroutine(LoadCoroutine(address));
    }

    // Material, Mesh Filter都会被卸载。
    // 但是 默认几何Mesh（Cube, Capsule,,,） 不会卸载。
    public void Unload()
    {
        Debug.Log($"[0] {loadHandle.Status}");
        Addressables.Release(loadHandle);
        //Debug.Log($"[1] {loadHandle.Status}"); //已为空
    }
}
