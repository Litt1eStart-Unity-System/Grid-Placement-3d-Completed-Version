using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlacementDirection
{
    NONE,
    RIGHT,
    DOWN,
    LEFT,
    UP,
}

public enum GridOperator
{
    PLACEMENT,
    DELETE
}
public class GridHolder : MonoBehaviour
{
    [SerializeField] private GridVisualizer visualizer;
    [SerializeField] private PlacementDirection direction;
    [SerializeField] private GridOperator gridOperator;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private int placementIndex = 0;

    private BuildingSO buildingData;
    private GridSystem gridSystem;

    private bool isDragging = false;
    private List<Vector2Int> dragPath;
    private Vector2Int dragStartPos;
    private Vector2Int dragEndPos;
    public GridSystem GridSystem => gridSystem;

    private void Start()
    {
        //gridSystem = new GridSystem(GameManager.Instance.gridWidth, GameManager.Instance.gridDepth, GameManager.Instance.cellSize, visualizer);
        gridSystem = GameManager.Instance.gridSystem;
        visualizer = gridSystem.Visualizer;
        dragPath = new List<Vector2Int>();
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        gridOperator = GameManager.Instance.currentOperator;
        switch (gridOperator)
        {
            case GridOperator.PLACEMENT:
                if (IsPointerOverUIObject()) return;

                buildingData = GameManager.Instance.buildingData;
                if (buildingData == null) return;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    Vector3 hitPosition = hitInfo.point;
                    Vector2Int hitGridPosition = gridSystem.GetGridPositionFromWorldPosition(hitPosition);

                    Debug.Log("OverlayPrefab name: " + buildingData.overlayPrefab.name);
                    visualizer.SpawnOverlayModel(hitPosition, buildingData, direction);
                    visualizer.VisualizeOverlayGridCell(hitGridPosition, buildingData, direction, true);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    ChangePlacementDirection();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        Debug.Log("Current Direction" + direction);
                        Vector2Int startPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                        gridSystem.PlaceDataOnGrid(startPos, buildingData, direction);
                    }
                }

                if(buildingData.Size == new Vector2Int(1, 1))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            isDragging = true;
                            dragStartPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                        }
                    }

                    if (Input.GetMouseButton(0) && isDragging)
                    {
                        if(Physics.Raycast(ray, out hitInfo))
                        {
                            dragEndPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                            UpdateDragPath(dragStartPos, dragEndPos);
                            visualizer.VisualizeDragPath(dragPath, buildingData, direction);
                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        isDragging = false;
                        visualizer.ClearDragPath();
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            dragEndPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                            PlaceRoadAlongPath(dragStartPos, dragEndPos);
                        }
                    }
                }
                
                break;

            case GridOperator.DELETE:
                visualizer.ClearOverlayModel();
                if (Input.GetMouseButtonDown(1))
                {
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, buildingLayer))
                    {
                        Vector3 hitPosition = hitInfo.point;
                        Vector2Int hitGridPosition = gridSystem.GetGridPositionFromWorldPosition(hitPosition);

                        gridSystem.DeleteBuildingOnGrid(hitGridPosition);
                        Destroy(hitInfo.collider.gameObject);
                    }
                }
                break;
        }
        
    }

    private void UpdateDragPath(Vector2Int startPos, Vector2Int endPos)
    {
        dragPath.Clear();

        if (startPos.y == endPos.y)
        {
            int minX = Mathf.Min(startPos.x, endPos.x);
            int maxX = Mathf.Max(startPos.x, endPos.x);
            for (int x = minX; x <= maxX; x++)
            {
                dragPath.Add(new Vector2Int(x, startPos.y));
            }
        }
        else if (startPos.x == endPos.x)
        {
            int minY = Mathf.Min(startPos.y, endPos.y);
            int maxY = Mathf.Max(startPos.y, endPos.y);
            for (int y = minY; y <= maxY; y++)
            {
                dragPath.Add(new Vector2Int(startPos.x, y));
            }
        }
    }

    private void PlaceRoadAlongPath(Vector2Int startPos, Vector2Int endPos)
    {
        Debug.Log($"StartDragPos: {startPos}, EndDragPos: {endPos}");
        if (startPos.y == endPos.y) 
        {
            int minX = Mathf.Min(startPos.x, endPos.x); 
            int maxX = Mathf.Max(startPos.x, endPos.x);
            for (int x = minX; x <= maxX; x++)
            {
                gridSystem.PlaceDataOnGrid(new Vector2Int(x, startPos.y), buildingData, direction);
            }
        }
        else if (startPos.x == endPos.x)
        {
            int minY = Mathf.Min(startPos.y, endPos.y);
            int maxY = Mathf.Max(startPos.y, endPos.y);
            for (int y = minY; y <= maxY; y++)
            {
                gridSystem.PlaceDataOnGrid(new Vector2Int(startPos.x, y), buildingData, direction);
            }
        }
    }
    private void ChangePlacementDirection()
    {
        if (placementIndex >= 4)
            placementIndex = 0;

        switch (placementIndex)
        {
            case 0:
                GameManager.Instance.SetPlacementDirection(PlacementDirection.RIGHT);
                break;
            case 1:
                GameManager.Instance.SetPlacementDirection(PlacementDirection.DOWN);
                break;
            case 2:
                GameManager.Instance.SetPlacementDirection(PlacementDirection.LEFT);
                break;
            case 3:
                GameManager.Instance.SetPlacementDirection(PlacementDirection.UP);
                break;
            default:
                break;
        }

        direction = GameManager.Instance.direction;
        Debug.Log("Changed Direction to: " + direction);
        visualizer.ChangePlacementDirectionOfOverlayModel(direction);
        placementIndex++;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results); 
        return results.Count > 0;
    }
}
