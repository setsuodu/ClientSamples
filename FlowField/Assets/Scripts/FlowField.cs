using UnityEngine;

public class FlowField
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; } //°ë¾¶

    private float cellDiameter; //Ö±¾¶

    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        this.cellRadius = _cellRadius;
        this.cellDiameter = this.cellRadius * 2;
        this.gridSize = _gridSize;
    }

    public void CreateGrid()
    {
        this.grid = new Cell[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
                this.grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }
}