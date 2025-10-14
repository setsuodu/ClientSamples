using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class SimpleHttpServer : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;
    private bool isRunning = true;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add(Global.BaseURL);
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

        listenerThread = new Thread(StartListener);
        listenerThread.Start();
        Debug.Log("Server started on port 5171");
    }

    private void StartListener()
    {
        while (isRunning)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne(1000); // Wait with timeout
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);
        Debug.Log("Request received: " + context.Request.Url);

        if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/bugtrace")
        {
            Debug.Log("POST~~BUG~~");

            string json;
            using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }
            Debug.Log("Received JSON: " + json);

            // Deserialize JSON
            BugTrace bugTrace = JsonConvert.DeserializeObject<BugTrace>(json);
            Debug.Log($"Deserialized: message={bugTrace.Error_message}, stack={bugTrace.Stack_trace}");
        }
        else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/user")
        {
            Debug.Log("GET~~~");
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
        }
    }

    void OnDestroy()
    {
        isRunning = false;
        listenerThread?.Abort();
        listener?.Stop();
        listener?.Close();
    }
}