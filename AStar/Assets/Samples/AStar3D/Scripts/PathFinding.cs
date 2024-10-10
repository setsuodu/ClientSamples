using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Grid GridReference;//For referencing the grid class
    public Transform StartPosition;//Starting position to pathfind from
    public Transform TargetPosition;//Starting position to pathfind to

    void Awake()
    {
        GridReference = GetComponent<Grid>();//Get a reference to the game manager
    }

    void Update()
    {
        FindPath(StartPosition.position, TargetPosition.position);//Find a path to the goal
    }

    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

        List<Node> OpenList = new List<Node>();//List of nodes for the open list
        HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

        OpenList.Add(StartNode); //开始OpenList中只有一个起始点

        while (OpenList.Count > 0) //Whilst there is something in the open list
        {
            Node CurrentNode = OpenList[0]; //从OpenList中取出第一个元素
            for (int i = 1; i < OpenList.Count; i++) //Loop through the open list starting from the second object
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].ihCost < CurrentNode.ihCost) //If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i]; // 找到一个总价值更低的，继续迭代。
                }
            }

            OpenList.Remove(CurrentNode); //每次迭代OpenList中移除当前点
            ClosedList.Add(CurrentNode);  //加入到ClosedList

            if (CurrentNode == TargetNode) // 遍历抵达终点
            {
                GetFinalPath(StartNode, TargetNode); //计算结束，获得最终路径
            }

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
            {
                if (!NeighborNode.IsWall || ClosedList.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
                {
                    continue; //跳过遮挡物
                }

                int MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor
                if (MoveCost < NeighborNode.igCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    NeighborNode.igCost = MoveCost;//Set the g cost to the f cost
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                    NeighborNode.ParentNode = CurrentNode;//Set the parent of the node for retracing steps

                    if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                    {
                        OpenList.Add(NeighborNode);//Add it to the list
                    }
                }
            }
        }
    }

    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        List<Node> FinalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.ParentNode;//Move onto its parent node
        }

        FinalPath.Reverse(); //反转列表元素，得到正确的顺序

        GridReference.FinalPath = FinalPath; //得到最终路径
    }

    // 曼哈顿距离，计算目标点到当前点的代价
    int GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.gridX - a_nodeB.gridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.gridY - a_nodeB.gridY);//y1-y2
        return ix + iy; //路径只沿水平，垂直方向
    }
}