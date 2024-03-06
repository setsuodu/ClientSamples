using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Get;
    private static bool Initialized = false;
    //public static Present present;

    private Transform canvasRoot;
    private UI_CheckUpdate ui_check;

    void Awake()
    {
        if (!Initialized)
        {
            Get = this;
            DontDestroyOnLoad(gameObject);

            SystemSetting();

            BindAssets();

            GetConfig();
        }
        else
        {
            OnInited();
        }
    }

    void OnApplicationQuit()
    {
        Initialized = false;
    }

    // 系统设置
    void SystemSetting()
    {
        Time.timeScale = 1.0f;
        //Time.fixedDeltaTime = 1f / Constants.FPS;
        //Application.targetFrameRate = Constants.FPS; //锁定渲染帧60，不锁是-1
        QualitySettings.vSyncCount = 0; //只能是0/1/2，0是不等待垂直同步
        Screen.fullScreen = false;
        //Screen.SetResolution(540, 960, false);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Debug.unityLogger.logEnabled = false; //release版关闭
    }

    // 绑定组件
    void BindAssets()
    {
        // 初始化目录
        if (!Directory.Exists(ConstValue.AB_AppPath))
            Directory.CreateDirectory(ConstValue.AB_AppPath);

        // 初始化各种管理器
        //GameObject clientNet = new GameObject("ClientNet");
        //clientNet.transform.SetParent(this.transform);
        //clientNet.AddComponent<ClientNet>();

        GameObject uiManager = new GameObject("UIManager");
        uiManager.transform.SetParent(this.transform);
        uiManager.AddComponent<HotFix.UIManager>();

        //GameObject configManager = new GameObject("ConfigManager");
        //configManager.transform.SetParent(this.transform);
        //configManager.AddComponent<ConfigManager>();

        //GameObject audioManager = new GameObject("AudioManager");
        //audioManager.transform.SetParent(this.transform);
        //audioManager.AddComponent<AudioManager>();


        // 初始UI
        canvasRoot = GameObject.Find("Canvas").transform;
        string ui_name = "UI_CheckUpdate";
        GameObject asset = Resources.Load<GameObject>(ui_name);
        GameObject obj = Instantiate(asset, canvasRoot);
        obj.name = ui_name;
        if (obj.GetComponent<UI_CheckUpdate>() == false)
            obj.AddComponent<UI_CheckUpdate>();
        ui_check = obj.GetComponent<UI_CheckUpdate>();
    }

    // 请求游戏配置
    async void GetConfig()
    {
        string text = await HttpHelper.TryGetAsync(ConstValue.PRESENT_GET);
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError($"配置请求失败: {ConstValue.PRESENT_GET}");
            return;
        }
        Debug.Log($"success: {text}");
        var obj = JsonConvert.DeserializeObject<ServerResponse>(text);
        //present = JsonConvert.DeserializeObject<Present>(obj.data);


#if UNITY_EDITOR && !USE_ASSETBUNDLE
        // 不检查更新
        OnInited();
#else
        // 加载配置（需要启动资源服务器）
        StartCoroutine(ui_check.StartCheck(OnInited));
#endif
    }

    void OnInited()
    {
        Initialized = true;

        // 进入HotFix代码
        //ConfigManager.Get().Load(); //AB加载完毕，加载配置
        ui_check.gameObject.SetActive(false);
        // 加载第一个UI
        //UIManager.Get().Push<UI_Login>();
    }

    void CheckUpdate()
    {
    
    }
}