using UnityEngine;

// 将此脚本加入 HybridCLR 的 HotUpdate Assembly Definition。
// 真机流程：主工程启动 -> 下载 HotUpdate.dll -> HybridCLR.LoadMetadataForAOTAssembly -> Assembly.Load -> 调用本入口。
public static class HotUpdateEntry
{
    public static void Run()
    {
        Debug.Log("[HotUpdate] HotUpdateEntry.Run invoked");
    }
}
