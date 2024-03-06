using System.IO;
using UnityEngine;

public class ConstValue
{
    #region Application
    public const string APP_NAME = "moefight";
    public const string COMPANY_NAME = "moegijinka";
    public const int FPS = 60;

#if UNITY_ANDROID
    public const string PLATFORM_NAME = "Android";
    public static string LocationPath = $"{BuildDir}\\{PLATFORM_NAME}\\{Application.productName}.apk";
#elif UNITY_IOS
    public const string PLATFORM_NAME = "iOS";
    public static string LocationPath = $"{BuildDir}\\{PLATFORM_NAME}\\{Application.productName}.ipa";
#else
    public const string PLATFORM_NAME = "StandaloneWindows64";
    public static string LocationPath =  $"{BuildDir}\\{PLATFORM_NAME}\\{Application.productName}.exe";
#endif
#endregion


    #region API URL
    public const string API_DOMAIN = "http://restapi.moegijinka.cn"; //使用Http请求
    public const string GAME_DATA = "api/v1/GameCenter/game_data";
    public static string LaunchGetURL = $"{APP_NAME}/v1/GetPresent/get";
    public static string LaunchDeploy = $"{APP_NAME}/v1/GetPresent/deploy";
    #endregion


    #region AssetBundle
    public const string PATCH_NAME = "Bundles";
    // ab打包源文件
    static string _srcPath;
    public static string srcPath
    {
        get
        {
            if (string.IsNullOrEmpty(_srcPath))
            {
                _srcPath = Path.Combine(Application.dataPath, PATCH_NAME);
            }
            return _srcPath;
        }
    }
    // ab第一次输出（MD5命名后删除）
    static string _outputPath1st;
    public static string outputPath1st
    {
        get
        {
            if (string.IsNullOrEmpty(_outputPath1st))
            {
                _outputPath1st = Path.Combine(UnityDir, PATCH_NAME);
            }
            return _outputPath1st;
        }
    }
    // ab第二次输出（App、Web部署后删除）
    static string _outputPath2nd;
    public static string outputPath2nd
    {
        get
        {
            if (string.IsNullOrEmpty(_outputPath2nd))
            {
                _outputPath2nd = Path.Combine(UnityDir, PLATFORM_NAME);
            }
            return _outputPath2nd;
        }
    }

    // ab包本地下载位置
    static string ab_path;
    public static string AB_AppPath
    {
        get
        {
            if (string.IsNullOrEmpty(ab_path))
            {
                ab_path = Path.Combine(Application.persistentDataPath, PLATFORM_NAME);
            }
            return ab_path;
        }
    }
    // ab包远程部署地址
    static string ab_url;
    public static string AB_WebURL
    {
        get
        {
            if (string.IsNullOrEmpty(ab_url))
            {
                ab_url = Path.Combine(GameManager.launchGet.res_url, PLATFORM_NAME);
            }
            return ab_url;
        }
    }

    // Unity工程根目录
    static string _unity_dir;
    public static string UnityDir
    {
        get
        {
            if (string.IsNullOrEmpty(_unity_dir))
            {
                _unity_dir = System.Environment.CurrentDirectory;
            }
            return _unity_dir;
        }
    }
    public static string BuildDir = $"{UnityDir}\\Build";
    // 局域网部署根目录
    public static string GetDeployRoot
    {
        get
        {
            //注意！！！Windows中，用 '/' 的路径是无法打开的。要用 '\' 。
            string path = $"D:\\wamp64\\www\\app\\{Application.productName}";
            return path;
        }
    }
    public static string GetDeployRes { get { return $"{GetDeployRoot}\\res"; } }
    // 部署，本地目录
    public static string ZipDeploy = $"{UnityDir}\\Deploy";
    #endregion
}