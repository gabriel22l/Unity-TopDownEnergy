using UnityEngine;
using System;
using System.Collections.Generic;
public class CraftingViewModel
{
    public CraftingController CraftingController { get; private set; }

    //Inventory changed -> model.OnDataChanged -> this.OnDataChanged invoked -> view updates
    public event Action OnDataChanged;
    
    //subscription, unsubscription & event invocation
    #region Initialization & Event Handling
    public CraftingViewModel(CraftingController craftingC)
    {
        CraftingController = craftingC;
        CraftingController.OnDataChanged += OnControllerDataChange;
    }
    public void Dispose()
    {
        CraftingController.OnDataChanged -= OnControllerDataChange;
    }
    private void OnControllerDataChange()
    {
        OnDataChanged?.Invoke();
    }
    #endregion
    
    #region UI Requests
    //returns a list of CraftingRecipeViewData for all recipes in CraftingController.AvailableRecipes
    public List<CraftingRecipeViewData> GetAllRecipeViewData()
    {
        //return if recipes null or recipe count == 0, add data to list and return list
        List<ItemRecipe> recipes = CraftingController.AvailableRecipes;
        List<CraftingRecipeViewData> viewData = new List<CraftingRecipeViewData>();
        
        if(recipes == null || recipes.Count == 0) return viewData;
        
        for (int i = 0; i < recipes.Count; i++)
        {
            CraftingRecipeViewData data = GetRecipeViewData(recipes[i], i);
            viewData.Add(data);
        }
        
        return viewData;
    }
    
    //Calls CraftinController.TryCraft with the recipe at the provided index, returns result
    public bool RequestCraft(int recipeIndex)
    {
        if(recipeIndex <  0 || recipeIndex >= CraftingController.AvailableRecipes.Count) return false;
        return CraftingController.TryCraft(CraftingController.AvailableRecipes[recipeIndex]);
    }
    #endregion

    #region Helper Methods & Data Getters
    //returns a list of ResourceViewData for the provided list of ResourceCost
    //gets each ResourceViewData from GetResourceViewData()
    private List<ResourceViewData> GetAllResourceViewData(List<ResourceCost> resources)
    {
        //return if null or count == 0, skip resources with null or amount == 0, add data to list and return list
        List<ResourceViewData> viewData = new List<ResourceViewData>();
        if (resources == null || resources.Count == 0) return viewData;
        
        foreach (ResourceCost resource in resources)
        {
            if(resource.amount <= 0 || resource.itemDataSo == null) continue; 
            ResourceViewData data = GetResourceViewData(resource);
            viewData.Add(data);
        }
        return viewData;
    }
    
    //Returns CraftingRecipeViewData for the provided ItemRecipe and index
    //uses GetResourcesViewData to get ResourceViewData list
    private CraftingRecipeViewData GetRecipeViewData(ItemRecipe recipe, int index)
    {
        if(recipe == null) return new CraftingRecipeViewData();
        return new CraftingRecipeViewData
        {
            index = index,
            itemIcon = recipe.icon,
            itemName = recipe.itemResult.itemName,
            outputAmount = recipe.outputAmount,
            canCraft = CraftingController.CanCraft(recipe),
            resourcesViewData = GetAllResourceViewData(recipe.resources)
        };
    }
    
    //Returns ResourceViewData for the provided ResourceCost
    //uses CraftingC.HasResource to set hasEnough bool
    private ResourceViewData GetResourceViewData(ResourceCost resource)
    {
        return new ResourceViewData()
        {
            resourceIcon = resource.itemDataSo.itemIcon,
            requiredAmount = resource.amount,
            hasEnough = CraftingController.HasResource(resource),
        };
    }
    #endregion
}
public struct CraftingRecipeViewData
{
    public int index;
    public Sprite itemIcon;
    public string itemName;
    public int outputAmount;
    public bool canCraft;
    public List<ResourceViewData> resourcesViewData;
}
public struct ResourceViewData
{
    public Sprite resourceIcon;
    public int requiredAmount;
    public bool hasEnough;
}