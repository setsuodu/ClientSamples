using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class LoginPage
{
    public static async UniTask ShowAsync(Canvas canvas, CancellationToken ct)
    {
        var root = new GameObject("LoginPage", typeof(RectTransform), typeof(Image)); root.transform.SetParent(canvas.transform,false);
        var bg=root.GetComponent<Image>(); bg.color=new Color(.08f,.12f,.18f,1); Stretch(root.GetComponent<RectTransform>());
        SimpleUI.Label(root.transform,"HybridCLR 热更案例",42).rectTransform.anchoredPosition=new Vector2(0,220);
        var account=Input(root.transform,"账号",new Vector2(0,80)); var password=Input(root.transform,"密码",new Vector2(0,0)); password.contentType=InputField.ContentType.Password;
        var btn=Button(root.transform,"登 录",new Vector2(0,-110)); 
        var tip=SimpleUI.Label(root.transform,"本示例为本地模拟登录",20); tip.rectTransform.anchoredPosition=new Vector2(0,-190); tip.raycastTarget = false;
        bool clicked=false; btn.onClick.AddListener(()=>clicked=true);
        await UniTask.WaitUntil(()=>clicked, cancellationToken:ct);
        if (string.IsNullOrWhiteSpace(account.text) || string.IsNullOrWhiteSpace(password.text)) { tip.text="请输入账号和密码"; clicked=false; await ShowAsync(canvas,ct); Object.Destroy(root); return; }
        Object.Destroy(root); HomePage.Show(canvas,account.text);
    }
    static InputField Input(Transform p,string hint,Vector2 pos){var go=new GameObject(hint,typeof(RectTransform),typeof(Image),typeof(InputField));go.transform.SetParent(p,false);var rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(420,64);rt.anchoredPosition=pos;go.GetComponent<Image>().color=Color.white;var f=go.GetComponent<InputField>();var text=SimpleUI.Label(go.transform,"",24);text.color=Color.black;text.alignment=TextAnchor.MiddleLeft;text.rectTransform.anchorMin=new Vector2(0,.0f);text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=new Vector2(18,0);text.rectTransform.offsetMax=new Vector2(-18,0);f.textComponent=text;f.placeholder=SimpleUI.Label(go.transform,hint,22);f.placeholder.color=Color.gray;return f;}
    static Button Button(Transform p,string label,Vector2 pos){var go=new GameObject("LoginButton",typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(p,false);var rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(420,70);rt.anchoredPosition=pos;go.GetComponent<Image>().color=new Color(.1f,.5f,.9f);SimpleUI.Label(go.transform,label,26);return go.GetComponent<Button>();}
    static void Stretch(RectTransform rt){rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;}
}

public static class HomePage
{
    public static void Show(Canvas canvas,string account){var root=new GameObject("HomePage",typeof(RectTransform),typeof(Image));root.transform.SetParent(canvas.transform,false);var bg=root.GetComponent<Image>();bg.color=new Color(.12f,.2f,.14f,1);var rt=root.GetComponent<RectTransform>();rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;SimpleUI.Label(root.transform,$"主页\n欢迎，{account}\n\nHybridCLR 热更已完成\nAssetBundleFramework 资源已就绪",36);}
}
