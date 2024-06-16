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
    public GridSystem GridSystem => gridSystem;

    private void Start()
    {
        gridSystem = GameManager.Instance.gridSystem;
        visualizer = gridSystem.Visualizer;
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
                        Vector2Int startPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                        gridSystem.PlaceDataOnGrid(startPos, buildingData, direction);
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
    private void ChangePlacementDirection()
    {

        switch (direction)
        {
            case PlacementDirection.RIGHT:
                direction = PlacementDirection.DOWN;
                break;
            case PlacementDirection.DOWN:
                direction = PlacementDirection.LEFT;
                break;
            case PlacementDirection.LEFT:
                direction = PlacementDirection.UP;
                break;
            case PlacementDirection.UP:
                direction = PlacementDirection.RIGHT;
                break;
            default:
                break;
        }

        // Update the visualizer with the new direction
        visualizer.ChangePlacementDirectionOfOverlayModel(direction);
        GameManager.Instance.SetPlacementDirection(direction);
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
