using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    [SerializeField]
    public FlowField curFlowField;

    public void InitializeFlowField()
    {
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
        Debug.Log($"create : {cellRadius}, {gridSize}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitializeFlowField();
        }
    }

    void OnDrawGizmos()
    {
        if (curFlowField == null)
        {
            DrawGrid(gridSize, Color.yellow, cellRadius);
        }
        else
        {
            DrawGrid(gridSize, Color.green, cellRadius);
        }
    }

    void DrawGrid(Vector2Int drawGridSize, Color color , float drawCellRadius)
    {
        Gizmos.color = color;

        for (int x = 0; x < drawGridSize.x; x++)
        {
            for (int y = 0; y < drawGridSize.y; y++)
            {
                Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
