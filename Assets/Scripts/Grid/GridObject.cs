using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    public Vector2Int gridPosition;
    public BuildingSO buildingData;
    public GridObject ref_grid;

    public GridObject(Vector2Int _gridPosition)
    {
        this.ref_grid = null;
        this.gridPosition = _gridPosition;
    }

    public void ClearGridObjectData()
    {
        this.ref_grid = null;
        this.buildingData = null;
    }
    
    public void SetRefGrid(GridObject headGrid)
    {
        this.ref_grid = headGrid;   
    }

    public void SetBuldingDataOfHeadGrid(BuildingSO data)
    {
        this.buildingData = data;
    }

    public override string ToString()
    {
        return $"GridObject at ({gridPosition.x}, {gridPosition.y})";
    }
}
