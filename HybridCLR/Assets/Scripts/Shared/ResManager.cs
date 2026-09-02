using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 资源门面。业务只调这里，底层走 ABManager。
/// Launcher 负责 Initialize / ReleaseAll。
/// </summary>
public static class ResManager
{
    static bool _inited;

    public static bool IsReady => _inited && ABManager.Instance != null;

    /// <summary>启动时调一次（Launcher）</summary>
    public static async UniTask InitializeAsync(CancellationToken ct = default)
    {
        if (_inited) return;
        await ABManager.Instance.InitializeAsync(ct);
        _inited = true;
        Debug.Log($"[Res] ready version={ABManager.Instance.GetVersion()}");
    }

    /// <summary>登出 / 回登录时调</summary>
    public static void ReleaseAll()
    {
        ABManager.Instance?.UnloadAll(true);
        _inited = false;
        Debug.Log("[Res] ReleaseAll");
    }

    // ---------- 通用 ----------

    public static UniTask<T> LoadAsync<T>(string bundle, string asset, CancellationToken ct = default)
        where T : Object
    {
        EnsureReady();
        return ABManager.Instance.LoadAssetAsync<T>(bundle, asset, ct);
    }

    public static void Unload(string bundle) => ABManager.Instance?.UnloadBundle(bundle);

    public static int GetRefCount(string bundle) => ABManager.Instance?.GetRefCount(bundle) ?? 0;

    public static bool IsLoaded(string bundle) => ABManager.Instance != null && ABManager.Instance.IsLoaded(bundle);

    // ---------- 按类型方便调用（key 约定：bundle = 路径小写，asset = 文件名）----------

    public static UniTask<GameObject> LoadUIAsync(string name, CancellationToken ct = default)
        => LoadAsync<GameObject>($"ui/{name.ToLowerInvariant()}", name, ct);

    public static UniTask<GameObject> LoadModelAsync(string name, CancellationToken ct = default)
        => LoadAsync<GameObject>($"characters/{name.ToLowerInvariant()}", name, ct);

    public static UniTask<GameObject> LoadPropAsync(string name, CancellationToken ct = default)
        => LoadAsync<GameObject>($"props/{name.ToLowerInvariant()}", name, ct);

    public static UniTask<AudioClip> LoadAudioAsync(string bundle, string asset, CancellationToken ct = default)
        => LoadAsync<AudioClip>(bundle, asset, ct);

    public static UniTask<Sprite> LoadSpriteAsync(string bundle, string asset, CancellationToken ct = default)
        => LoadAsync<Sprite>(bundle, asset, ct);

    public static UniTask<Texture2D> LoadTextureAsync(string bundle, string asset, CancellationToken ct = default)
        => LoadAsync<Texture2D>(bundle, asset, ct);

    static void EnsureReady()
    {
        if (!_inited || ABManager.Instance == null)
            throw new System.InvalidOperationException("[Res] 先调用 await ResManager.InitializeAsync()");
    }
}
