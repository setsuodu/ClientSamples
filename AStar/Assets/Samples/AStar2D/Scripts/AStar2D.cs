using System.Collections.Generic;
using UnityEngine;

public class AStar2D : MonoBehaviour
{
    public const int width = 10;
    public const int height = 10;

    public Grid2D[,] map = new Grid2D[height, width];
    public GridView2D[,] sprites = new GridView2D[height, width];//图片和结点一一对应

    public GameObject prefab;   //代表结点的图片
    public Grid2D start;
    public Grid2D end;

    public Color lineColor = Color.gray;
    public Color obstacleColor = Color.black;
    public Color startColor = Color.green;
    public Color endColor = Color.red;

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

    // 初始化地图
    public void InitMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sprites[i, j] = Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity).GetComponent<GridView2D>();
                map[i, j] = new Grid2D(i, j);
            }
        }
    }

    // 添加障碍
    public void AddObstacle(int x, int y)
    {
        map[x, y].isObstacle = true;
        sprites[x, y].spriteRenderer.color = obstacleColor;
    }

    // 设置起点和终点
    public void SetStartAndEnd(int startX, int startY, int endX, int endY)
    {
        start = map[startX, startY];
        sprites[startX, startY].spriteRenderer.color = startColor;
        end = map[endX, endY];
        sprites[endX, endY].spriteRenderer.color = endColor;
    }

    public void FindPath()
    {
        List<Grid2D> openList = new List<Grid2D>();
        List<Grid2D> closeList = new List<Grid2D>();
        openList.Add(start);
        while (openList.Count > 0)//只要开放列表还存在元素就继续
        {
            Grid2D point = GetMinFOfList(openList);//选出open集合中代价最小的
            openList.Remove(point);
            closeList.Add(point);
            List<Grid2D> SurroundPoints = GetSurroundPoint(point.X, point.Y); //获取周围点

            foreach (Grid2D p in closeList)//在周围点中把已经在关闭列表的点删除
            {
                if (SurroundPoints.Contains(p))
                {
                    SurroundPoints.Remove(p);
                }
            }

            foreach (Grid2D p in SurroundPoints)//遍历周围的点
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

    // 显示路径
    public void ShowPath()
    {
        Grid2D temp = end.parent;
        while (temp != start)
        {
            sprites[temp.X, temp.Y].spriteRenderer.color = lineColor;
            temp = temp.parent;
        }
    }

    // 得到一个点周围的点 2/3/4 个
    public List<Grid2D> GetSurroundPoint(int x, int y)
    {
        List<Grid2D> PointList = new List<Grid2D>();
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

    // 计算该点总代价
    public void GetF(Grid2D point)
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

    // 筛选总代价最小的点
    public Grid2D GetMinFOfList(List<Grid2D> list)
    {
        int min = int.MaxValue;
        Grid2D point = null;
        foreach (Grid2D p in list)
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