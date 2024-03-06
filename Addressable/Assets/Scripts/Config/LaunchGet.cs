using UnityEngine;

// APP首次启动请求
public class LaunchGet
{
    public LaunchGet()
    {
        website = "https://moegijinka.cn";
        gate_server = "moegijinka.cn"; //本地host配了域名
        app_url = $"https://moegijinka.cn/{Application.productName}";
        res_url = $"http://app.moegijinka.cn/{Application.productName}/res";
        app_version = "1.0.0";
        res_version = "1";
        notice = string.Empty;
    }
    public string website;      //官网地址
    public string gate_server;  //网关地址
    public string app_url;      //应用下载地址
    public string res_url;      //AB资源包地址
    public string app_version;  //官方App版本（检查更新）
    public string res_version;  //官方资源版本（检查更新）
    public string notice;       //启动弹窗公告
    public override string ToString()
    {
        string content = $"\n[website] {website}\n[gate_server] {gate_server}\n[app_url] {app_url}\n[res_url] {res_url}\n[app_version] {app_version}\n[res_version] {res_version}\n[notice] {notice}";
        return content;
    }
}