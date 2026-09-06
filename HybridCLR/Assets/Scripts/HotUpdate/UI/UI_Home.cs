using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主页。挂在 UI_Home.prefab 根节点上，文案 Label 在 Inspector 里拖好。
/// </summary>
public class UI_Home : UIBase
{
    /// <summary>登录页写入的临时账号（演示用）</summary>
    public static string PendingAccount;

    [Header("在 Prefab 上拖引用")]
    [SerializeField] Text welcomeLabel;

    protected override void OnOpen()
    {
        if (welcomeLabel == null)
            welcomeLabel = GetComponentInChildren<Text>(true);

        if (welcomeLabel != null)
        {
            string user = string.IsNullOrEmpty(PendingAccount) ? "玩家" : PendingAccount;
            welcomeLabel.text =
                $"主页\n欢迎，{user}\n\n" +
                "UI 脚本预挂在 Prefab\n" +
                "控件为 SerializeField 引用";
        }
    }
}
