#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPipelineMenu
{
    [MenuItem("案例/构建 AssetBundles/当前平台")]
    public static void BuildBundles()
    {
        var output = Path.Combine(Application.dataPath, "../AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString());
        Directory.CreateDirectory(output);
        BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        File.WriteAllText(Path.Combine(output, "version.json"), JsonUtility.ToJson(new RemoteVersion { version = Application.version }));
        AssetDatabase.Refresh(); Debug.Log("AB 已输出到：" + output);
    }

    [MenuItem("案例/HybridCLR/生成热更 DLL（先安装 HybridCLR）")]
    public static void GenerateHotUpdate()
    {
        Debug.Log("请在 HybridCLR/Installer 中执行 Install，再执行 HybridCLR/Generate/All；本菜单保留项目流程入口。");
    }
}
#endif
