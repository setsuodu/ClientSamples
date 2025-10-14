using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ο�����
/// </summary>
public class HollowOutMask : Graphic, ICanvasRaycastFilter
{
    [Header("�ο�����")]
    [Space(25)]
    public RectTransform inner_trans;
    private RectTransform outer_trans;//��������

    private Vector2 inner_rt;//�ο���������Ͻ�����
    private Vector2 inner_lb;//�ο���������½�����
    private Vector2 outer_rt;//������������Ͻ�����
    private Vector2 outer_lb;//������������½�����

    [Header("�Ƿ�ʵʱˢ��")]
    [Space(25)]
    public bool realtimeRefresh;

    protected override void Awake()
    {
        base.Awake();

        outer_trans = GetComponent<RectTransform>();

        //����߽�
        CalcBounds();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (inner_trans == null)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        //0 outer���½�
        vertex.position = new Vector3(outer_lb.x, outer_lb.y);
        vh.AddVert(vertex);
        //1 outer���Ͻ�
        vertex.position = new Vector3(outer_lb.x, outer_rt.y);
        vh.AddVert(vertex);
        //2 outer���Ͻ�
        vertex.position = new Vector3(outer_rt.x, outer_rt.y);
        vh.AddVert(vertex);
        //3 outer���½�
        vertex.position = new Vector3(outer_rt.x, outer_lb.y);
        vh.AddVert(vertex);
        //4 inner���½�
        vertex.position = new Vector3(inner_lb.x, inner_lb.y);
        vh.AddVert(vertex);
        //5 inner���Ͻ�
        vertex.position = new Vector3(inner_lb.x, inner_rt.y);
        vh.AddVert(vertex);
        //6 inner���Ͻ�
        vertex.position = new Vector3(inner_rt.x, inner_rt.y);
        vh.AddVert(vertex);
        //7 inner���½�
        vertex.position = new Vector3(inner_rt.x, inner_lb.y);
        vh.AddVert(vertex);

        //����������
        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(1, 4, 5);
        vh.AddTriangle(1, 5, 2);
        vh.AddTriangle(2, 5, 6);
        vh.AddTriangle(2, 6, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(4, 7, 3);
        vh.AddTriangle(0, 4, 3);
    }

    /// <summary>
    /// ���˵����߼��
    /// </summary>
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (inner_trans == null)
        {
            return true;
        }

        return !RectTransformUtility.RectangleContainsScreenPoint(inner_trans, screenPos, eventCamera);
    }

    /// <summary>
    /// ����߽�
    /// </summary>
    private void CalcBounds()
    {
        if (inner_trans == null)
        {
            return;
        }

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer_trans, inner_trans);
        inner_rt = bounds.max;
        inner_lb = bounds.min;
        outer_rt = outer_trans.rect.max;
        outer_lb = outer_trans.rect.min;
    }

    private void Update()
    {
        if (realtimeRefresh == false)
        {
            return;
        }

        //����߽�
        CalcBounds();
        //ˢ��
        SetAllDirty();
    }
}