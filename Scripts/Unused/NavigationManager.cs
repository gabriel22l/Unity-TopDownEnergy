using UnityEngine;
using NavMeshPlus.Components;
using System;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private NavMeshSurface navigationSurface;

    private void OnEnable()
    {
        if(gridManager != null)
        {
            gridManager.OnGridChanged += UpdateSurface;
        }
    }
    private void OnDisable()
    {
        if (gridManager != null)
        {
            gridManager.OnGridChanged -= UpdateSurface;
        }
    }
    private void UpdateSurface()
    {
        navigationSurface.UpdateNavMesh(navigationSurface.navMeshData);
        Debug.Log("Update NavMesh");
    }
}
