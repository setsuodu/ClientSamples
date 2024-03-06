// 和TP5的model格式一致
public class ServerResponse
{
    public ServerResponse()
    {
        status = 0;
        msg = string.Empty;
        data = string.Empty;
    }
    public int status;
    public string msg;
    public string data;
}

public class C2S_QRVerify
{
    public C2S_QRVerify()
    {
        code = string.Empty;
    }
    public string code;
}
public class C2S_Login
{
    public C2S_Login()
    {
        username = string.Empty;
        password = string.Empty;
        token = string.Empty;
    }
    public string username;
    public string password;
    public string token;
}
public class S2C_Login
{
    public S2C_Login()
    {
        username = string.Empty;
        nickname = string.Empty;
        token = string.Empty;
        library = string.Empty;
    }
    public string username;
    public string nickname;
    public string token;
    public string library;
}

public class DBApp
{
    public DBApp()
    {
        id = 0;
        name = string.Empty;
        platform = string.Empty;
        bundle = string.Empty;
        price = 0;
        app_version = string.Empty;
        res_version = string.Empty;
        intro = string.Empty;
        discount = 0;
        relation = 0;
    }
    public int id;
    public string name;
    public string platform;
    public string bundle;
    public int price;
    public string app_version;
    public string res_version;
    public string intro;
    public int discount;
    public int relation;
}

public class C2S_Deploy
{
    public C2S_Deploy()
    {
        app_version = string.Empty;
        res_version = string.Empty;
    }
    public string app_version;
    public string res_version;
}