using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
using Newtonsoft.Json;

public class Maibox : MonoBehaviour
{
    public Transform canvas;
    public Button Btn_Mail;
    public Transform MailView;
    public Button Btn_CloseMail;

    public DataSourceMgr sourceMgr;
    public LoopListView2 mLoopListView;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;

        Btn_Mail = canvas.Find("MailBtn").GetComponent<Button>();
        Btn_Mail.onClick.AddListener(OnOpenMail);

        MailView = canvas.Find("UI_Mailbox");
        MailView.gameObject.SetActive(false);

        Btn_CloseMail = MailView.Find("CloseBtn").GetComponent<Button>();
        Btn_CloseMail.onClick.AddListener(OnCloseMail);

        sourceMgr = GetComponent<DataSourceMgr>();
    }

    void OnOpenMail()
    {
        Debug.Log("打开邮箱");

        //TODO: 请求查询邮箱，返回邮件列表
        string json = Resources.Load<TextAsset>("response").text;
        List<MailContent> mailList = JsonConvert.DeserializeObject<List<MailContent>>(json);


        //sourceMgr.SetDataTotalCount(mailList.Count);
        sourceMgr.UpdateData(mailList);
        mLoopListView.SetListItemCount(mailList.Count, false);


        MailView.gameObject.SetActive(true);
    }

    void OnCloseMail()
    {
        Debug.Log("关闭邮箱");

        MailView.gameObject.SetActive(false);
    }

    #region Test
    void Test_Response()
    {
        List<MailContent> mailList = new List<MailContent>();

        int number = Random.Range(1, 10);
        for (int i = 0; i < number; i++)
        {
            MailContent mail = new MailContent();
            mail.title = "标题" + i;
            mail.content = "亲爱的用户：\n内容" + i;
            mail.attachments = "";
            mail.datetime = "2024-06-0" + i;
            mail.is_read = false;

            mailList.Add(mail);
        }

        string json = JsonConvert.SerializeObject(mailList);
        Debug.Log(json);
    }
    #endregion
}
