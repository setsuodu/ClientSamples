using UnityEngine;

// APP�״���������
public class LaunchGet
{
    public LaunchGet()
    {
        website = "https://moegijinka.cn";
        gate_server = "moegijinka.cn"; //����host��������
        app_url = $"https://moegijinka.cn/{Application.productName}";
        res_url = $"http://app.moegijinka.cn/{Application.productName}/res";
        app_version = "1.0.0";
        res_version = "1";
        notice = string.Empty;
    }
    public string website;      //������ַ
    public string gate_server;  //���ص�ַ
    public string app_url;      //Ӧ�����ص�ַ
    public string res_url;      //AB��Դ����ַ
    public string app_version;  //�ٷ�App�汾�������£�
    public string res_version;  //�ٷ���Դ�汾�������£�
    public string notice;       //������������
    public override string ToString()
    {
        string content = $"\n[website] {website}\n[gate_server] {gate_server}\n[app_url] {app_url}\n[res_url] {res_url}\n[app_version] {app_version}\n[res_version] {res_version}\n[notice] {notice}";
        return content;
    }
}