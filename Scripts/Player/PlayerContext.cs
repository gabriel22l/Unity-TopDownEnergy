using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerContext : MonoBehaviour
{
    //PlayerContext class provides access to player components
    public UIController UiController { get; private set; } //UIController wired to this player instance
    [field:SerializeField] public InventoryController InventoryController { get; private set; }
    [field: SerializeField] public HotbarController hotbarC { get; private set; }
    [field:SerializeField] public PlayerInput PlayerInput { get; private set; }
    public InventoryViewModel InventoryViewModel { get; private set; }
    public HotbarViewModel HotbarViewModel { get; private set; }
    public List<StructureRecipe> structureRecipes;
    public List<ItemRecipe> itemRecipes;
    
    [SerializeField]private CraftingController craftingController;
    public CraftingViewModel  CraftingViewModel { get; private set; }

    private void Awake()
    {
        HotbarViewModel = new HotbarViewModel(hotbarC, InventoryController);
        InventoryViewModel = new InventoryViewModel(InventoryController, hotbarC.SlotAmount, InventoryController.slotAmount);
        
        //Crafting
        craftingController.Initialize(InventoryController, itemRecipes);
        CraftingViewModel = new CraftingViewModel(craftingController);
    }
    private void OnDestroy()
    {
        InventoryViewModel?.Dispose();
        HotbarViewModel?.Dispose();
    }
    public void Initialize(UIController uiController)
    {
        this.UiController = uiController;
    }
}