using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RemoteManager : MonoBehaviour
{
    public Button btn_Download;
    public Button btn_Create;
    public Button btn_Destroy;

    AsyncOperationHandle downloadDependencies;

    void Awake()
    {
        btn_Download.onClick.AddListener(Preload);
        btn_Create.onClick.AddListener(LoadCharacter);
        btn_Destroy.onClick.AddListener(Unload);
    }

    void Update()
    {
        // 下载是否有效
        if (downloadDependencies.IsValid())
        {
            if (downloadDependencies.GetDownloadStatus().Percent < 1)
            {
                Debug.Log($"下载中: {downloadDependencies.GetDownloadStatus().Percent}");
            }
            else if (downloadDependencies.IsDone)
            {
                Debug.Log("下载完成");
                Addressables.Release(downloadDependencies);
            }
        }
    }

    public void Preload()
    {
        StartCoroutine(StartPreload());
    }

    public IEnumerator StartPreload()
    {
        AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
        yield return handle;
        Debug.Log("Initialized");

        // 清除缓存
        Caching.ClearCache();

        // 计算下载文件大小
        string key = "HD";
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
        yield return getDownloadSize;

        if (getDownloadSize.Result > 0)
        {
            downloadDependencies = Addressables.DownloadDependenciesAsync(key);
            
            // 显示进度条
            
            yield return downloadDependencies;
            Debug.Log("Done");
        }
    }

    public string player = "Assets/Bundles/Prefabs/Player.prefab";

    AsyncOperationHandle<GameObject> loadHandle; //对象加载过程内存

    public void LoadCharacter()
    {
        StartCoroutine(LoadCoroutine(player));
    }

    IEnumerator LoadCoroutine(string address)
    {
        loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
        yield return loadHandle;
        Instantiate(loadHandle.Result);
    }

    public void Unload()
    {
        if (loadHandle.IsValid())
        {
            Debug.Log($"[0] {loadHandle.Status}");
            Addressables.Release(loadHandle);
        }
        else
        {
            Debug.Log("已经不存在");
        }
        //Debug.Log($"[1] {loadHandle.Status}"); //已为空
    }
}
