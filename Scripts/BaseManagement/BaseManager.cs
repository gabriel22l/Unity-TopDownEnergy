using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class BaseManager : MonoBehaviour
{
    public List<BaseSlot> BaseSlots { get; private set; }
    public InventoryController InventoryController {get; private set; }
    public List<StructureRecipe> AvailableRecipes { get; private set; }

    public List<GameObject> lightPosts = new List<GameObject>();
    public List<LightPost> lightObjects = new List<LightPost>();

    public EnergyController EnergyController { get; private set; }

    public event Action OnSlotsChanged;
    public event Action OnDataChanged;
    
    private void Awake()
    {
        BaseSlots = new List<BaseSlot>(GetComponentsInChildren<BaseSlot>());
        EnergyController = GetComponent<EnergyController>();
        if(EnergyController == null) Debug.LogError("No Energy Controller found on " + gameObject.name);
        
        GetLights();
    }
    private void GetLights()
    {
        lightObjects.Clear();
        foreach (GameObject lightPost in lightPosts)
        {
            LightPost currentLight = lightPost.GetComponentInChildren<LightPost>();
            if (currentLight != null) lightObjects.Add(currentLight);
        }
    }
    #region building
    
    //subscribes to new inventoryC
    //unsubscribes from previous inventoryC.OnInventoryChanged if not null
    //subscribes to EnergyController.OnEnergyChanged to notify data changes
    public void Bind(InventoryController inventoryController, List<StructureRecipe> recipes)
    {
        //unsubscribe from previous InventoryC & run null guards
        if(this.InventoryController != null) this.InventoryController.OnInventoryChanged -= NotifyDataChanged;
        if (inventoryController == null || recipes == null)
        {
            Debug.LogWarning("Attempted to bind BaseManager with null InventoryController or Recipes list");
            return;
        }
        
        //set references & subscribe
        this.InventoryController = inventoryController;
        this.AvailableRecipes = recipes;
        
        inventoryController.OnInventoryChanged += NotifyDataChanged;
        
        //subscription to energy changes for viewmodel and view updates
        EnergyController.OnEnergyChanged -= NotifyDataChanged;
        EnergyController.OnEnergyChanged += NotifyDataChanged;
    }
    
    //unsubscribes from inventoryC and EnergyController
    public void UnBind()
    {
        if(EnergyController != null) EnergyController.OnEnergyChanged -= NotifyDataChanged;
        if(InventoryController != null) InventoryController.OnInventoryChanged -= NotifyDataChanged;
        
        this.InventoryController = null;
        this.AvailableRecipes = null;
    }
    private void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    public bool HasResources(List<ResourceCost> costs, float energy)
    {
        if (InventoryController == null) return false;
        bool hasResources = InventoryController.HasResources(costs);
        bool hasEnergy = HasEnergy(energy);
        return hasResources && hasEnergy;
    }
    public bool RequestCheckResource(ResourceCost resource)
    {
        if(InventoryController == null) return false;
        ItemData data = resource.itemDataSo;
        int amount = resource.amount;
        return InventoryController.HasResource(data, amount);
    }
    public bool CanBuildRecipe(StructureRecipe recipe, int baseSlotIndex)
    {
        if(recipe == null || baseSlotIndex < 0 || baseSlotIndex >= BaseSlots.Count) return false;
        if(!BaseSlots[baseSlotIndex].IsEmpty) return false;
        return HasResources(recipe.resources, recipe.energyCost) && CanPlaceStructure(recipe.structureResult);
    }
    public bool HasEnergy(float energyAmount)
    {
        if(EnergyController == null) return false;
        return EnergyController.CurrentEnergy >= energyAmount;
    }
    public bool TryBuild(int baseSlotIndex, StructureRecipe recipe)
    {
        if(recipe.structureResult.uniqueStructure && HasStructure(recipe.structureResult)) return false;
        if(baseSlotIndex < 0 || baseSlotIndex >= BaseSlots.Count) return false;
        if(InventoryController == null) return false;
        
        BaseSlot baseSlot = BaseSlots[baseSlotIndex];
        if(!baseSlot.IsEmpty) return false;
        
        if(!HasEnergy(recipe.energyCost)) return false;   
        
        bool removedResources = InventoryController.RemoveResources(recipe.resources);
        if (!removedResources) return false;
        
        EnergyController.RemoveEnergy(recipe.energyCost);
        
        IStructure structure = baseSlot.Build(recipe.structureResult);
        structure?.Initialize(this);
        
        OnSlotsChanged?.Invoke();
        return true;
    }
    
    #region Structure Uniqueness Logic
    private bool HasStructure(StructureData structureData)
    {
        if(structureData == null) return false;
        foreach (BaseSlot slot in BaseSlots)
        {
            if(slot.StructureData == structureData) 
                return true;
        }
        return false;
    }
    private bool CanPlaceStructure(StructureData structureData)
    {
        if(structureData == null) return false;
        if(structureData.uniqueStructure && HasStructure(structureData)) return false;
        return true;
    }
    #endregion
    #endregion
}