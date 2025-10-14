[System.Serializable]
public class MailContent
{
    public string title; //标题文字（竞技场每日结算，国庆排行结算，活动结算，更新补偿，，）
    public string content; //内容文字（亲爱的用户：）
    public string attachments; //附件（物品列表）
    public string datetime; //发件时间
    public string endtime; //到期时间
    public bool is_read; //已读
}

[System.Serializable]
public class Items
{
    public string itemID; //物品ID（知道ID，其余信息都本地查表获得）
    public string ItemType; //物品类型（1：金币，2：钻石，3：体力，4：道具，5：英雄）
    public string rarities; //稀有度（普通白，稀有蓝，传奇橙）
    public string itemIconPath; //图片路径
    public string ItemModelPath; //模型路径
    public int BuyPrice; //购买价格
    public int SellPrice; //回收价格
    public int StackSize; //堆叠上限
    public string DisplayName_Key; //展示名称本地化Key（另外查表）
    public string Description_Key; //描述本地化Key（另外查表）
}