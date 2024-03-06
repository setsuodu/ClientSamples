using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class ILGlobal : MonoBehaviour
{
    public static ILGlobal Get;

    void Awake()
    {
        Get = this;
    }

    public void GlobalInit()
    {
        StartCoroutine(LoadHotFixAssembly());
    }

    // 从ab包加载dll
    IEnumerator LoadHotFixAssembly()
    {
        yield return null;

        //byte[] dll = ABManager.LoadDLL();
        //var fs = new MemoryStream(dll);
        //appdomain.LoadAssembly(fs, null, null);

        //InitializeILRuntime();
        OnHotFixLoaded();
    }

    void OnHotFixLoaded()
    {
        // IL热更加载UI
        //appdomain.Invoke("HotFix.Main", "Init", gameObject, null); //static方法
        HotFix.Main.Init();

        StartCoroutine(StartPreload());
    }

    Slider progressBar;

    AsyncOperationHandle downloadDependencies;

    IEnumerator StartPreload()
    {
        AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
        yield return handle;
        Debug.Log("InitializeAsync");

        // Clear all cached Assetbundle
        Caching.ClearCache();

        string key = "HD"; //下载所有HD标签的资源包
        AsyncOperationHandle<long> getdownloadSize = Addressables.GetDownloadSizeAsync(key);
        yield return getdownloadSize;

        if (getdownloadSize.Result > 0)
        {
            downloadDependencies = Addressables.DownloadDependenciesAsync(key);
            //progressBar.gameObject.SetActive(true);
            yield return downloadDependencies;
        }
    }

    void Update()
    {
        if (downloadDependencies.IsValid())
        {
            if (downloadDependencies.GetDownloadStatus().Percent < 1)
            {
                //UpdateProgressBar(downloadDependencies.GetDownloadStatus().Percent);
            }
            else
            {
                Addressables.Release(downloadDependencies);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadPrefab("Assets/Bundles/Prefabs/Aoi.prefab", "HD");
        }
    }

    private void LoadPrefab(string key, string label)
    {
        Addressables.LoadAssetAsync<GameObject>(key).Completed += Prefab_Completed;
    }

    private void Prefab_Completed(AsyncOperationHandle<GameObject> obj)
    {
        var go = Instantiate(obj.Result);
    }

    private void LoadTexture(string key, string label)
    {
        Addressables.LoadAssetsAsync<Texture2D>(new List<object> { key, label }, null, Addressables.MergeMode.Intersection).Completed += Texture_Completed;
    }

    private void Texture_Completed(AsyncOperationHandle<IList<Texture2D>> handle)
    {
        //obj.material.mainTexture = handle.Results[0];
    }
}