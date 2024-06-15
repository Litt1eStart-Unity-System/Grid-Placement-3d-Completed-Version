using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private Color defaultCellColor = Color.white;
    [SerializeField] private Color defaultFontColor = Color.white;
    [SerializeField] private Color errorCellColor = Color.red;
    public Color usedCellColor = Color.green;
    [SerializeField] private Color usedFontColor = Color.green;

    private Dictionary<Vector2Int, LineRenderer> lineRenderers;
    private Dictionary<Vector2Int, LineRenderer> overlayRenderers;
    private Dictionary<Vector2Int, TextMeshPro> cellTexts;
    private GridSystem gridSystem;

    private float cellSize;
    public  bool isSpawnNewOverlayModel;
    private GameObject currentOverlayModel;
    private GameObject cellParent;
    public void SetVisualizer(int size, GridSystem gridSystem, float cellSize)
    {
        lineRenderers = new Dictionary<Vector2Int, LineRenderer>(size);
        overlayRenderers = new Dictionary<Vector2Int, LineRenderer>(size);
        cellTexts = new Dictionary<Vector2Int, TextMeshPro>(size);
        cellParent = new GameObject("Cell Parent");
        isSpawnNewOverlayModel = true;
        this.cellSize = cellSize;
        this.gridSystem = gridSystem;
    }

    public void InitGridCell(Vector2Int gridPosition, float cellSize)
    {
        lineRenderers[gridPosition] = CreateNewCell(gridPosition, cellSize);
        cellTexts[gridPosition] = CreateNewText(gridPosition, cellSize);
    }

    public void SpawnOverlayModel(Vector3 mousePosition, BuildingSO buildingSO, PlacementDirection direction)
    {
        if (!isSpawnNewOverlayModel) return;

        GameObject existeingOverlayPrefabOnScene = GameObject.FindGameObjectWithTag("OverlayPrefab");
        if(existeingOverlayPrefabOnScene!=null)
            Destroy(existeingOverlayPrefabOnScene);
        else
        {
            GameObject go = Instantiate(buildingSO.overlayPrefab, mousePosition, Quaternion.identity);
            go.tag = "OverlayPrefab";
            go.GetComponent<BuildingOverlayModel>().SetBuildingModelInfo(direction, cellSize);
            currentOverlayModel = go;
            AdjustOverlayModelPositionAndRotation(currentOverlayModel, mousePosition, direction, cellSize);
            isSpawnNewOverlayModel = false;
        }
    }

    


    public void VisualizeOverlayGridCell(Vector2Int gridPosition, BuildingSO buildingSO, PlacementDirection direction, bool isDisplayModel)
    {
        ClearAllCell();

        Vector2Int buildingSize = buildingSO == null ? new Vector2Int(1, 1) : buildingSO.Size;
        bool canBuild = CanBuild(gridPosition, buildingSize, direction);
        Color cellColor = canBuild ? usedCellColor : errorCellColor;

        if (isDisplayModel)
        {
            currentOverlayModel.GetComponent<BuildingOverlayModel>()?.SetOverlayShader(canBuild);
            return;
        }
        else
        {
            switch (direction)
            {
                case PlacementDirection.UP:
                    for (int z = gridPosition.y; z < gridPosition.y + buildingSize.x; z++)
                    {
                        for (int x = gridPosition.x; x > gridPosition.x - buildingSize.y; x--)
                        {
                            UpdateCellData(new Vector2Int(x, z), cellColor, buildingSO);
                        }
                    }
                    break;
                case PlacementDirection.DOWN:
                    for (int z = gridPosition.y; z > gridPosition.y - buildingSize.x; z--)
                    {
                        for (int x = gridPosition.x; x < gridPosition.x + buildingSize.y; x++)
                        {
                            UpdateCellData(new Vector2Int(x, z), cellColor, buildingSO);
                        }
                    }
                    break;
                case PlacementDirection.LEFT:
                    for (int x = gridPosition.x; x > gridPosition.x - buildingSize.x; x--)
                    {
                        for (int z = gridPosition.y; z > gridPosition.y - buildingSize.y; z--)
                        {
                            UpdateCellData(new Vector2Int(x, z), cellColor, buildingSO);
                        }
                    }
                    break;
                case PlacementDirection.RIGHT:
                    for (int x = gridPosition.x; x < gridPosition.x + buildingSize.x; x++)
                    {
                        for (int z = gridPosition.y; z < gridPosition.y + buildingSize.y; z++)
                        {
                            UpdateCellData(new Vector2Int(x, z), cellColor, buildingSO);
                        }
                    }
                    break;
            }
        }
    }

    public void ChangePlacementDirectionOfOverlayModel(PlacementDirection direction)
    {
        if (currentOverlayModel == null) return;

        currentOverlayModel.GetComponent<BuildingOverlayModel>().SetBuildingModelInfo(direction, cellSize);
        AdjustOverlayModelPositionAndRotation(currentOverlayModel, currentOverlayModel.transform.position, direction, cellSize);
    }
    public void UpdateCellData(Vector2Int gridPosition, Color cellColor, BuildingSO buildingSO)
    {
        if (gridSystem.GetGridObjectByGridPosition(gridPosition).ref_grid != null) return;
        
        if (lineRenderers.ContainsKey(gridPosition))
        {
            lineRenderers[gridPosition].startWidth = .5f;
            lineRenderers[gridPosition].endWidth = .5f;
            lineRenderers[gridPosition].startColor = cellColor;
            lineRenderers[gridPosition].endColor = cellColor;
        }

        if (cellTexts.ContainsKey(gridPosition))
        {
            cellTexts[gridPosition].text = buildingSO.DisplayName;
        }
        
    }

    public void ClearAllCell()
    {
        foreach (var kvp in lineRenderers)
        {
            if (gridSystem.GetGridObjectByGridPosition(kvp.Key).ref_grid != null) continue;

            kvp.Value.startColor = defaultCellColor;
            kvp.Value.endColor = defaultCellColor;
            kvp.Value.startWidth = .1f;
            kvp.Value.endWidth = .1f;
        }

        foreach (var kvp in cellTexts)
        {
            kvp.Value.text = $"({kvp.Key.x},{kvp.Key.y})";
        }
    }
    private TextMeshPro CreateNewText(Vector2Int gridPosition, float cellSize)
    {
        GameObject textObject = new GameObject($"CellInfo: ({gridPosition.x},{gridPosition.y})");
        textObject.transform.parent = cellParent.transform;

        Vector3 cellStart = gridSystem.GetWorldPositionFromGridPosition(gridPosition);
        Vector3 cellCenter = cellStart + new Vector3(cellSize/ 2, 0, cellSize / 2); 

        TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();
        
        RectTransform rectTransform = tmp.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = $"({gridPosition.x},{gridPosition.y})";
        tmp.fontSize = CalculateFontSize(cellSize);
        tmp.color = defaultCellColor;
        tmp.transform.position = cellCenter + new Vector3(0, .1f, 0);
        tmp.transform.eulerAngles = new Vector3(90, 0, 0);

        return tmp;
    }

    private float CalculateFontSize(float cellSize)
    {
        float baselineCellSize = 20f;
        float baselineFontSize = 45f;
        float scaleFactor = baselineFontSize / baselineCellSize;
        float fontSize = cellSize * scaleFactor;

        fontSize = Mathf.Clamp(fontSize, 1, 30);
        return fontSize;
    }

    private LineRenderer CreateNewCell(Vector2Int gridPosition, float cellSize)
    {
        GameObject lineObject = new GameObject($"Cell: ({gridPosition.x},{gridPosition.y})");
        lineObject.transform.parent = cellParent.transform;
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 5;

        int x = gridPosition.x;
        int z = gridPosition.y;
        float yTreshold = .1f;

        Vector3 startPos = new Vector3(x * cellSize, yTreshold, z * cellSize);
        Vector3 secondPos = new Vector3(x * cellSize, yTreshold, z * cellSize + cellSize);
        Vector3 thirdPos = new Vector3(x * cellSize + cellSize, yTreshold, z * cellSize + cellSize);
        Vector3 fourthPos = new Vector3(x * cellSize + cellSize, yTreshold, z * cellSize);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, secondPos);
        lineRenderer.SetPosition(2, thirdPos);
        lineRenderer.SetPosition(3, fourthPos);
        lineRenderer.SetPosition(4, startPos);
        
        lineRenderer.startColor = defaultCellColor;
        lineRenderer.endColor = defaultCellColor;

        return lineRenderer;
    }

    private bool CanBuild(Vector2Int startPos, Vector2Int buildingSize, PlacementDirection direction)
    {
        if(gridSystem.IsGridPositionOutOfRange(startPos))
            return false;

        switch (direction)
        {
            case PlacementDirection.UP:
                for (int z = startPos.y; z < startPos.y + buildingSize.x; z++)
                {
                    for (int x = startPos.x; x > startPos.x - buildingSize.y; x--)
                    {
                        if (gridSystem.GetGridObjectByGridPosition(new Vector2Int(x, z)).ref_grid != null)
                        {
                            return false;
                        }
                    }
                }
                break;
            case PlacementDirection.DOWN:
                for (int z = startPos.y; z > startPos.y - buildingSize.x; z--)
                {
                    for (int x = startPos.x; x < startPos.x + buildingSize.y; x++)
                    {
                        if (gridSystem.GetGridObjectByGridPosition(new Vector2Int(x, z)).ref_grid != null)
                        {
                            return false;
                        }
                    }
                }
                break;
            case PlacementDirection.LEFT:
                for (int x = startPos.x; x > startPos.x - buildingSize.x; x--)
                {
                    for (int z = startPos.y; z > startPos.y - buildingSize.y; z--)
                    {
                        if (gridSystem.GetGridObjectByGridPosition(new Vector2Int(x, z)).ref_grid != null)
                        {
                            return false;
                        }
                    }
                }
                break;
            case PlacementDirection.RIGHT:
                for (int x = startPos.x; x < startPos.x + buildingSize.x; x++)
                {
                    for (int z = startPos.y; z < startPos.y + buildingSize.y; z++)
                    {
                        if (gridSystem.GetGridObjectByGridPosition(new Vector2Int(x, z)).ref_grid != null)
                        {
                            return false;
                        }
                    }
                }
                break;
        }

        return true;
    }
    public void SpawnBuildingModel(Vector2Int currnetGridPosition, BuildingSO buildingData, PlacementDirection direction)
    {
        GameObject go = Instantiate(buildingData.prefab, gridSystem.GetWorldPositionFromGridPosition(currnetGridPosition), Quaternion.identity);

        HandleModelRotationBasedOnDirection(go, direction);
    }

    private void HandleModelRotationBasedOnDirection(GameObject go, PlacementDirection direction)
    {
        switch (direction)
        {
            case PlacementDirection.RIGHT:
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
                go.transform.position += new Vector3(0, 0, 0);
                break;
            case PlacementDirection.DOWN:
                go.transform.rotation = Quaternion.Euler(0, 90, 0);
                go.transform.position += new Vector3(0, 0, cellSize);
                break;
            case PlacementDirection.LEFT:
                go.transform.rotation = Quaternion.Euler(0, 180, 0);
                go.transform.position += new Vector3(cellSize, 0, cellSize);
                break;
            case PlacementDirection.UP:
                go.transform.rotation = Quaternion.Euler(0, -90, 0);
                go.transform.position += new Vector3(cellSize, 0, 0);
                break;
        }
    }

    private void AdjustOverlayModelPositionAndRotation(GameObject overlayModel, Vector3 position, PlacementDirection direction, float cellSize)
    {
        Vector3 adjustedPosition = position;

        switch (direction)
        {
            case PlacementDirection.RIGHT:
                overlayModel.transform.rotation = Quaternion.Euler(0, 0, 0);
                adjustedPosition += new Vector3(0, 0, 0);
                break;
            case PlacementDirection.DOWN:
                overlayModel.transform.rotation = Quaternion.Euler(0, 90, 0);
                adjustedPosition += new Vector3(cellSize / 2, 0, -cellSize / 2);
                break;
            case PlacementDirection.LEFT:
                overlayModel.transform.rotation = Quaternion.Euler(0, 180, 0);
                adjustedPosition += new Vector3(-cellSize / 2, 0, -cellSize / 2);
                break;
            case PlacementDirection.UP:
                overlayModel.transform.rotation = Quaternion.Euler(0, -90, 0);
                adjustedPosition += new Vector3(-cellSize / 2, 0, cellSize / 2);
                break;
        }

        overlayModel.transform.position = adjustedPosition;
    }

}
