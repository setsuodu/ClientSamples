public class Point
{
    public int X; //自身横坐标
    public int Y; //自身纵坐标
    public int F; //总代价 G+H
    public int G; //临近代价
    public int H; //终点代价
    public Point parent = null; //记录路径上的前一个Node

    public bool isObstacle = false; //是否是遮挡物

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetParent(Point parent, int g)
    {
        this.parent = parent;
        G = g;
        F = G + H;
    }
}