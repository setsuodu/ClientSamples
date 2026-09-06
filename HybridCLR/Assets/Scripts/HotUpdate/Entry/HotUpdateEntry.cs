using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 热更入口。由 AOT HotUpdateLoader 反射调用 Run()。
/// </summary>
public static class HotUpdateEntry
{
    public static void Run()
    {
        Debug.Log("[HotUpdate] HotUpdateEntry.Run");
        EnsureUIManager();
        UIManager.Instance.OpenAsync("UI_Login").Forget();
    }

    static void EnsureUIManager()
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.mainCanvas == null)
                UIManager.Instance.mainCanvas = Object.FindObjectOfType<Canvas>();
            return;
        }

        var host = ABManager.Instance != null
            ? ABManager.Instance.gameObject
            : new GameObject("UIManager");

        var ui = host.GetComponent<UIManager>() ?? host.AddComponent<UIManager>();
        ui.mainCanvas = Object.FindObjectOfType<Canvas>();
        if (ABManager.Instance == null)
            Object.DontDestroyOnLoad(host);
    }
}
