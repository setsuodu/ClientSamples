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

    AsyncOperationHandle<GameObject> loadHandle; //������ع����ڴ�

    IEnumerator LoadCoroutine(string address)
    {
        // ģ�� AssetDatabase.LoadAssetAtPath ��Editor����
        loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
        loadHandle.Completed += LoadHandle_Completed;
        loadHandle.Destroyed += LoadHandle_Destroyed;
        yield return loadHandle;
        Instantiate(loadHandle.Result);
    }

    // ���ػص�
    void LoadHandle_Completed(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log($"Created : {obj.Status} �Ѽ���");
    }

    // �ͷŻص�
    void LoadHandle_Destroyed(AsyncOperationHandle obj)
    {
        Debug.Log($"Released : {obj.Status} ���ͷ�");
    }

    public void Load()
    {
        StartCoroutine(LoadCoroutine(address));
    }

    // Material, Mesh Filter���ᱻж�ء�
    // ���� Ĭ�ϼ���Mesh��Cube, Capsule,,,�� ����ж�ء�
    public void Unload()
    {
        Debug.Log($"[0] {loadHandle.Status}");
        Addressables.Release(loadHandle);
        //Debug.Log($"[1] {loadHandle.Status}"); //��Ϊ��
    }
}
