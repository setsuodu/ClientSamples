using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AOT：从 AB 加载热更 DLL bytes。
/// 不直接 using HybridCLR，避免 Main 未引用 HybridCLR.Runtime 时 CompileDll 失败。
/// 真机补充元数据通过反射调用 HybridCLR.RuntimeApi。
/// </summary>
public static class HotUpdateLoader
{
    public const string HotUpdateBundle = "code/hotupdate";
    public const string HotUpdateAsset = "HotUpdate";

    const string AssemblyName = "HotUpdate";
    const string EntryTypeName = "HotUpdateEntry";
    const string EntryMethodName = "Run";

    public static async UniTask LoadAndRunAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        Assembly hotAss;
#if UNITY_EDITOR
        hotAss = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == AssemblyName);
        if (hotAss == null)
        {
            Debug.LogError("[HotUpdate] Editor 域中无 HotUpdate，检查 HotUpdate.asmdef");
            return;
        }
        Debug.Log("[HotUpdate] Editor：使用已加载程序集");
#else
        LoadAotMetadataFromAb(ct);

        var text = await ResManager.LoadAsync<TextAsset>(HotUpdateBundle, HotUpdateAsset, ct);
        if (text == null || text.bytes == null || text.bytes.Length == 0)
        {
            Debug.LogError($"[HotUpdate] AB 加载失败: {HotUpdateBundle}/{HotUpdateAsset}");
            return;
        }

        hotAss = Assembly.Load(text.bytes);
        Debug.Log($"[HotUpdate] 从 AB 加载成功 size={text.bytes.Length}");
#endif

        var type = hotAss.GetType(EntryTypeName);
        var method = type?.GetMethod(EntryMethodName, BindingFlags.Public | BindingFlags.Static);
        if (method == null)
        {
            Debug.LogError($"[HotUpdate] 找不到 {EntryTypeName}.{EntryMethodName}");
            return;
        }
        method.Invoke(null, null);
        await UniTask.CompletedTask;
    }

#if !UNITY_EDITOR
    static void LoadAotMetadataFromAb(CancellationToken ct)
    {
        // 反射调用，不依赖编译期 HybridCLR 引用
        var runtimeApi = Type.GetType("HybridCLR.RuntimeApi, HybridCLR.Runtime")
                      ?? Type.GetType("HybridCLR.RuntimeApi");
        if (runtimeApi == null)
        {
            Debug.LogWarning("[HotUpdate] 无 HybridCLR.RuntimeApi，跳过 AOT 元数据");
            return;
        }

        var loadMethod = runtimeApi.GetMethod(
            "LoadMetadataForAOTAssembly",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] { typeof(byte[]), Type.GetType("HybridCLR.HomologousImageMode, HybridCLR.Runtime") ?? typeof(int) },
            null);

        // 简化：找所有 LoadMetadataForAOTAssembly(byte[], *)
        if (loadMethod == null)
        {
            foreach (var m in runtimeApi.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name != "LoadMetadataForAOTAssembly") continue;
                var ps = m.GetParameters();
                if (ps.Length >= 1 && ps[0].ParameterType == typeof(byte[]))
                {
                    loadMethod = m;
                    break;
                }
            }
        }

        if (loadMethod == null)
        {
            Debug.LogWarning("[HotUpdate] 找不到 LoadMetadataForAOTAssembly");
            return;
        }

        string[] names =
        {
            "mscorlib", "System", "System.Core",
            "UnityEngine.CoreModule",
        };

        foreach (var name in names)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                // 同步路径仅作占位；正式项目可改成 await ResManager
                // 这里不强制加载，缺 AB 就跳过
                Debug.Log($"[HotUpdate] AOT 元数据占位跳过（可按需接 ResManager）: {name}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[HotUpdate] AOT 元数据 {name}: {e.Message}");
            }
        }
    }
#endif
}
