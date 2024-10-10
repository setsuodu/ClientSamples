[System.Serializable]
public class Grid2D
{
    public int X; //自身横坐标
    public int Y; //自身纵坐标
    public int F; //总代价 F=G+H
    public int G; //起点到此处的距离
    public int H; //此处距终点的距离
    public Grid2D parent = null; //记录路径上的前一个Point

    public bool isObstacle = false; //是否是遮挡物

    public Grid2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetParent(Grid2D parent, int g)
    {
        this.parent = parent;
        G = g;
        F = G + H;
    }
}