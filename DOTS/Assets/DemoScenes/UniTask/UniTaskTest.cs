using Cysharp.Threading.Tasks; // 引入 UniTask 命名空间
using UnityEngine;
using UnityEngine.UI;

public class UniTaskTest : MonoBehaviour
{
    public Button myButton;

    void Awake()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(() => OnButtonClickAsync().Forget());
        }
    }

    private async UniTaskVoid OnButtonClickAsync()
    {
        Debug.Log("按钮被点击了！开始异步操作...");
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        Debug.Log("按钮点击事件后的异步操作完成！");

        var client = new ApiClient("https://jsonplaceholder.typicode.com/");
        var result = await client.GetAsync<ApiResponse>("api/login");
    }
}

public struct ApiResponse
{
    public string status;
    public string message;
}