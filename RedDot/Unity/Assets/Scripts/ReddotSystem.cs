using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 完全通过获取服务器状态，is_read，等判断，客户端自行显示
public class ReddotSystem : MonoBehaviour
{
    // TODO: 这里注册各种存在红点的UI，他们内部的消息方法。
    // 当接收到关于 is_read 类，执行一遍 RefreshReddot();

    // 一个消息通常包含多个层级，关联多个 UI。
    // 如：未读邮件。在主界面邮箱上绘制红点，在打开邮箱，对应的邮件上还有红点。
    // 涉及一条消息，通知多个 UI_View 的机制。
    // 这里用订阅制。让需要显示红点的 UI，自行在内部订阅消息。

    // 另一个问题：关于 inactive 状态的 gameObject / script 是否能接收消息派发？→应该是能的【试验】
    public UnityEvent ReddotEvent;

    void Start()
    {
        ReddotEvent.AddListener(Action);
    }

    void Action()
    {
        
    }
}
