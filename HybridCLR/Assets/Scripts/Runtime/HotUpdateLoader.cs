using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class HotUpdateLoader
{
    public static async UniTask LoadAndRunAsync(string dllFile, CancellationToken ct = default)
    {
        var path = Path.Combine(Application.persistentDataPath, "HotUpdate", dllFile);
        if (!File.Exists(path)) { Debug.LogWarning("[HotUpdate] 未找到 DLL，跳过：" + path); return; }
        // HybridCLR 的实际元数据补充应在此处按目标平台调用 LoadMetadataForAOTAssembly。
        var asm = Assembly.Load(File.ReadAllBytes(path));
        asm.GetType("HotUpdateEntry")?.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
        await UniTask.CompletedTask;
    }
}
