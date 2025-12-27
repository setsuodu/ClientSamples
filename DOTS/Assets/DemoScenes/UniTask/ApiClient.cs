using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient
{
    private readonly string _baseUrl; // 如 "https://api.my-server.com/v1/"
    private readonly Dictionary<string, string> _defaultHeaders = new();

    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    // 添加全局 header（如登录后调用）
    public void AddDefaultHeader(string key, string value)
    {
        _defaultHeaders[key] = value;
    }

    public void RemoveDefaultHeader(string key)
    {
        _defaultHeaders.Remove(key);
    }

    // GET 请求
    public async UniTask<T> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        return await RequestAsync<T>(UnityWebRequest.kHttpVerbGET, endpoint, null, ct);
    }

    // POST 请求（带 JSON body）
    public async UniTask<T> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        string json = JsonUtility.ToJson(body);
        return await RequestAsync<T>(UnityWebRequest.kHttpVerbPOST, endpoint, json, ct);
    }

    // PUT / DELETE 等类似，可扩展
    private async UniTask<T> RequestAsync<T>(string method, string endpoint, string jsonBody, CancellationToken ct)
    {
        string url = _baseUrl + endpoint;

        using var uwr = new UnityWebRequest(url, method);

        if (!string.IsNullOrEmpty(jsonBody))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        // 添加所有默认 header
        foreach (var header in _defaultHeaders)
        {
            uwr.SetRequestHeader(header.Key, header.Value);
        }

        try
        {
            await uwr.SendWebRequest().WithCancellation(ct);

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {uwr.error} | {uwr.downloadHandler.text}");
                throw new Exception(uwr.error);
            }

            string responseText = uwr.downloadHandler.text;
            return JsonUtility.FromJson<T>(responseText);
        }
        finally
        {
            uwr.Dispose();
        }
    }
}