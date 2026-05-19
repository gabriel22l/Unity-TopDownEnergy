using DefaultNamespace;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class BaseManager : MonoBehaviour
{
    public List<BaseSlot> BaseSlots { get; private set; }
    private InventoryController inventoryController;
    public event Action<List<BaseSlot>> OnSlotsChanged;

    public List<GameObject> lightPosts = new List<GameObject>();
    public List<Light2D> lightObjects = new List<Light2D>();

    public EnergyController EnergyController { get; private set; }

    private void Awake()
    {
        BaseSlots = new List<BaseSlot>(GetComponentsInChildren<BaseSlot>());
        EnergyController = GetComponent<EnergyController>();
        if(EnergyController == null) Debug.LogError("No Energy Controller found on " + gameObject.name);
        
        GetLights();
    }
    private void GetLights()
    {
        if (lightPosts == null) return;
        foreach (GameObject lightPost in lightPosts)
        {
            Light2D currentLight = lightPost.GetComponentInChildren<Light2D>();
            if (currentLight != null) lightObjects.Add(currentLight);
        }
    }
    #region building
    
    public void Bind(InventoryController inventoryController)
    {
        this.inventoryController = inventoryController;
    }
    public void UnBind()
    {
        this.inventoryController = null;
    }

    public bool HasResources(List<ResourceCost> costs, float energy)
    {
        if (inventoryController == null) return false;
        bool hasResources = inventoryController.HasResources(costs);
        bool hasEnergy = HasEnergy(energy);
        return hasResources && hasEnergy;
    }
    public bool RequestCheckResource(ResourceCost resource)
    {
        if(inventoryController == null) return false;
        itemDataSO data = resource.itemDataSo;
        int amount = resource.amount;
        return inventoryController.HasResource(data, amount);
    }
    public bool HasEnergy(float energyAmount)
    {
        if(EnergyController == null) return false;
        return EnergyController.CurrentEnergy >= energyAmount;
    }
    public bool TryBuild(int baseSlotIndex, StructureRecipeSO recipe)
    {
        if(recipe.structureResult.uniqueStructure && HasStructure(recipe.structureResult)) return false;
        if(baseSlotIndex < 0 || baseSlotIndex >= BaseSlots.Count) return false;
        if(inventoryController == null) return false;
        
        BaseSlot baseSlot = BaseSlots[baseSlotIndex];
        if(!baseSlot.IsEmpty) return false;
        
        if(!HasEnergy(recipe.energyCost)) return false;   
        
        bool removedResources = inventoryController.RemoveResources(recipe.resources);
        if (!removedResources) return false;
        
        EnergyController.RemoveEnergy(recipe.energyCost);
        
        IStructure structure = baseSlot.Build(recipe.structureResult);
        structure?.Initialize(this);
        
        OnSlotsChanged?.Invoke(BaseSlots);
        return true;
    }
    private bool HasStructure(StructureDataSO structureData)
    {
        if(structureData == null) return false;
        foreach (BaseSlot slot in BaseSlots)
        {
            if(slot.StructureData == structureData) 
                return true;
        }
        return false;
    }
    public bool CanBuildStructure(StructureDataSO structureData)
    {
        if(structureData == null) return false;
        if(structureData.uniqueStructure && HasStructure(structureData)) return false;
        return true;
    }
    #endregion
}