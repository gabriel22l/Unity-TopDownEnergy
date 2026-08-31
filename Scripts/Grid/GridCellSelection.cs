using System;
using UnityEngine;

public class GridCellSelection : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int cellOffset = 2;
    private GridManager gridManager;
    public Vector2Int CurrentCell {get; private set;}

    private void Start()
    {
        gridManager = World.Instance?.GridManager;
        if (gridManager == null)
        {
            Debug.LogWarning("GridManager or World.Instance is null");
        }
    }

    private void Update()
    {
        CalculateTargetCell();
    }
    private void CalculateTargetCell()
    {
        if(!gridManager) return;
        
        //calculate direction and target cell
        Vector2 dropDir = playerMovement != null && playerMovement.FacingDirection != Vector2.zero ?
            playerMovement.FacingDirection * cellOffset:
            Vector2.up * cellOffset;

        Vector2Int standingCell = gridManager.WorldToCell(transform.position);
        Vector2Int facingDir = Vector2Int.RoundToInt(dropDir);
        this.CurrentCell = standingCell + facingDir;
    }
}