using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private Dictionary<Vector2Int, GridObject> gridDict;
    private GridVisualizer visualizer;
    private float cellSize;

    public GridVisualizer Visualizer => visualizer;
    public Dictionary<Vector2Int, GridObject> GridDict => gridDict;
    public int GridSize => gridDict.Count;
    public GridSystem(int width, int depth, float cellSize, GridVisualizer visualizer)
    {
        if (width < 0 || depth < 0) return;

        this.visualizer = visualizer;
        this.cellSize = cellSize;


        gridDict = new Dictionary<Vector2Int, GridObject>(width * depth);
        visualizer.SetVisualizer(GridSize, this, cellSize);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int gridPosition = new Vector2Int(x, z);
                gridDict[gridPosition] = new GridObject(gridPosition);
                
                if(GameManager.Instance.isDrawnGrid)
                    visualizer.InitGridCell(gridPosition, cellSize);
            }
        }
    }

    public void PlaceDataOnGrid(Vector2Int startPos, BuildingSO buildingData, PlacementDirection direction)
    {
        Vector2Int buildingSize = buildingData.Size;

        if(!CanBuild(startPos, buildingSize, direction)) return;

        GridObject headGrid = GetGridObjectByGridPosition(startPos);
        headGrid.buildingData = buildingData;
        visualizer.SpawnBuildingModel(startPos, buildingData, direction);

        IterateGridPositionsByPlacementDirection(startPos, buildingSize, direction, (x, z) =>
        {
            Vector2Int currnetGridPosition = new Vector2Int(x, z);
            gridDict[currnetGridPosition].ref_grid = headGrid;
            gridDict[currnetGridPosition].direction = direction;
            visualizer.UpdateCellData(currnetGridPosition, visualizer.usedCellColor, gridDict[currnetGridPosition].ref_grid.buildingData);
        });

       
    }

    public void DeleteBuildingOnGrid(Vector2Int clickedPosition)
    {
        GridObject clickedGrid = GetGridObjectByGridPosition(clickedPosition);
        if(clickedGrid==null || clickedGrid.ref_grid==null) return;

        GridObject headGrid = clickedGrid.ref_grid;
        Vector2Int startPos = headGrid.gridPosition;
        Vector2Int buildingSize = headGrid.buildingData.Size;
        PlacementDirection directionOfHeadGrid = headGrid.direction;

        IterateGridPositionsByPlacementDirection(startPos, buildingSize, directionOfHeadGrid, (x, z) =>
        {
            GridObject gridObj = GetGridObjectByGridPosition(new Vector2Int(x, z));
            gridObj.ClearGridObjectData();
        });
    }

    private bool CanBuild(Vector2Int startPos, Vector2Int buildingSize, PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.UP:
                for (int z = startPos.y; z < startPos.y + buildingSize.x; z++)
                {
                    for (int x = startPos.x; x > startPos.x - buildingSize.y; x--)
                    {
                        Vector2Int currnetGridPosition = new Vector2Int(x, z);
                        if (gridDict[currnetGridPosition].ref_grid != null)
                            return false;
                    }
                }
                break;
            case PlacementDirection.DOWN:
                for (int z = startPos.y; z > startPos.y - buildingSize.x; z--)
                {
                    for (int x = startPos.x; x < startPos.x + buildingSize.y; x++)
                    {
                        Vector2Int currnetGridPosition = new Vector2Int(x, z);
                        if (gridDict[currnetGridPosition].ref_grid != null)
                            return false;
                    }
                }
                break;
            case PlacementDirection.LEFT:
                for (int x = startPos.x; x > startPos.x - buildingSize.x; x--)
                {
                    for (int z = startPos.y; z > startPos.y - buildingSize.y; z--)
                    {
                        Vector2Int currnetGridPosition = new Vector2Int(x, z);
                        if (gridDict[currnetGridPosition].ref_grid != null)
                            return false;
                    }
                }
                break;
            case PlacementDirection.RIGHT:
                for (int x = startPos.x; x < startPos.x + buildingSize.x; x++)
                {
                    for (int z = startPos.y; z < startPos.y + buildingSize.y; z++)
                    {
                        Vector2Int currnetGridPosition = new Vector2Int(x, z);
                        if (gridDict[currnetGridPosition].ref_grid != null)
                            return false;
                    }
                }
                break;
        }

        return true;
    }
    private void IterateGridPositionsByPlacementDirection(Vector2Int startPos, Vector2Int buildingSize, PlacementDirection direction, Action<int, int> action)
    {
        switch (direction)
        {
            case PlacementDirection.UP:
                for (int z = startPos.y; z < startPos.y + buildingSize.x; z++)
                {
                    for (int x = startPos.x; x > startPos.x - buildingSize.y; x--)
                    {
                        action(x, z);
                    }
                }
                break;
            case PlacementDirection.DOWN:
                for (int z = startPos.y; z > startPos.y - buildingSize.x; z--)
                {
                    for (int x = startPos.x; x < startPos.x + buildingSize.y; x++)
                    {
                        action(x, z);
                    }
                }
                break;
            case PlacementDirection.LEFT:
                for (int x = startPos.x; x > startPos.x - buildingSize.x; x--)
                {
                    for (int z = startPos.y; z > startPos.y - buildingSize.y; z--)
                    {
                        action(x, z);
                    }
                }
                break;
            case PlacementDirection.RIGHT:
                for (int x = startPos.x; x < startPos.x + buildingSize.x; x++)
                {
                    for (int z = startPos.y; z < startPos.y + buildingSize.y; z++)
                    {
                        action(x, z);
                    }
                }
                break;
        }
    }

    public GridObject GetGridObjectByGridPosition(Vector2Int gridPosition)
    {
        if(gridDict.TryGetValue(gridPosition, out GridObject gridObject))
            return gridObject;

        return null;
    }
    public Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x / cellSize), Mathf.FloorToInt(worldPosition.z / cellSize));
    }

    public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        if (GridDict.ContainsKey(gridPosition))
        {
            return new Vector3(gridPosition.x, 0, gridPosition.y) * cellSize;
        }
        else
        {
            return new Vector3(-1, -1, -1);
        }
    }

    public bool IsGridPositionOutOfRange(Vector2Int gridPosition)
    {
        return gridDict.ContainsKey(gridPosition);
    }
}
