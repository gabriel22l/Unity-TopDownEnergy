using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemRecipe", menuName = "Scriptable Objects/ItemRecipe")]
public class ItemRecipe : ScriptableObject
{
    public int recipeID;
    public CraftingStationType requiredStation;
    public ItemData itemResult;
    public int outputAmount = 1;
    public Sprite icon;
    public List<ResourceCost> resources;
}

public enum CraftingStationType
{
    None,
}
