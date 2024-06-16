using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BuildingOverlayModel : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private Color defaultColor;
    private Color errorColor;
    private PlacementDirection direction;
    private float cellSize;

    private void Start()
    {
        defaultColor = GameManager.Instance.defaultOverlayColor;
        errorColor = GameManager.Instance.errorOverlayColor;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
        {
            float modelThreshold = cellSize / 2;
            Vector3 hitPosition = Vector3.zero;
            switch (direction)
            {
                case PlacementDirection.RIGHT:
                    hitPosition = new Vector3(hitInfo.point.x - modelThreshold, .5f, hitInfo.point.z - modelThreshold);
                    break;
                case PlacementDirection.DOWN:
                    hitPosition = new Vector3(hitInfo.point.x - modelThreshold, .5f, hitInfo.point.z + modelThreshold);
                    break;
                case PlacementDirection.LEFT:
                    hitPosition = new Vector3(hitInfo.point.x + modelThreshold, .5f, hitInfo.point.z + modelThreshold);
                    break;
                case PlacementDirection.UP:
                    hitPosition = new Vector3(hitInfo.point.x + modelThreshold, .5f, hitInfo.point.z - modelThreshold);
                    break;
            }
            Debug.Log($"Hit Position: {hitInfo.point}, Adjusted Position: {hitPosition}, Direction: {direction}");
            transform.position = hitPosition;
        }
    }

    public void SetBuildingModelInfo(PlacementDirection direction, float cellSize)
    {
        this.direction = direction;
        this.cellSize = cellSize;
        Debug.Log($"SetBuildingModelInfo called with Direction: {direction}, CellSize: {cellSize}");
        UpdatePosition();
    }

    public void SetOverlayShader(bool canBuild)
    {
        Color color = canBuild ? defaultColor : errorColor;
        Debug.Log("OverlayColor: " + canBuild);
        transform.GetComponentInChildren<MeshRenderer>().material.SetColor("_Tint", color);
        transform.GetComponentInChildren<MeshRenderer>().material.SetColor("_FresnalColor", color);
    }
}
