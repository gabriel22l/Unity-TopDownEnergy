using System;
using UnityEngine;

public class GridOccupant : MonoBehaviour
{
    private Vector2Int cellPos;
    private GridManager gridManager;
    private bool isRegistered;

    private void Start()
    {
        if(!TryGetGridManager()) return;
        HandleRegistration();
    }
    // Unregister from grid
    private void OnDestroy()
    {
        if(gridManager == null || !isRegistered) return;
        gridManager.RemoveGridOccupant(this.cellPos);
    }
    private bool TryGetGridManager()
    {
        gridManager = World.Instance?.GridManager;
        if (gridManager == null)
        {
            Debug.LogWarning("World.Instance or World.Instance.GridManager is null ");
            return  false;
        }
        return true;
    }
    private void SnapToGrid(Vector2Int cell)
    {
        Vector3 target = gridManager.GetCellBottomCenterWorld(cell);
        transform.position = target;
    }
    // gets cell position and tries to register to GridManager
    // if successful saves cell position and snaps transform to cell
    private void HandleRegistration()
    {
        if(gridManager == null || isRegistered) return;
        
        Vector2Int cellPosition = gridManager.WorldToCell(transform.position);
        bool success = gridManager.RegisterGridOccupant(cellPosition, this);
        
        if(success)
        {
            this.cellPos = cellPosition;
            SnapToGrid(cellPosition);
            isRegistered = true;
        }
        else
        {
            Debug.LogWarning("grid occupant registration failed");
        }
    }
}