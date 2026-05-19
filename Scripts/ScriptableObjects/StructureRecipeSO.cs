using DefaultNamespace;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureRecipeSO", menuName = "Scriptable Objects/StructureRecipeSO")]
public class StructureRecipeSO : ScriptableObject
{
    public int recipeID;
    public StructureDataSO structureResult;
    public Sprite icon;
    public List<ResourceCost> resources;
    public float energyCost;
}