using UnityEngine;

public class TestCost : MonoBehaviour
{
    Grid GridReference;//For referencing the grid class

    void Awake()
    {
        GridReference = GetComponent<Grid>();//Get a reference to the game manager
    }

    void Update()
    {
        // 右键，计算代价
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                Node node = GridReference.NodeFromWorldPoint(hit.point);
                //Debug.Log($"{hit.point} --- {node.igCost} + {node.ihCost} = {node.igCost + node.ihCost}");
                Debug.Log($"({node.gridX}, {node.gridY}) --- {node.igCost} + {node.ihCost} = {node.igCost + node.ihCost}");
            }
        }
    }
}