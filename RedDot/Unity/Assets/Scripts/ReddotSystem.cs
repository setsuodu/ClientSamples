using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ��ȫͨ����ȡ������״̬��is_read�����жϣ��ͻ���������ʾ
public class ReddotSystem : MonoBehaviour
{
    // TODO: ����ע����ִ��ں���UI�������ڲ�����Ϣ������
    // �����յ����� is_read �ִ࣬��һ�� RefreshReddot();

    // һ����Ϣͨ����������㼶��������� UI��
    // �磺δ���ʼ����������������ϻ��ƺ�㣬�ڴ����䣬��Ӧ���ʼ��ϻ��к�㡣
    // �漰һ����Ϣ��֪ͨ��� UI_View �Ļ��ơ�
    // �����ö����ơ�����Ҫ��ʾ���� UI���������ڲ�������Ϣ��

    // ��һ�����⣺���� inactive ״̬�� gameObject / script �Ƿ��ܽ�����Ϣ�ɷ�����Ӧ�����ܵġ����顿
    public UnityEvent ReddotEvent;

    void Start()
    {
        ReddotEvent.AddListener(Action);
    }

    void Action()
    {
        
    }
}
