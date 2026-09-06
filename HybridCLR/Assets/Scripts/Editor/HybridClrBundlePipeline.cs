#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// HybridCLR 热更 DLL → Assets/Bundles（走 AssetBundleFramework 规范）自动化。
/// 1) CompileDll
/// 2) 拷贝到 Assets/Bundles/Code/HotUpdate.bytes
/// 3) 可选：拷贝补充元数据到 Assets/Bundles/AotDumps/*.bytes
/// 之后用 Tools/AssetBundle 打 AB；运行时 ResManager 从 AB 读 TextAsset.bytes。
/// </summary>
public static class HybridClrBundlePipeline
{
    const string BundlesCodeDir = "Assets/Bundles/Code";
    const string BundlesAotDir = "Assets/Bundles/AotDumps";
    const string DestHotUpdateBytes = "Assets/Bundles/Code/HotUpdate.bytes";

    [MenuItem("案例/HybridCLR/1. 编译热更DLL并同步到 Bundles", false, 100)]
    public static void CompileAndCopyToBundles()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        Debug.Log($"[HybridCLR流水线] CompileDll → {target}");

        if (!TryCompileDll(target))
        {
            Debug.LogError(
                "[HybridCLR流水线] CompileDll 失败。请先：\n" +
                "1) 安装 HybridCLR 并 Installer 成功\n" +
                "2) Settings 里 Hot Update Assemblies 包含 HotUpdate\n" +
                "3) 菜单 HybridCLR/CompileDll/ActiveBuildTarget 能单独跑通");
            return;
        }

        string srcDll = FindCompiledHotUpdateDll(target);
        if (string.IsNullOrEmpty(srcDll) || !File.Exists(srcDll))
        {
            Debug.LogError(
                "[HybridCLR流水线] 未找到编译产物 HotUpdate.dll。\n" +
                "预期目录: HybridCLRData/HotUpdateDlls/<Platform>/HotUpdate.dll");
            return;
        }

        Directory.CreateDirectory(BundlesCodeDir);
        File.Copy(srcDll, DestHotUpdateBytes, true);
        AssetDatabase.ImportAsset(DestHotUpdateBytes, ImportAssetOptions.ForceUpdate);
        Debug.Log($"[HybridCLR流水线] 已拷贝:\n  {srcDll}\n→ {DestHotUpdateBytes}");

        int aotCount = CopyAotStripDlls(target);
        if (aotCount > 0)
            Debug.Log($"[HybridCLR流水线] 已同步 {aotCount} 个 AOT 补充元数据 → {BundlesAotDir}");

        AssetDatabase.Refresh();
        Debug.Log(
            "[HybridCLR流水线] 完成。下一步：\n" +
            "  Tools/AssetBundle → Build（+ 同步 StreamingAssets）\n" +
            "运行时将从 AB 加载 code/hotupdate 下的 HotUpdate（TextAsset.bytes）");
    }

    [MenuItem("案例/HybridCLR/2. 仅同步已编译 DLL 到 Bundles（不编译）", false, 101)]
    public static void CopyOnly()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string srcDll = FindCompiledHotUpdateDll(target);
        if (string.IsNullOrEmpty(srcDll) || !File.Exists(srcDll))
        {
            Debug.LogError("[HybridCLR流水线] 没有已编译的 HotUpdate.dll，请先 CompileDll");
            return;
        }
        Directory.CreateDirectory(BundlesCodeDir);
        File.Copy(srcDll, DestHotUpdateBytes, true);
        AssetDatabase.ImportAsset(DestHotUpdateBytes, ImportAssetOptions.ForceUpdate);
        CopyAotStripDlls(target);
        AssetDatabase.Refresh();
        Debug.Log($"[HybridCLR流水线] 仅拷贝完成 → {DestHotUpdateBytes}");
    }

    [MenuItem("案例/HybridCLR/打开 HybridCLRData 输出目录", false, 120)]
    public static void OpenHybridClrData()
    {
        string root = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "HybridCLRData");
        if (!Directory.Exists(root))
        {
            Debug.LogWarning("[HybridCLR流水线] 目录不存在: " + root);
            return;
        }
        EditorUtility.RevealInFinder(root);
    }

    static bool TryCompileDll(BuildTarget target)
    {
        // 反射调用，避免未安装 HybridCLR 时编辑器脚本编译失败
        var type = System.Type.GetType("HybridCLR.Editor.Commands.CompileDllCommand, HybridCLR.Editor");
        if (type == null)
        {
            // 兼容部分版本程序集名
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType("HybridCLR.Editor.Commands.CompileDllCommand");
                if (type != null) break;
            }
        }
        if (type == null)
        {
            Debug.LogError("[HybridCLR流水线] 找不到 HybridCLR.Editor.Commands.CompileDllCommand");
            return false;
        }

        var method = type.GetMethod(
            "CompileDll",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            null,
            new[] { typeof(BuildTarget) },
            null);
        if (method == null)
        {
            // 尝试 ActiveBuildTarget 无参
            method = type.GetMethod("CompileDllActiveBuildTarget",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, null);
                return true;
            }
            Debug.LogError("[HybridCLR流水线] 找不到 CompileDll(BuildTarget) 方法");
            return false;
        }

        method.Invoke(null, new object[] { target });
        return true;
    }

    static string FindCompiledHotUpdateDll(BuildTarget target)
    {
        string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        string platform = target.ToString();
        string root = Path.Combine(projectRoot, "HybridCLRData", "HotUpdateDlls");

        string[] candidates =
        {
            Path.Combine(root, platform, "HotUpdate.dll"),
            Path.Combine(root, platform, "HotUpdate", "HotUpdate.dll"),
        };
        foreach (var c in candidates)
        {
            if (File.Exists(c)) return c;
        }

        if (Directory.Exists(root))
        {
            // 全盘扫 HotUpdate.dll，并打印目录便于排查
            var found = Directory.GetFiles(root, "HotUpdate.dll", SearchOption.AllDirectories);
            if (found.Length > 0)
            {
                Debug.Log("[HybridCLR流水线] 扫描到: " + found[0]);
                return found[0];
            }
            Debug.LogWarning("[HybridCLR流水线] HotUpdateDlls 目录存在但无 HotUpdate.dll，子目录:" + string.Join("", Directory.GetDirectories(root)));
        }
        else
        {
            Debug.LogWarning("[HybridCLR流水线] 不存在目录: " + root);
        }
        return null;
    }

    static int CopyAotStripDlls(BuildTarget target)
    {
        string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        string srcDir = Path.Combine(projectRoot, "HybridCLRData", "AssembliesPostIl2CppStrip", target.ToString());
        if (!Directory.Exists(srcDir))
            return 0;

        Directory.CreateDirectory(BundlesAotDir);
        // 清理旧 bytes（仅本流水线生成的）
        foreach (var old in Directory.GetFiles(BundlesAotDir, "*.bytes"))
            File.Delete(old);

        int n = 0;
        foreach (var dll in Directory.GetFiles(srcDir, "*.dll"))
        {
            string name = Path.GetFileNameWithoutExtension(dll) + ".bytes";
            string dest = Path.Combine(BundlesAotDir, name);
            // Asset 路径
            string assetPath = BundlesAotDir + "/" + name;
            File.Copy(dll, dest.Replace('/', Path.DirectorySeparatorChar), true);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            n++;
        }
        return n;
    }
}
#endif
