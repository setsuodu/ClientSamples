using VContainer.Unity;

public class GameEntryPoint : IStartable  // 或 IAsyncStartable 如果需要异步
{
    private readonly GreetingService _greetingService;

    // 构造函数注入
    public GameEntryPoint(GreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    public void Start()
    {
        _greetingService.Greet();  // 运行时会输出 "Hello World from VContainer!"
    }
}