# HybridCLR + AssetBundleFramework 热更案例

这是一个极简 Unity UGUI 案例，目标流程是：**启动检查版本 → AssetBundleFramework 初始化并下载 AB → 显示登录页 → 输入任意非空账号密码 → 直接进入主页**。

## Environment

> Unity6000.5.x/6.x 向CLR迁移过程，大量底层API变更，目前不支持 HybridCLR。 
6000.5.x 才开始支持 Panel Renderer，UI Document 长期将废弃。
所以本案例使用 6000.3.x LTS + uGUI

## 依赖

| 依赖 | 版本/来源 |
|---|---|
| `com.setsuodu.assetbundleframework` | `1.0.2`，OpenUPM |
| HybridCLR | 通过 Package Manager / HybridCLR Installer 安装，建议使用与项目 Unity 版本匹配的稳定版 |
| UniTask | `2.5.10` |
| Unity | 建议 Unity 2021.3 LTS 或更高 LTS |

`Packages/manifest.json` 已写入 AssetBundleFramework 依赖。若你的 OpenUPM 注册表不是全局启用，请在 Package Manager 中添加 `https://package.openupm.com`，并按包页面提供的 scope 安装该包。

## 使用步骤

1. 用 Unity Hub 打开本目录，等待 Package Manager 完成依赖解析。
2. 安装并执行 HybridCLR Installer；然后执行 `HybridCLR/Generate/All`，生成 AOT 泛型引用和热更程序集环境。
3. 打开 `Assets/Scenes/Bootstrap.unity`，运行场景即可看到启动检查、资源初始化、登录与主页流程。
4. 在菜单 `案例/构建 AssetBundles/当前平台` 构建 AB。AssetBundleFramework 的 `ABManager.InitializeAsync` 负责运行时读取资源版本、检查缓存并下载缺失或更新资源。
5. 生产环境把 `GameBootstrap.remoteVersionUrl` 指向 CDN 的 `version.json`；开发环境为空时使用 `Resources/HotUpdate/version.json`。
6. 若要启用真正的 DLL 热更，把 `HotUpdateEntry.cs` 放入 HybridCLR HotUpdate Assembly Definition，构建 DLL 后上传到 CDN，再由 `HotUpdateLoader.LoadAndRunAsync` 加载。

## 关键代码

`GameBootstrap` 是启动入口。它先调用 `VersionChecker.CheckAsync`，然后调用用户提供的 `ResManager.InitializeAsync`。`ResManager` 保留了用户脚本中的 ABManager 门面调用：`InitializeAsync`、`LoadAssetAsync`、`UnloadAll`、引用计数和 UI/模型/道具快捷加载。

`LoginPage` 和 `HomePage` 使用运行时创建的 UGUI 控件，因此不依赖额外美术资源。登录按钮只做本地演示校验：账号和密码非空即成功，不连接服务器，也不保存凭据。

## 注意

本仓库提供的是可扩展的工程骨架和业务流程示例；AssetBundleFramework 与 HybridCLR 的具体安装器版本应以你当前 Unity 版本对应的官方文档为准。不同包版本若调整了命名空间或初始化参数，只需在 `ResManager.cs` 的 ABManager 适配层修改，不影响登录和启动业务层。
