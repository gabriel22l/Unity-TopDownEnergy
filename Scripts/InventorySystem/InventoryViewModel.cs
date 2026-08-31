using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryViewModel
{
    public InventoryController InvController { get; private set; }
    
    // Inventory changed -> notify listeners to refresh
    public event Action OnInvDataChanged;
    public int InventorySlotAmount => slotCount  - startIndex;

    private int startIndex;
    private int slotCount;

    #region Binding & event handling
    public InventoryViewModel(InventoryController inventoryC, int startIndex, int slotCount)
    {
        InvController = inventoryC;
        inventoryC.OnInventoryChanged += OnDataChanged;
        
        this.startIndex = startIndex;
        this.slotCount = slotCount;
    }
    public void Dispose()
    {
        InvController.OnInventoryChanged -= OnDataChanged;
    }

    private void OnDataChanged()
    {
        OnInvDataChanged?.Invoke();
    }
    #endregion
    
    #region UI Requests
    //Clears and fills viewDataList with SlotViewData for each slot in the inventory
    public void GetInventorySlots(List<SlotViewData> viewDataList)
    {
        viewDataList.Clear();
        for(int i = startIndex; i < slotCount; i++)
        {
            if (i < 0 || i >= this.InvController.slotAmount) continue;
            viewDataList.Add(GetInventoryViewData(i));
        }
    }
    
    //Returns SlotViewData for the slot at the provided index
    private SlotViewData GetInventoryViewData(int index)
    {
        SlotViewData data = new SlotViewData();
        InventorySlot slot = InvController.Slots[index];
        
        if (slot.itemData != null)
        {
            data.HasItem = true;
            data.Sprite = slot.itemData.itemIcon;
            data.ItemAmount = slot.amount;
        }
        else
        {
            data.HasItem = false;
            data.Sprite = null;
            data.ItemAmount = 0;
        }
        data.index = index;
        return data;
    }
    public void RequestSwapItems(int indexTo, int indexFrom)
    {
        InvController.SwapItem(indexTo, indexFrom);
    }
    public void RequestDropItem(int index)
    {
        InvController.DropItem(index);
    }
    public void RequestTransferItem(InventoryViewModel sourceVM, int sourceIdx, InventoryViewModel targetVM, int targetIdx)
    {
        InventoryController.TransferItem(sourceVM.InvController, sourceIdx, targetVM.InvController, targetIdx);
    }
    #endregion
}

public struct SlotViewData
{
    public int index;
    public bool HasItem;
    public Sprite Sprite;
    public int ItemAmount;
}