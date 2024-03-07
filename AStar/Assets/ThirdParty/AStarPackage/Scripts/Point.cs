public class Point
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public Point parent = null;

    public bool isObstacle = false;

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