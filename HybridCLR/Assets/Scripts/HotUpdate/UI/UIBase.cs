using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UI 面板基类。脚本预挂在 Prefab 上，控件用 [SerializeField] 在 Inspector 拖引用。
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    public string PanelName { get; private set; }
    public bool IsClosed { get; internal set; }

    internal void __Init(string panelName)
    {
        PanelName = panelName;
        IsClosed = false;
        OnOpen();
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    public void Close()
    {
        if (IsClosed) return;
        IsClosed = true;
        OnClose();
        if (UIManager.Instance != null)
            UIManager.Instance.Close(PanelName);
    }

    protected UniTask<GameObject> OpenPanelAsync(string name, CancellationToken ct = default)
        => UIManager.Instance.OpenAsync(name, ct);
}
