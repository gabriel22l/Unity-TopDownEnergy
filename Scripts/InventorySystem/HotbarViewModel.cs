using UnityEngine;
using System;
public class HotbarViewModel
{
    private HotbarController hotbarC;
    public InventoryViewModel InventoryVM { get; private set; }
    public event Action OnSelectionChanged;
    public int SlotAmount
    {
        get
        {
            if (hotbarC == null) return 0;
            return hotbarC.SlotAmount;
        }
    }
    public int SelectedIndex => hotbarC.SelectedIndex;
    public HotbarViewModel(HotbarController hotbar, InventoryController inventoryC)
    {
        if (hotbar == null || inventoryC == null)
        {
            Debug.LogWarning("HotbarController or InventoryController are null");
            return;
        }
        
        this.hotbarC = hotbar;
        InventoryVM = new InventoryViewModel(inventoryC, 0, hotbar.SlotAmount);
        hotbar.OnSelectionChanged += NotifySelectionChanged;
    }
    public void Dispose()
    {
        if (hotbarC != null)
        {
            hotbarC.OnSelectionChanged -= NotifySelectionChanged;
        }
        InventoryVM.Dispose();
    }
    private void NotifySelectionChanged()
    {
        OnSelectionChanged?.Invoke();
    }
}