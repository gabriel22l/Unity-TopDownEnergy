using System;
using UnityEngine;

public class ItemPhysics : MonoBehaviour
{
    private GridManager gridManager;
    private bool isMoving;
    private void Start()
    {
        gridManager = World.Instance?.GridManager;
        if (gridManager == null)
        {
            Debug.LogWarning("ItemPhysics could not find GridManager");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.isTrigger || isMoving)
            return;
        HandleMovement();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.isTrigger)
            return;
        isMoving = false;
        
    }
    // Gets first empty cell and moves to it
    private void HandleMovement()
    {
        isMoving = true;
        if(gridManager == null) return;
        Vector2Int currentCell = gridManager.WorldToCell(transform.position);
        Vector2Int targetCell = gridManager.GetFirstEmptyCell(currentCell);
        if (targetCell == currentCell) return;
        
        Vector3 targetPosition = gridManager.GetCellBottomCenterWorld(targetCell);
        transform.position = targetPosition;
    }
}