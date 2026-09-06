using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Prefab 上已预挂 UIBase 子类；实例化后 GetComponent，不再反射 AddComponent。
/// OpenAsync("UI_Login") → bundle=ui/ui_login，asset=UI_Login
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("不填则自动找 MainCanvas 或场景第一个 Canvas")]
    public Canvas mainCanvas;

    readonly Dictionary<string, UIBase> _opened = new();
    readonly Dictionary<string, string> _bundleOf = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (mainCanvas == null)
            mainCanvas = FindMainCanvas();
    }

    public async UniTask<GameObject> OpenAsync(string name, CancellationToken ct = default)
    {
        string key = name.ToLowerInvariant();

        if (_opened.TryGetValue(key, out var exist) && exist != null && !exist.IsClosed)
        {
            exist.gameObject.SetActive(true);
            return exist.gameObject;
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

        // 校验：Prefab 上应已挂 UIBase
        if (prefab.GetComponent<UIBase>() == null)
        {
            Debug.LogError($"[UI] Prefab 未挂 UIBase 子类，请在 {name}.prefab 根节点挂好脚本: {name}");
            return null;
        }

        var go = Instantiate(prefab, mainCanvas.transform, false);
        go.name = name;

        var ui = go.GetComponent<UIBase>();
        if (ui == null)
        {
            Debug.LogError($"[UI] 实例上没有 UIBase: {name}");
            Destroy(go);
            return null;
        }

        _opened[key] = ui;
        _bundleOf[key] = bundle;
        ui.__Init(name);
        return go;
    }

    public UniTask<GameObject> OpenAsync(string bundle, string asset, CancellationToken ct = default)
        => OpenAsync(asset, ct);

    public void Close(string name)
    {
        string key = name.ToLowerInvariant();
        if (_opened.TryGetValue(key, out var ui) && ui != null)
        {
            if (!ui.IsClosed)
            {
                ui.IsClosed = true;
                // OnClose：若经 UIBase.Close 已调过则再调一次无妨，这里仅 Destroy 路径补一次
            }
            Destroy(ui.gameObject);
        }
        _opened.Remove(key);

        if (_bundleOf.TryGetValue(key, out var bundle))
        {
            ResManager.Unload(bundle);
            _bundleOf.Remove(key);
        }
    }

    public void CloseAll()
    {
        foreach (var ui in _opened.Values)
            if (ui != null) Destroy(ui.gameObject);
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
