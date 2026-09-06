using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 登录页。挂在 UI_Login.prefab 根节点上，控件在 Inspector 里拖好。
/// </summary>
public class UI_Login : UIBase
{
    [Header("在 Prefab 上拖引用")]
    [SerializeField] InputField accountInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button loginButton;
    [SerializeField] Text tipLabel;

    protected override void OnOpen()
    {
        if (passwordInput != null)
            passwordInput.contentType = InputField.ContentType.Password;

        if (loginButton != null)
        {
            loginButton.onClick.RemoveListener(OnClickLogin);
            loginButton.onClick.AddListener(OnClickLogin);
        }
        else
        {
            Debug.LogError("[UI_Login] loginButton 未在 Inspector 赋值");
        }
    }

    protected override void OnClose()
    {
        if (loginButton != null)
            loginButton.onClick.RemoveListener(OnClickLogin);
    }

    void OnClickLogin()
    {
        string user = accountInput != null ? accountInput.text.Trim() : "";
        string pass = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            if (tipLabel != null)
                tipLabel.text = "请输入账号和密码";
            return;
        }

        // 本地演示：任意非空即可
        Debug.Log($"[UI_Login] 本地登录成功 user={user}");
        UI_Home.PendingAccount = user;
        Close();
        OpenPanelAsync("UI_Home").Forget();
    }
}
