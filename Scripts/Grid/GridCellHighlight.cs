using System;
using UnityEngine;

public class GridCellHighlight : MonoBehaviour
{
    [SerializeField] private GridCellSelection gridCellSelection;
    [SerializeField] private SpriteRenderer highlightRenderer;
    private GridManager gridManager;
    private void Start()
    {
        gridManager = World.Instance?.GridManager;
        if (gridManager == null)
        {
            Debug.LogWarning("gridManager or World.Instance is null");
        }
    }
    private void Update()
    {
        if (!gridCellSelection || !gridManager) return;

        Vector2Int targetCell = gridCellSelection.CurrentCell;
        
        Vector3 cellCenter =  gridManager.GetCellCenterWorld(targetCell);
        
        highlightRenderer.transform.position = cellCenter;
    }
}
