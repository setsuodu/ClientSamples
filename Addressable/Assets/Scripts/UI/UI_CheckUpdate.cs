using System.IO;
using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class UI_CheckUpdate : MonoBehaviour
{
    //internal static event System.Action OnUnZipCompletedEvent; //解压完成事件
    internal static event System.Action OnDownloadCompleteEvent; //下载完成事件

    private Text m_ProgressText;
    private Slider m_ProgressSlider;
    //private List<ABInfo> downloadList;
    private int fileCount = 0;

    void Awake()
    {
        m_ProgressText = transform.Find("Slider").Find("Text").GetComponent<Text>();
        m_ProgressSlider = transform.Find("Slider").GetComponent<Slider>();
        //downloadList = new List<ABInfo>();
        fileCount = 0;
    }

    void Update()
    {
        //float percent = downloadList.Count == 0 ? 0 : ((float)fileCount / (float)downloadList.Count);
        //m_ProgressText.text = $"{(percent * 100).ToString("F0")}%";
        m_ProgressSlider.value = fileCount;
    }

    static IEnumerator BeginDownLoad(string downloadfileName, string desFileName)
    {
        //Debug.Log($"BeginDownLoad: {downloadfileName}\nTo: {desFileName}");
        if (downloadfileName.Contains("http") == false)
        {
            downloadfileName = $"http://{downloadfileName}";
        }
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(downloadfileName);
        request.Timeout = 5000;
        WebResponse response = request.GetResponse();
        using (FileStream fs = new FileStream(desFileName, FileMode.Create))
        using (Stream netStream = response.GetResponseStream())
        {
            int packLength = 1024 * 20;
            long countLength = response.ContentLength;
            byte[] nbytes = new byte[packLength];
            int nReadSize = 0;
            nReadSize = netStream.Read(nbytes, 0, packLength);
            while (nReadSize > 0)
            {
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = netStream.Read(nbytes, 0, packLength);

                double dDownloadedLength = fs.Length * 1.0 / (1024 * 1024);
                double dTotalLength = countLength * 1.0 / (1024 * 1024);
                string ss = string.Format("已下载 {0:F3}M / {1:F3}M", dDownloadedLength, dTotalLength);
                //Debug.Log(ss);
                yield return false;
            }
            netStream.Close();
            fs.Close();
            if (OnDownloadCompleteEvent != null)
            {
                Debug.Log("download  finished");
                OnDownloadCompleteEvent.Invoke();
            }
        }
    }
}