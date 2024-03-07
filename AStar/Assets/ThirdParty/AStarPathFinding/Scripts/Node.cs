using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int gridX; //X Position in the node Array
    public int gridY; //Y Position in the node Array

    public bool IsWall; //if this node is being obstructed
    public Vector3 Postion; //the world position of the node

    public Node ParentNode; //For the A* algoritm, will store what node it previously came from so it can trace the shortest path.

    public int igCost; //the cost of moving to the next square
    public int ihCost; //the distance to the goal from this node
    public int FCost { get { return igCost + ihCost; } }

    public Node(bool a_IsWall, Vector3 a_Pos, int a_gridX, int a_gridY)
    {
        IsWall = a_IsWall;
        Postion = a_Pos;
        gridX = a_gridX;
        gridY = a_gridY;
    }
}
