using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform StartPosition;
    public LayerMask WallMask;
    public Vector2 vGridWorldSize;
    public float fNodeRadius; //半径
    public float fDistanceBetweenNodes;

    Node[,] NodeArray; //地图Node
    public List<Node> FinalPath;

    float fNodeDiameter; //直径
    int iGridSizeX, iGridSizeY;

    void Start()
    {
        fNodeDiameter = fNodeRadius * 2;//Double the radius to get diameter
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        CreateGrid();//Draw the grid
    }

    // 初始化地图 NodeArray
    void CreateGrid()
    {
        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);
                bool Wall = true;
                if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask)) //检查每个Node是否是遮挡物
                {
                    Wall = false;
                }
                NodeArray[x, y] = new Node(Wall, worldPoint, x, y);
            }
        }
    }

    // 世界坐标转Node网格
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }

    // 获取某个Node周围4个点
    public List<Node> GetNeighboringNodes(Node a_Node)
    {
        List<Node> NeighboringNodes = new List<Node>();
        int xCheck;
        int yCheck;

        // Right Side
        xCheck = a_Node.gridX + 1;
        yCheck = a_Node.gridY;
        if (xCheck >= 0 && xCheck < iGridSizeX)
        {
            if (yCheck >= 0 && yCheck < iGridSizeY)
            {
                NeighboringNodes.Add(NodeArray[xCheck, yCheck]);
            }
        }

        // Left Side
        xCheck = a_Node.gridX - 1;
        yCheck = a_Node.gridY;
        if (xCheck >= 0 && xCheck < iGridSizeX)
        {
            if (yCheck >= 0 && yCheck < iGridSizeY)
            {
                NeighboringNodes.Add(NodeArray[xCheck, yCheck]);
            }
        }

        // Top Side
        xCheck = a_Node.gridX;
        yCheck = a_Node.gridY + 1;
        if (xCheck >= 0 && xCheck < iGridSizeX)
        {
            if (yCheck >= 0 && yCheck < iGridSizeY)
            {
                NeighboringNodes.Add(NodeArray[xCheck, yCheck]);
            }
        }

        // Bottom Side
        xCheck = a_Node.gridX;
        yCheck = a_Node.gridY - 1;
        if (xCheck >= 0 && xCheck < iGridSizeX)
        {
            if (yCheck >= 0 && yCheck < iGridSizeY)
            {
                NeighboringNodes.Add(NodeArray[xCheck, yCheck]);
            }
        }

        return NeighboringNodes;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

        if (NodeArray != null)
        {
            foreach (Node node in NodeArray)
            {
                if (node.IsWall)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                if (FinalPath != null)
                {
                    if (FinalPath.Contains(node))//If the current node is in the final path
                    {
                        Gizmos.color = Color.red;//Set the color of that node
                    }
                }

                Gizmos.DrawCube(node.Postion, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));
            }
        }
    }
}