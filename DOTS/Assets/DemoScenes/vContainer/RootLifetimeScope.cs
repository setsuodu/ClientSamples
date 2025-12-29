using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 注册服务（Singleton：整个项目单例）
        builder.Register<GreetingService>(Lifetime.Singleton);

        // 注册入口点（自动调用 Start）
        builder.RegisterEntryPoint<GameEntryPoint>();
    }
}