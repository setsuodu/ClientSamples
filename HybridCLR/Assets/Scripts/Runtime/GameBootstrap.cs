using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public sealed class GameBootstrap : MonoBehaviour
{
    [Header("远端版本 JSON；为空时使用 StreamingAssets/HotUpdate/version.json")]
    [SerializeField] string remoteVersionUrl = "";
    [SerializeField] Canvas canvas;
    CancellationTokenSource cts;

    async void Start()
    {
        cts = new CancellationTokenSource();
        EnsureCanvas();
        EnsureABManager();
        var loading = SimpleUI.CreatePanel(canvas.transform, "启动中", "正在检查版本…");
        try
        {
            var version = await VersionChecker.CheckAsync(remoteVersionUrl, cts.Token);
            loading.SetMessage($"版本 {version.Version}\n正在下载资源…");
            // AssetBundleFramework 负责读取版本清单、比较本地缓存并下载缺失/更新的 AB。
            await ResManager.InitializeAsync(cts.Token);
            Destroy(loading.gameObject);
            await LoginPage.ShowAsync(canvas, cts.Token);
        }
        catch (Exception e)
        {
            loading.SetMessage("启动失败\n" + e.Message + "\n请检查网络后重试");
            Debug.LogException(e);
        }
    }

    void EnsureCanvas()
    {
        if (canvas != null) return;
        var go = new GameObject("MainCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);
        DontDestroyOnLoad(go);
        var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        DontDestroyOnLoad(es);
    }

    void EnsureABManager()
    {
        var go = new GameObject("ABManager", typeof(ABManager), typeof(UIManager));
        DontDestroyOnLoad(go);
    }

    void OnDestroy() => cts?.Cancel();
}

[Serializable] public struct RemoteVersion { public string version; public string cdn; public string[] bundles; public string Version => string.IsNullOrEmpty(version) ? "1.0.0" : version; }

public static class VersionChecker
{
    public static async UniTask<RemoteVersion> CheckAsync(string url, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            var local = Resources.Load<TextAsset>("HotUpdate/version");
            return local == null ? new RemoteVersion { version = Application.version } : JsonUtility.FromJson<RemoteVersion>(local.text);
        }
        using var req = UnityWebRequest.Get(url);
        await req.SendWebRequest().WithCancellation(ct);
        if (req.result != UnityWebRequest.Result.Success) throw new Exception("版本检查失败：" + req.error);
        return JsonUtility.FromJson<RemoteVersion>(req.downloadHandler.text);
    }
}

internal static class SimpleUI
{
    public static Text Label(Transform parent, string text, int size = 28)
    {
        var go = new GameObject("Label", typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>(); t.text = text; t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.fontSize = size; t.color = Color.white; t.alignment = TextAnchor.MiddleCenter; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = t.rectTransform; rt.anchorMin = new Vector2(.1f,.1f); rt.anchorMax = new Vector2(.9f,.9f); rt.offsetMin=rt.offsetMax=Vector2.zero; return t;
    }
    public static LoadingPanel CreatePanel(Transform parent, string title, string message)
    { var go = new GameObject(title, typeof(RectTransform), typeof(Image), typeof(LoadingPanel)); go.transform.SetParent(parent,false); var img=go.GetComponent<Image>(); img.color=new Color(.05f,.08f,.12f,.96f); var rt=go.GetComponent<RectTransform>(); rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero; var p=go.GetComponent<LoadingPanel>();p.label=Label(go.transform,message,32);return p; }
}
public sealed class LoadingPanel : MonoBehaviour { public Text label; public void SetMessage(string s){if(label)label.text=s;} }
