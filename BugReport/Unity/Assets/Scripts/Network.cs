using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class Network : MonoBehaviour
{
    // 编辑器打印，没有实际意义
    public static string TraceError(Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine("═════════ ERROR TRACE ═════════");
        sb.AppendLine($"[Timestamp] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        sb.AppendLine($"[Machine] {Environment.MachineName}");
        sb.AppendLine($"[Process] {System.Diagnostics.Process.GetCurrentProcess().ProcessName}");

        // Recursive exception tracing
        int exceptionDepth = 0;
        Exception currentEx = ex;
        while (currentEx != null)
        {
            sb.AppendLine($"\n─── EXCEPTION #{++exceptionDepth} ───");
            sb.AppendLine($"Type: {currentEx.GetType().FullName}");
            sb.AppendLine($"Message: {currentEx.Message}");
            sb.AppendLine($"Source: {currentEx.Source}");
            sb.AppendLine($"Target Site: {currentEx.TargetSite?.Name ?? "N/A"}");
            sb.AppendLine("\nStack Trace:");
            sb.AppendLine(currentEx.StackTrace ?? "No stack trace available");

            currentEx = currentEx.InnerException;
        }

        // Environment context
        sb.AppendLine("\n─── ENVIRONMENT ───");
        sb.AppendLine($"OS: {Environment.OSVersion}");
        sb.AppendLine($"CLR: {Environment.Version}");
        sb.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
        sb.AppendLine($"64-bit Process: {Environment.Is64BitProcess}");
        sb.AppendLine($"Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB");

        // Current stack trace (to show call path to error)
        sb.AppendLine("\n─── CURRENT STACK ───");
        sb.AppendLine(new System.Diagnostics.StackTrace(true).ToString());

        sb.AppendLine("═════════ TRACE END ══════════");
        return sb.ToString();
    }

    void Awake()
    {
        Application.logMessageReceived += HandleUnityLog;
    }

    void HandleUnityLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
#if UNITY_EDITOR
            var ex = new Exception($"{type}: {condition}\n{stackTrace}");
            Debug.LogError(TraceError(ex));
#endif

            var bug = new BugTrace
            {
                Project_id = "your_project_id",
                App_version = Application.version,
                Unity_version = Application.unityVersion,
                OS = SystemInfo.operatingSystem,
                Device_model = SystemInfo.deviceModel,
                Device_memory = SystemInfo.systemMemorySize.ToString(),
                Graphics_device = SystemInfo.graphicsDeviceName,
                Graphics_memory = SystemInfo.graphicsMemorySize.ToString(),
                Cpu = SystemInfo.processorType,
                Cpu_cores = SystemInfo.processorCount.ToString(),
                System_memory = SystemInfo.systemMemorySize.ToString(),
                Error_message = condition,
                Stack_trace = stackTrace,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };

            // Send the error log to a remote server
            OnSend(bug);
        }
        else if (type == LogType.Error)
        {
            //Debug.Log($"type={type}, condition={condition}, stack={stackTrace}");
        }
        else
        {
            //Debug.Log($"type={type}");
        }
    }

    async void OnSend(BugTrace data)
    {
        await Post(data);
    }

    async Task Post(BugTrace data)
    {
        using (var client = new HttpClient())
        {
            string url = Path.Combine(Global.BaseURL, "bugtrace");
            Debug.Log($"[POST] {url}");

            var method = HttpMethod.Post;

            string body = JsonConvert.SerializeObject(data);

            try
            {
                // Send the HTTP request
                var request = new HttpRequestMessage(method, url)
                {
                    Content = body != null ? new StringContent(body, Encoding.UTF8, "application/json") : null
                };

                var response = await client.SendAsync(request);

                // Check the status code of the response
                response.EnsureSuccessStatusCode();

                // Read the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                Debug.Log($"message={responseContent}, status={response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                Debug.Log($"Error: {ex.Message}");
            }
        }
    }
}