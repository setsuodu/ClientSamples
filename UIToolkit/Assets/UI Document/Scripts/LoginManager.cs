using UnityEngine;
using UnityEngine.UIElements; // 必须引入 UI Toolkit 命名空间

public class LoginManager : MonoBehaviour
{
    private TextField usernameField;
    private TextField passwordField;
    private Button loginButton;

    private void OnEnable()
    {
        // 1. 获取当前 GameObject 上的 UI Document 组件
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // 2. 根据 name 查找 UI 元素
        usernameField = root.Q<TextField>("input-username");
        passwordField = root.Q<TextField>("input-password");
        loginButton = root.Q<Button>("btn-login");

        // 3. 绑定按钮点击事件
        if (loginButton != null)
        {
            loginButton.clicked += OnLoginClicked;
        }
    }

    private void OnDisable()
    {
        // 养成解绑事件的好习惯，防止内存泄漏
        if (loginButton != null)
        {
            loginButton.clicked -= OnLoginClicked;
        }
    }

    private void OnLoginClicked()
    {
        string user = usernameField.text;
        string pass = passwordField.text;

        Debug.Log($"[登录尝试] 账号: {user} | 密码: {pass}");

        // 简单验证逻辑
        if (user == "admin" && pass == "123456")
        {
            Debug.Log("登录成功！");
        }
        else
        {
            Debug.LogError("账号或密码错误！");
        }
    }
}