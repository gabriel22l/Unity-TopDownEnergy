using System;
using UnityEngine;

public class InventoryViewModel
{
    private readonly InventoryController inventoryController;
    public event Action OnInvDataChanged;
    public int InventorySlotAmount => inventoryController.slotAmount;

    public InventoryViewModel(InventoryController inventoryC)
    {
        inventoryController = inventoryC;
        inventoryC.OnInventoryChanged += OnDataChanged;
    }
    public void Dispose()
    {
        inventoryController.OnInventoryChanged -= OnDataChanged;
    }

    private void OnDataChanged()
    {
        OnInvDataChanged?.Invoke();
    }

    public SlotViewData GetInventoryViewData(int index)
    {
        SlotViewData data = new SlotViewData();
        InventorySlot slot = inventoryController.slots[index];
        if (slot.itemDataSo != null)
        {
            data.HasItem = true;
            data.Sprite = slot.itemDataSo.itemIcon;
            data.ItemAmount = slot.amount;
        } else{
            data.HasItem = false;
            data.Sprite = null;
            data.ItemAmount = 0;
        }
        return data;
    }
    public void RequestSwapItems(int indexTo, int indexFrom)
    {
        inventoryController.SwapItem(indexTo, indexFrom);
    }
    public void RequestDropItem(int index)
    {
        inventoryController.DropItem(index);
    }
}

public struct SlotViewData
{
    public bool HasItem;
    public Sprite Sprite;
    public int ItemAmount;
}