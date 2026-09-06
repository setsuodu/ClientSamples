using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AOT 启动：Canvas / ABManager → ABConfig → 可选热更资源 → ResManager → 进入热更 DLL 入口。
/// 不直接引用 UI_*（UI 在 HotUpdate 程序集）。
/// </summary>
public sealed class GameBootstrap : MonoBehaviour
{
    [SerializeField] ABUpdater updater;
    [SerializeField] Canvas canvas;
    [Tooltip("没有则自动 AddComponent")]
    [SerializeField] bool autoAddUpdater = true;

    CancellationTokenSource cts;

    async void Start()
    {
        cts = new CancellationTokenSource();
        EnsureCanvas();
        EnsureManagers();

        var loading = SimpleUI.CreatePanel(canvas.transform, "启动中", "正在初始化资源…");
        try
        {
            await ABConfig.LoadAsync(cts.Token);

            if (updater == null)
                updater = FindObjectOfType<ABUpdater>();
            if (updater == null && autoAddUpdater)
                updater = gameObject.AddComponent<ABUpdater>();

            if (updater != null && ABConfig.EnableHotUpdate)
            {
                loading.SetMessage("正在检查资源更新…");
                var progress = new Progress<(float progress, string tip)>(p =>
                    loading.SetMessage($"{p.tip}\n{p.progress:P0}"));
                await updater.CheckAndUpdateAsync(progress, cts.Token);
            }

            loading.SetMessage("正在初始化 AssetBundle…");
            await ResManager.InitializeAsync(cts.Token);

            loading.SetMessage("正在进入热更逻辑…");
            Destroy(loading.gameObject);

            // UI / 业务在 HotUpdate 程序集
            await HotUpdateLoader.LoadAndRunAsync(cts.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            if (loading != null)
                loading.SetMessage("启动失败\n" + e.Message);
            Debug.LogException(e);
        }
    }

    void EnsureCanvas()
    {
        if (canvas != null) return;
        var go = new GameObject("MainCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        DontDestroyOnLoad(go);
        var es = new GameObject("EventSystem",
            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.EventSystems.StandaloneInputModule));
        DontDestroyOnLoad(es);
    }

    void EnsureManagers()
    {
        if (ABManager.Instance == null)
        {
            var go = new GameObject("ABManager");
            go.AddComponent<ABManager>();
            DontDestroyOnLoad(go);
        }
        // UIManager 在热更程序集，由 HotUpdateEntry 负责挂接
    }

    void OnDestroy() => cts?.Cancel();
}

internal static class SimpleUI
{
    public static Text Label(Transform parent, string text, int size = 28)
    {
        var go = new GameObject("Label", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = size;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = t.rectTransform;
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.9f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return t;
    }

    public static LoadingPanel CreatePanel(Transform parent, string title, string message)
    {
        var go = new GameObject(title, typeof(RectTransform), typeof(Image), typeof(LoadingPanel));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.96f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var p = go.GetComponent<LoadingPanel>();
        p.label = Label(go.transform, message, 32);
        return p;
    }
}

public sealed class LoadingPanel : MonoBehaviour
{
    public Text label;
    public void SetMessage(string s) { if (label) label.text = s; }
}
