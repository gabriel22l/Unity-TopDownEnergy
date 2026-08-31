using UnityEngine;
using System;
using System.Collections.Generic;
public class BaseManagerViewModel
{
    //Pending: Fix Binding issue
    //currently base manager would have to be bound before creating the view model & disposed once interaction ends
    //Possible fix: Make BaseManager own the OnInventoryChanged and OnEnergyChanged event subscriptions
    //make the callback be a NotifyDataChanged() method that invokes its own data changed event
    //then have the ViewModel (this) subscribe to that event and forward it to the UI through its own OnDataChanged event
    // (BaseManager.OnDataChanged += NotifyDataChanged)
    // Note: Consider changing the name of the events for clarity
    public BaseManager BaseManager { get; private set; }
    
    //Invoked on inventory, energy and base slots changes
    public event Action OnDataChanged;
    
    #region Binding & Event Handling
    public BaseManagerViewModel(BaseManager baseManager)
    {
        this.BaseManager = baseManager;
        baseManager.OnSlotsChanged += NotifyDataChanged;
        baseManager.OnDataChanged += NotifyDataChanged;
    }
    public void Dispose()
    {
        if(BaseManager == null) return;
        BaseManager.OnSlotsChanged -= NotifyDataChanged;
        BaseManager.OnDataChanged -= NotifyDataChanged;
    }
    private void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }
    #endregion

    #region UI Requests
    
    public EnergyViewData GetEnergyData()
    {
        EnergyViewData data = new EnergyViewData();
        if(BaseManager?.EnergyController == null)
        {
            Debug.LogError("Null Energy Controller attached to BaseManager");
            return data;
        }
        
        data.currentEnergy = BaseManager.EnergyController.CurrentEnergy;
        data.maxEnergy = BaseManager.EnergyController.MaxEnergy;
        data.energyPercentage = BaseManager.EnergyController.EnergyPercentage;
        
        return data;
    }
    
    // loops through baseSlots and gets data through GetBaseSlotData
    public List<BaseSlotData> GetAllBaseSlotData()
    {
        List<BaseSlotData> dataList = new List<BaseSlotData>();
        
        //Handle null refs and empty base slot list
        if(BaseManager == null)
        {
            Debug.LogError("Null BaseManager");
            return dataList;
        }
        if(BaseManager.BaseSlots == null || BaseManager.BaseSlots.Count == 0) return dataList;
        
        //loop through base slots, get data for each slot and add to list
        for(int i = 0; i < BaseManager.BaseSlots.Count; i++)
        {
            if (BaseManager.BaseSlots[i] == null) continue;
            BaseSlotData slotData = GetBaseSlotData(BaseManager.BaseSlots[i], i);
            dataList.Add(slotData);
        }
        return dataList;
    }
    
    // loops through AvailableRecipes and gets data through GetStructureRecipeData
    public List<StructureRecipeData> GetAllStructureRecipeData(int baseSLotIndex)
    {
        List<StructureRecipeData> dataList = new List<StructureRecipeData>();
        
        // Handle null refs and empty lists
        if(BaseManager == null)
        {
            Debug.LogError("Null BaseManager");
            return dataList;
        }
        if(BaseManager.AvailableRecipes == null || BaseManager.AvailableRecipes.Count == 0) return dataList;
        
        // loop through AvailableRecipes, get data for each recipe
        for(int i = 0; i < BaseManager.AvailableRecipes.Count; i++)
        {
            if (BaseManager.AvailableRecipes[i] == null) continue;
            StructureRecipeData recipeData = GetStructureRecipeData(BaseManager.AvailableRecipes[i], i, baseSLotIndex);
            dataList.Add(recipeData);
        }
        return dataList;
        
    }
    public bool TryBuildRecipe(int baseSlotIndex, int recipeIndex)
    {
        return BaseManager.TryBuild(baseSlotIndex, BaseManager.AvailableRecipes[recipeIndex]);
    }
    #endregion

    #region Data Gathering Methods
    
    private BaseSlotData GetBaseSlotData(BaseSlot slot, int index)
    {
        BaseSlotData data = new BaseSlotData();
        if (BaseManager == null)
        {
            Debug.LogError("Null BaseManager");
            return data;
        }
        
        data.index = index;
        data.isEmpty = slot.IsEmpty;
        data.icon = slot.IsEmpty ? null : slot.StructureData.structureSprite;
        data.structureName = slot.IsEmpty ? "" : slot.StructureData.structureName;
        return data;
    }
    
    //recipe data (resource data, enough energy and can build are dependent on BaseManager)
    //the rest of the fields are set directly through Recipe
    private StructureRecipeData GetStructureRecipeData(StructureRecipe recipe, int index, int baseSlotIndex)
    {
        StructureRecipeData data = new StructureRecipeData();
        if(recipe == null) return data;
        
        data.index = index;
        data.structureName = recipe.structureResult.structureName;
        data.icon = recipe.structureResult.structureSprite;
        data.resources = GetAllResourceData(recipe.resources);
        data.energyCost = recipe.energyCost;
        data.requiresEnergy = recipe.energyCost > 0;
        data.enoughEnergy = BaseManager.HasEnergy(recipe.energyCost);
        data.canBuild = BaseManager.CanBuildRecipe(recipe, baseSlotIndex);
        
        return data;
    }
    
    // Loops through a provided ResourceCost List and Gets data through GetResourceViewData()
    private List<ResourceViewData> GetAllResourceData(List<ResourceCost> resources)
    {
        List<ResourceViewData> dataList = new List<ResourceViewData>();
        
        //Handle null refs and empty list
        if(BaseManager == null)
        {
            Debug.LogError("Null BaseManager");
            return dataList;
        }
        if(resources == null || resources.Count == 0) return dataList;
        
        //Loop through resources to get data
        foreach (ResourceCost resource in resources )
        {
            if(resource.amount <= 0 || resource.itemDataSo == null) continue;
            ResourceViewData data = GetResourceViewData(resource);
            dataList.Add(data);
        }
        return dataList;
    }
    
    // Sets icon and amount from ResourceCost, checks if player has enough through BaseManager.RequestCheckResource()
    private ResourceViewData GetResourceViewData(ResourceCost resource)
    {
        if(resource.itemDataSo == null || resource.amount <= 0) return new ResourceViewData();
        return new ResourceViewData()
        {
            resourceIcon = resource.itemDataSo.itemIcon,
            requiredAmount = resource.amount,
            hasEnough = BaseManager.RequestCheckResource(resource),
        };
    }
    #endregion
}
public struct EnergyViewData
{
    public float currentEnergy;
    public float maxEnergy;
    public float energyPercentage;
}

public struct BaseSlotData
{
    public int index;
    public bool isEmpty;
    public Sprite icon;
    public string structureName;
}

public struct StructureRecipeData
{
    public int index;
    public string structureName;
    public Sprite icon;
    public List<ResourceViewData> resources;
    public float energyCost;
    public bool requiresEnergy;
    public bool enoughEnergy;
    public bool canBuild;
}