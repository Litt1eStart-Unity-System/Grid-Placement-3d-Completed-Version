using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementDirection
{
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
    [SerializeField] private int placementIndex = 0;

    private BuildingSO buildingData;
    private GridSystem gridSystem;
    public GridSystem GridSystem => gridSystem;

    private void Start()
    {
        //gridSystem = new GridSystem(GameManager.Instance.gridWidth, GameManager.Instance.gridDepth, GameManager.Instance.cellSize, visualizer);
        gridSystem = GameManager.Instance.gridSystem;
        visualizer = gridSystem.Visualizer;
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        switch (gridOperator)
        {
            case GridOperator.PLACEMENT:
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
                        Debug.Log("Current Direction" + direction);
                        Vector2Int startPos = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                        gridSystem.PlaceDataOnGrid(startPos, buildingData, direction);
                    }
                }
                break;
            case GridOperator.DELETE:
                break;
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
        visualizer.ChangePlacementDirectionOfOverlayModel(direction);
        placementIndex++;
    }
}
