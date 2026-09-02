using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 极简 UI 管理：Open / Close / CloseAll。
/// 资源全部走 ResManager，不再直接碰 ABManager。
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("不填则自动找 MainCanvas 或场景第一个 Canvas")]
    public Canvas mainCanvas;

    readonly Dictionary<string, GameObject> _opened = new();
    readonly Dictionary<string, string> _bundleOf = new(); // key → bundle，关闭时减引用

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (mainCanvas == null) mainCanvas = FindMainCanvas();
    }

    /// <summary>
    /// 打开界面。name 同时作为 bundle 后缀和资源名，例如 "UI_Home" → bundle=ui/ui_home, asset=UI_Home
    /// </summary>
    public async UniTask<GameObject> OpenAsync(string name, CancellationToken ct = default)
    {
        string key = name.ToLowerInvariant();
        if (_opened.TryGetValue(key, out var exist) && exist != null)
        {
            exist.SetActive(true);
            return exist;
        }

        if (mainCanvas == null)
        {
            mainCanvas = FindMainCanvas();
            if (mainCanvas == null) return null;
        }

        string bundle = $"ui/{key}";
        var prefab = await ResManager.LoadAsync<GameObject>(bundle, name, ct);
        if (prefab == null)
        {
            Debug.LogError($"[UI] 加载失败: {bundle}/{name}");
            return null;
        }

        var go = Instantiate(prefab, mainCanvas.transform, false);
        go.name = name;
        _opened[key] = go;
        _bundleOf[key] = bundle;
        return go;
    }

    /// <summary>兼容旧调用：bundle + asset</summary>
    public UniTask<GameObject> OpenAsync(string bundle, string asset, CancellationToken ct = default)
        => OpenAsync(asset, ct);

    public void Close(string name)
    {
        string key = name.ToLowerInvariant();
        if (_opened.TryGetValue(key, out var go) && go != null)
            Destroy(go);
        _opened.Remove(key);

        if (_bundleOf.TryGetValue(key, out var bundle))
        {
            ResManager.Unload(bundle);
            _bundleOf.Remove(key);
        }
    }

    public void Close(string bundle, string asset) => Close(asset);

    public void CloseAll()
    {
        foreach (var go in _opened.Values)
            if (go != null) Destroy(go);
        _opened.Clear();

        foreach (var bundle in _bundleOf.Values)
            ResManager.Unload(bundle);
        _bundleOf.Clear();
    }

    Canvas FindMainCanvas()
    {
        var go = GameObject.Find("MainCanvas");
        if (go != null)
        {
            var c = go.GetComponent<Canvas>();
            if (c != null) return c;
        }
        var any = FindObjectOfType<Canvas>();
        if (any != null)
        {
            Debug.LogWarning($"[UI] 未找到 MainCanvas，使用: {any.name}");
            return any;
        }
        Debug.LogError("[UI] 场景中没有 Canvas");
        return null;
    }
}
