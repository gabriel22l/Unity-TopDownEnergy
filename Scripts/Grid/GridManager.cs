using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    private Dictionary<Vector2Int, GridOccupant> occupants = new Dictionary<Vector2Int, GridOccupant>();
    private Vector2Int[] directions = new[]
    {
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
    };
    public event Action OnGridChanged;

    public bool RegisterGridOccupant(Vector2Int pos, GridOccupant occupant)
    {
        if(!ValidatePosition(pos ,occupant.GetComponent<Collider2D>())) return false;
        occupants[pos] = occupant;
        OnGridChanged?.Invoke();
        return true;
    }
    public void RemoveGridOccupant(Vector2Int pos)
    {
        occupants.Remove(pos);
        OnGridChanged?.Invoke();
    }
    public Vector2Int WorldToCell(Vector3 pos)
    {
        Vector3Int cell = grid.WorldToCell(pos);
        return new Vector2Int(cell.x, cell.y);
    }
    public Vector3 GetCellCenterWorld(Vector2Int pos)
    {
        return grid.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
    }
    public Vector3 GetCellBottomCenterWorld(Vector2Int pos)
    {
        Vector3 center = grid.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
        center.y -= grid.cellSize.y / 2;
        return center;
    }
    // Checks if occupants contains the position key
    // Checks if there is any collider that is not a trigger or the provided ignore collider
    // from cell center
    public bool ValidatePosition(Vector2Int pos, Collider2D ignore = null)
    {
        if (occupants.ContainsKey(pos)) return false;
        
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
        Vector2 physicsBoxSize = new Vector2(grid.cellSize.x, grid.cellSize.y) * 0.9f;
        Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, physicsBoxSize, 0);
        // ignore triggers and registration collider if provided
        foreach (Collider2D hit in hits)
        {
            if (hit.isTrigger) continue;
            if (ignore != null && hit == ignore) continue;
            return false;
        }
        return true;
    }
    public Vector2Int GetFirstEmptyCell(Vector2Int initialPos)
    {
        if(ValidatePosition(initialPos)) return initialPos;
        int maxRadius = 10;
        for (int r = 1; r <= maxRadius; r++)
        {
            // Vertically middle cells, when r = 1, y = 0
            for (int y = -r + 1; y <= r - 1; y++)
            {
                Vector2Int left = new Vector2Int(initialPos.x - r, initialPos.y + y);
                Vector2Int right = new Vector2Int(initialPos.x + r, initialPos.y + y);
                if (ValidatePosition(left)) return left;
                if (ValidatePosition(right)) return right;
            }
            // Top/bottom rows, when r = 1, x = -1, 0, 1
            for (int x = -r; x <= r; x++)
            {
                Vector2Int top = new Vector2Int(initialPos.x + x, initialPos.y + r);
                Vector2Int bottom = new Vector2Int(initialPos.x + x, initialPos.y - r);
                if (ValidatePosition(top)) return top;
                if (ValidatePosition(bottom)) return bottom;
            }
        }
        return initialPos;
    }
    
    public Vector2Int FindNearestRandomEmptyCell(Vector2Int initialPos)
    {
        if(ValidatePosition(initialPos)) return initialPos;
        int maxRadius = 10;
        
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int r = 1; r <= maxRadius; r++)
        {
            positions.Clear();
            // Vertically middle cells, when r = 1, y = 0
            for (int y = -r + 1; y <= r - 1; y++)
            {
                Vector2Int left = new Vector2Int(initialPos.x - r, initialPos.y + y);
                Vector2Int right = new Vector2Int(initialPos.x + r, initialPos.y + y);
                positions.Add(left);
                positions.Add(right);
            }
            // Top/bottom rows, when r = 1, x = -1, 0, 1
            for (int x = -r; x <= r; x++)
            {
                Vector2Int top = new Vector2Int(initialPos.x + x, initialPos.y + r);
                Vector2Int bottom = new Vector2Int(initialPos.x + x, initialPos.y - r);
                positions.Add(top);
                positions.Add(bottom);
            }
            int randomIndex = Random.Range(0, positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int pos = positions[(randomIndex + i) % positions.Count];

                if (ValidatePosition(pos))
                    return pos;
            }
        }
        return initialPos;
    }

    [ContextMenu("Log occupants")]
    public void LogOccupants()
    {
        string log = string.Empty;
        int i = 0;
        foreach (var (pos, occupant) in occupants)
        {
            if(occupant == null) continue;
            log += $"{i}. {pos} : {occupant}\n";
            i++;
        }
        Debug.Log(log);
    }
}