using System;
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    [SerializeField] private GridCellSelection gridCellSelection;

    private GridManager grid;
    private void Start()
    {
        grid = World.Instance?.GridManager;
        if(grid == null) Debug.LogWarning("null GridManager or World.Instance");
        
        if(gridCellSelection == null) Debug.LogWarning("null GridCellSelection");
    }
    public bool TryPlace(ItemData itemData, int amount)
    {
        if(grid == null || gridCellSelection == null) return false;
        
        Vector2Int targetCell = gridCellSelection.CurrentCell;
        
        bool valid = grid.ValidatePosition(targetCell);
        if (!valid) return false;
        
        Vector3 cellBottom = grid.GetCellBottomCenterWorld(targetCell);
        ItemDropUtility.DropItem(itemData, amount, cellBottom);
        return true;
    }
    public bool TryPlaceInFirstValidPos(ItemData itemData, int amount)
    {
        if(grid == null || gridCellSelection == null) return false;
        
        Vector2Int currentCell = gridCellSelection.CurrentCell;

        Vector2Int target = grid.GetFirstEmptyCell(currentCell);
        if (!grid.ValidatePosition(target)) return false;

        Vector3 cellBottom = grid.GetCellBottomCenterWorld(target);
        ItemDropUtility.DropItem(itemData, amount, cellBottom);
        return true;
    }
    public bool TryPlaceInNearestRandomPos(ItemData itemData, int amount)
    {
        if(grid == null || gridCellSelection == null) return false;
        
        Vector2Int currentCell = gridCellSelection.CurrentCell;

        Vector2Int target = grid.FindNearestRandomEmptyCell(currentCell);
        if (!grid.ValidatePosition(target)) return false;

        Vector3 cellBottom = grid.GetCellBottomCenterWorld(target);
        ItemDropUtility.DropItem(itemData, amount, cellBottom);
        return true;
    }
}
