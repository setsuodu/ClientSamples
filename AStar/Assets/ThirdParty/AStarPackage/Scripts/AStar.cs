﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public const int width = 10;
    public const int height = 10;

    public Point[,] map = new Point[height, width];
    public SpriteRenderer[,] sprites = new SpriteRenderer[height, width];//图片和结点一一对应

    public GameObject prefab;   //代表结点的图片
    public Point start;
    public Point end;

    void Start()
    {
        InitMap();
        //测试代码
        AddObstacle(2, 4);
        AddObstacle(2, 3);
        AddObstacle(2, 2);
        AddObstacle(2, 0);
        AddObstacle(6, 4);
        AddObstacle(8, 4);
        SetStartAndEnd(0, 0, 7, 7);
        FindPath();
        ShowPath();
    }

    public void InitMap()//初始化地图
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sprites[i, j] = Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity).GetComponent<SpriteRenderer>();
                map[i, j] = new Point(i, j);
            }
        }
    }

    public void AddObstacle(int x, int y)//添加障碍
    {
        map[x, y].isObstacle = true;
        sprites[x, y].color = Color.black;
    }

    public void SetStartAndEnd(int startX, int startY, int endX, int endY)//设置起点和终点
    {
        start = map[startX, startY];
        sprites[startX, startY].color = Color.green;
        end = map[endX, endY];
        sprites[endX, endY].color = Color.red;
    }

    public void ShowPath()//显示路径
    {
        Point temp = end.parent;
        while (temp != start)
        {
            sprites[temp.X, temp.Y].color = Color.gray;
            temp = temp.parent;
        }
    }

    public void FindPath()
    {
        List<Point> openList = new List<Point>();
        List<Point> closeList = new List<Point>();
        openList.Add(start);
        while (openList.Count > 0)//只要开放列表还存在元素就继续
        {
            Point point = GetMinFOfList(openList);//选出open集合中F值最小的点
            openList.Remove(point);
            closeList.Add(point);
            List<Point> SurroundPoints = GetSurroundPoint(point.X, point.Y);

            foreach (Point p in closeList)//在周围点中把已经在关闭列表的点删除
            {
                if (SurroundPoints.Contains(p))
                {
                    SurroundPoints.Remove(p);
                }
            }

            foreach (Point p in SurroundPoints)//遍历周围的点
            {
                if (openList.Contains(p))//周围点已经在开放列表中
                {
                    //重新计算G,如果比原来的G更小,就更改这个点的父亲
                    int newG = 1 + point.G;
                    if (newG < p.G)
                    {
                        p.SetParent(point, newG);
                    }
                }
                else
                {
                    //设置父亲和F并加入开放列表
                    p.parent = point;
                    GetF(p);
                    openList.Add(p);
                }
            }
            if (openList.Contains(end))//只要出现终点就结束
            {
                break;
            }
        }
    }

    public List<Point> GetSurroundPoint(int x, int y)//得到一个点周围的点
    {
        List<Point> PointList = new List<Point>();
        if (x > 0 && !map[x - 1, y].isObstacle)
        {
            PointList.Add(map[x - 1, y]);
        }
        if (y > 0 && !map[x, y - 1].isObstacle)
        {
            PointList.Add(map[x, y - 1]);
        }
        if (x < height - 1 && !map[x + 1, y].isObstacle)
        {
            PointList.Add(map[x + 1, y]);
        }
        if (y < width - 1 && !map[x, y + 1].isObstacle)
        {
            PointList.Add(map[x, y + 1]);
        }
        return PointList;
    }

    public void GetF(Point point)//计算某个点的F值
    {
        int G = 0;
        int H = Mathf.Abs(end.X - point.X) + Mathf.Abs(end.Y - point.Y);
        if (point.parent != null)
        {
            G = 1 + point.parent.G;
        }
        int F = H + G;
        point.H = H;
        point.G = G;
        point.F = F;
    }

    public Point GetMinFOfList(List<Point> list)//得到一个集合中F值最小的点
    {
        int min = int.MaxValue;
        Point point = null;
        foreach (Point p in list)
        {
            if (p.F < min)
            {
                min = p.F;
                point = p;
            }
        }
        return point;
    }
}