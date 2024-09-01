using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public LayerMask hitLayers; //只接收该层点击反馈

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    // 不要移动
                    Debug.Log("障碍物");
                }
                else
                {
                    this.transform.position = hit.point;
                }
            }
        }
    }
}