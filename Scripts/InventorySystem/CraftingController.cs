using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftingController : MonoBehaviour
{
    [SerializeField] private CraftingStationType stationType =  CraftingStationType.None;
    [SerializeField] private DropHandler dropHandler;
    private InventoryController inventoryController;
    public List<ItemRecipe> AvailableRecipes { get; private set; }
    
    //Event invoked when inventory changes, used to notify view model to update
    public event Action OnDataChanged; 

    #region Initialization & Unity callbacks
    
    private void OnDisable()
    {
        if (inventoryController == null) return;
        inventoryController.OnInventoryChanged -= OnInventoryChange;
    }
    
    //Binds to new inventoryC and sets AvailableRecipes from the provided recipe list through GetRecipeList
    //Unsubscribes from old inventoryC if not null
    public void Initialize(InventoryController inventoryC, List<ItemRecipe> recipes)
    {
        //return if null, desubscribe from old invC, bind & subscribe to new inventoryC, set available recipes
        
        if(inventoryC  == null || recipes == null)
        {
            Debug.LogWarning("Null references on crafting controller initialization.");
            return;
        }
        if(inventoryController != null) inventoryController.OnInventoryChanged -= OnInventoryChange; //prevent lingering subscriptions
        
        this.inventoryController = inventoryC;
        inventoryController.OnInventoryChanged += OnInventoryChange;
        
        this.AvailableRecipes = GetRecipeList(recipes);
    }
    #endregion

    #region Helper methods & event callbacks
    //Filters through provided ItemRecipe list
    //only includes recipes where RequiredStation == this.stationType || RequiredStation == None
    private List<ItemRecipe> GetRecipeList(List<ItemRecipe> recipeList)
    {
        //return if List is null, filter list
        List<ItemRecipe> result = new List<ItemRecipe>();
        if(recipeList == null) return result;
        
        foreach (ItemRecipe recipe in recipeList)
        {
            if(recipe == null) continue;
            if(recipe.requiredStation == stationType || recipe.requiredStation == CraftingStationType.None)
                result.Add(recipe);
        }
        return result;
    }
    private void OnInventoryChange()
    {
        OnDataChanged?.Invoke();
    }
    #endregion
    
    #region ViewModel Requests
    //calls inventoryC.HasResource and returns result 
    public bool HasResource(ResourceCost resourceCost)
    {
        if(inventoryController == null) return false;
        return inventoryController.HasResource(resourceCost.itemDataSo, resourceCost.amount);
    }
    
    //calls inventoryC.HasResources and returns result
    public bool CanCraft(ItemRecipe recipe)
    {
        if(recipe == null || inventoryController == null) return false;
        return inventoryController.HasResources(recipe.resources);
    }
    
    //Checks if resources are sufficient
    //removes resource
    //adds item to inventory
    //drops item amount that couldn't fit in inventory
    //returns true if crafting was successful
    public bool TryCraft(ItemRecipe recipe)
    {
        //return if null, check resources, remove, add item result, drop item amount that didn't fit
        
        if(recipe == null || inventoryController == null) return false;
        
        if(!CanCraft(recipe)) return false;
        
        inventoryController.RemoveResources(recipe.resources);
        int amount = inventoryController.AddItem(recipe.itemResult, recipe.outputAmount);

        if (amount > 0)
        {
            bool placed = dropHandler.TryPlaceInFirstValidPos(recipe.itemResult, amount);
            if (!placed)
            {
                //rollback
                inventoryController.RemoveResource(recipe.itemResult, recipe.outputAmount - amount);
                int unableToAdd = 0;
                foreach (var resource in recipe.resources)
                {
                    unableToAdd += inventoryController.AddItem(resource.itemDataSo, resource.amount);
                }
                if(unableToAdd > 0) Debug.LogError("Crafting Rollback failed, AddItem was not able to fit full resource amount");
                return false;
            }
        }
        return true;
        //no event invoke here, inventory controller will invoke OnInventoryChanged,
        //which will trigger crafting controller's OnDataChanged,
        //which will trigger the view model's OnDataChanged,
        //which will trigger the view to update
    }
    #endregion
}