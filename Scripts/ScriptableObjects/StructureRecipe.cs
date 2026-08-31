using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureRecipe", menuName = "Scriptable Objects/StructureRecipe")]
public class StructureRecipe : ScriptableObject
{
    public int recipeID;
    public StructureData structureResult;
    public Sprite icon;
    public List<ResourceCost> resources;
    public float energyCost;
}