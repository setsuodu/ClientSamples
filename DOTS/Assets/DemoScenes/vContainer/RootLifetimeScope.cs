using MessagePipe;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        var options = builder.RegisterMessagePipe();  // 注册 MessagePipe，返回选项可配置

        // 注册消息类型（必须手动注册每个消息类型）
        builder.RegisterMessageBroker<PlayerAttackData>(options);

        // 启用诊断窗口（可选，但推荐）
        builder.RegisterBuildCallback(resolver => GlobalMessagePipe.SetProvider(resolver.AsServiceProvider()));


        // 注册服务（Singleton：整个项目单例）
        builder.Register<GreetingService>(Lifetime.Singleton);
        // 自动查找并注入场景中的组件
        builder.Register<GameManager>(Lifetime.Singleton);

        // 注册入口点（自动调用 Start）
        builder.RegisterEntryPoint<GameEntryPoint>();
    }
}