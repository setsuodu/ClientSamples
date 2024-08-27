using UnityEngine;

public class Cell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public byte cost;

    public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
    {
        this.worldPos = _worldPos;
        this.gridIndex = _gridIndex;
    }
}