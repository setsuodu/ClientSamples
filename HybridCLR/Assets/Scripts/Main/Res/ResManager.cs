using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>AOT：资源门面，热更程序集可引用 Main 后调用。</summary>
public static class ResManager
{
    static bool _inited;
    public static bool IsReady => _inited && ABManager.Instance != null;

    public static async UniTask InitializeAsync(CancellationToken ct = default)
    {
        if (_inited) return;
        await ABManager.Instance.InitializeAsync(ct);
        _inited = true;
        Debug.Log($"[Res] ready version={ABManager.Instance.GetVersion()}");
    }

    public static void ReleaseAll()
    {
        ABManager.Instance?.UnloadAll(true);
        _inited = false;
    }

    public static UniTask<T> LoadAsync<T>(string bundle, string asset, CancellationToken ct = default)
        where T : Object
    {
        if (!_inited || ABManager.Instance == null)
            throw new System.InvalidOperationException("[Res] 先调用 await ResManager.InitializeAsync()");
        return ABManager.Instance.LoadAssetAsync<T>(bundle, asset, ct);
    }

    public static void Unload(string bundle) => ABManager.Instance?.UnloadBundle(bundle);
}
