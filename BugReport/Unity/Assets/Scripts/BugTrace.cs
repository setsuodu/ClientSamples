// 客户端捕捉异常日志堆栈结构
[System.Serializable]
public class BugTrace
{
    public string Project_id { get; set; }
    public string App_version { get; set; }
    public string Unity_version { get; set; }
    public string OS { get; set; }
    public string Device_model { get; set; }
    public string Device_memory { get; set; }
    public string Graphics_device { get; set; }
    public string Graphics_memory { get; set; }
    public string Cpu { get; set; }
    public string Cpu_cores { get; set; }
    public string System_memory { get; set; }
    public string Error_message { get; set; }
    public string Stack_trace { get; set; }
    public string Timestamp { get; set; }
}