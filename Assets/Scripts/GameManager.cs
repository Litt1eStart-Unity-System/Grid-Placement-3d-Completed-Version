using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int gridWidth;
    public int gridDepth;
    public float cellSize;
    public bool isDrawnGrid;
    public Color defaultOverlayColor;
    public Color errorOverlayColor;

    public BuildingSO buildingData;
    public GridSystem gridSystem {  get; private set; }
    public GridHolder gridHolder;
    public PlacementDirection direction;
    public GridOperator currentOperator;
    [SerializeField] private GridVisualizer visualizer;
    private void Awake()
    {
        Instance = this;
        gridSystem = new GridSystem(gridWidth, gridDepth, cellSize, visualizer);
        direction = PlacementDirection.UP;
    }

    public void SetBuildingData(BuildingSO buildingData)
    {
        this.buildingData = buildingData;
    }

    public void SetPlacementDirection(PlacementDirection direction) 
    {
        this.direction = direction; 
    }

    public void ChangeBuldingData(BuildingSO newBuilding)
    {
        GameManager.Instance.gridSystem.Visualizer.isSpawnNewOverlayModel = true;
        SetBuildingData(newBuilding);
    }
}
