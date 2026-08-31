using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private DropHandler dropHandler;
    public int slotAmount = 8;
    public InventorySlot[] Slots {get; private set;}
    public InventorySlot[] startingItems;
    public event Action OnInventoryChanged;

    public bool IsEmpty => Slots.All(slot => slot.IsEmpty);

    // Creates a new Inventory array with empty slots
    // fills starting items if they exist and are valid
    private void Awake()
    {
        Slots = new InventorySlot[slotAmount];
        for (int i = 0; i < slotAmount; i++)
        {
            Slots[i] = new InventorySlot();
            if (startingItems != null && i < startingItems.Length 
                                      && startingItems[i] != null 
                                      && startingItems[i].itemData != null 
                                      && startingItems[i].amount > 0) 
            {   //set startingItems in inventory
                Slots[i].itemData = startingItems[i].itemData;
                Slots[i].amount = startingItems[i].amount;
            }
        }

    }
    
    #region Add items
    // tries to stack first, ends there if the whole amount was stacked
    // otherwise goes through inventory and fills empty slots until amount is 0 or no more empty slots
    // then returns remaining amount
    public int AddItem(ItemData itemData, int amount)
    { 
        if(itemData == null ||  amount <= 0) return amount;
        
        amount = TryStacking(itemData, amount); // try stacking and return remaining amount
        
        if (amount == 0)
        {
            OnInventoryChanged?.Invoke();
            return 0;
        }
        
        foreach (InventorySlot slot in Slots)
        {
            if (!slot.IsEmpty) continue;
            
            slot.itemData = itemData;
            int toAdd = amount < slot.itemData.maxStackAmount ? 
                amount : slot.itemData.maxStackAmount; //check the amount to add
            
            slot.amount = toAdd;
            amount -= toAdd;
            if (amount == 0)
            {
                break;
            }
        }
        OnInventoryChanged?.Invoke();
        return amount;
    }

    // search for slots where slot isn't full and item data == received item data
    // calculate remaining capacity in that slot and stack as much as possible
    // reduce amount by stacked amount, repeat until amount is 0 or no more stackable slots
    public int TryStacking(ItemData itemData, int amount)
    {
        foreach (InventorySlot slot in Slots )
        {
            if (slot.itemData == itemData && !slot.IsFull) 
            { 
                //if current slot item is same as item and slot amount < maxStack try stacking
                int remainingCapacity = slot.itemData.maxStackAmount - slot.amount;
                
                if (amount <= remainingCapacity)  
                {
                    //stack all  if remaining slot capacity is less or equal to remaining capacity
                    slot.amount += amount;
                    return 0;
                }
                
                if (amount > remainingCapacity) 
                {
                    //if amount is more than remaining capacity -> add remaining, look for empty slot to add rest
                    slot.amount += remainingCapacity;
                    amount -= remainingCapacity;
                }
            }
        }
        return amount;
    }
    #endregion
    
    #region Drop/Slot interaction
    // checks if slot is valid and not empty
    // Instantiates item drop in front of player or up from object and clears slot
    public void DropItem(int index)
    {
        //check if index is valid and slot isn't empty
        if (index < 0 || index > Slots.Length - 1) return;
        
        InventorySlot slot = Slots[index];
        if (slot.IsEmpty) return;
        
        //drop item with slot item amount & clear slot
        int itemAmount = slot.amount;
        
        bool success = dropHandler?.TryPlace(slot.itemData, itemAmount) ?? false;

        if (!success) return;
       
        ClearSlot(slot);
        OnInventoryChanged?.Invoke();
    }
    
    //checks if both indices are valid and not the same
    //then checks if items are the same type and tries merging
    //if merging isn't possible or items aren't the same type, swaps them
    public void SwapItem(int indexTo, int indexFrom)
    {
        //valid indices and not the same index
        if(indexTo == indexFrom) return;
        if (indexTo < 0 || indexTo >= Slots.Length) return;
        if (indexFrom < 0 || indexFrom >= Slots.Length) return;
        
        InventorySlot targetSlot = Slots[indexTo];
        InventorySlot ogSlot = Slots[indexFrom];
        
        if (targetSlot.itemData == null && ogSlot.itemData == null) return; //return if both items are null
        
        //try merging
        if (targetSlot.itemData == ogSlot.itemData)
        {
            bool mergeSuccessful =  TryMerging(targetSlot, ogSlot);
            if (mergeSuccessful) {
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        
        //swap items
        //assign invSlot1 = invSlot2 and vice versa, then invoke invChanged event
        ItemData ogSlotItem = ogSlot.itemData;
        ItemData targetSlotItem = targetSlot.itemData;
        int ogSlotAmount = ogSlot.amount;
        int  targetSlotAmount = targetSlot.amount;
        
        Slots[indexTo].itemData = ogSlotItem;
        Slots[indexTo].amount = ogSlotAmount;
        Slots[indexFrom].itemData = targetSlotItem;
        Slots[indexFrom].amount = targetSlotAmount;
        OnInventoryChanged?.Invoke();
    }
    
    // adds as much as possible to target slot if same item type and not full
    // reduces ogSlot by that amount or clears it if all was moved
    private bool TryMerging(InventorySlot targetSlot, InventorySlot ogSlot)
    {
        //return if full or not same item type
        ItemData itemType  = targetSlot.itemData;
        if (targetSlot.IsFull || ogSlot.IsFull || targetSlot.itemData != ogSlot.itemData) return false;
        
        //Merge
        int targetSlotRemainingCapacity = itemType.maxStackAmount - targetSlot.amount;

        if (targetSlotRemainingCapacity >= ogSlot.amount)
        {
                targetSlot.amount += ogSlot.amount;
                ClearSlot(ogSlot);
        }
        else
        {
                targetSlot.amount += targetSlotRemainingCapacity;
                ogSlot.amount -= targetSlotRemainingCapacity;
        }
        return true;
    }
    #endregion

    #region resources
    //iterates through resourceList, returns false if any resource check fails
    public bool HasResources(List<ResourceCost> resourceList)
    {
        foreach (ResourceCost resource in resourceList )
        {
            ItemData item = resource.itemDataSo;
            int amount = resource.amount;
            bool hasResource = HasResource(item, amount);
            if(!hasResource) return false;
        }
        return true;
    }
    
    //counts total amount of resourceData across all slots, returns true if total >= amount
    public bool HasResource(ItemData resourceData, int amount)
    {
        int totalAmount = 0;
        foreach (InventorySlot slot in Slots)
        {
            if (slot.itemData != resourceData) continue;
            totalAmount += slot.amount;
            if(totalAmount >= amount) return true;
        }
        return false;
    }
    
    //returns false if resources aren't available
    //iterates through resourceList and removes each resource, returns false if any removal fails
    //invokes OnInventoryChanged if all resources removed successfully
    public bool RemoveResources(List<ResourceCost> resourceList)
    {
        if(!HasResources(resourceList)) return false;
        foreach (ResourceCost resource in resourceList )
        {
            ItemData item = resource.itemDataSo;
            int amount =  resource.amount;
            bool removed = RemoveResource(item, amount);
            if(!removed) return false; //safety, just in case, probably overkill
        }
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    //returns false if resource isn't available
    //iterates through slots, removes amount from slots until amount is 0
    //clears slot if amount reaches 0, returns true if all amount removed successfully
    public bool RemoveResource(ItemData resource, int amount) 
    {
        if(!HasResource(resource, amount)) return false;
        foreach (InventorySlot slot in Slots)
        {
            if (slot.itemData != resource) continue;
            if (slot.amount >= amount)
            {
                slot.amount -= amount;
                if (slot.amount <= 0)
                    ClearSlot(slot);
                return true;
            }
            else
            {
                int thisSlotAmount = slot.amount;
                ClearSlot(slot);
                amount -= thisSlotAmount;
            }
        }
        return false; //compiler requirement, function will never reach this point
    }
    #endregion
    
    //sets slot itemData to null and amount to 0
    private void ClearSlot(InventorySlot slot)
    {
        slot.itemData = null;
        slot.amount = 0;
    }

    
    // If items are the same type it tries to merge the slots and add the remaining amount to the target inventory
    // If not the same type it just swaps the slot contents
    public static void TransferItem(InventoryController source, int sourceIdx,  InventoryController target, int targetIdx)
    {
        if (source == target && sourceIdx == targetIdx) return; //unlikely but guards against item disappearing bug
        if (sourceIdx < 0 || sourceIdx >= source.Slots.Length) return; //return if index out of bounds
        if (targetIdx < 0 || targetIdx >= target.Slots.Length) return;

        InventorySlot sourceSlot = source.Slots[sourceIdx];
        InventorySlot targetSlot = target.Slots[targetIdx];

        if (targetSlot.itemData == null && sourceSlot.itemData == null) return; //return if both items are null
        
        if (targetSlot.itemData == sourceSlot.itemData)
        {
            target.TryMerging(targetSlot, sourceSlot);
            if (!sourceSlot.IsEmpty)
            {
                int remainder = target.AddItem(sourceSlot.itemData, sourceSlot.amount);
                if(remainder == 0) source.ClearSlot(sourceSlot);
                else sourceSlot.amount =  remainder;
            }
            target.OnInventoryChanged?.Invoke();
            source.OnInventoryChanged?.Invoke();
            return;
        }
        
        //assign invSlot1 = invSlot2 and vice versa, then invoke invChanged event
        ItemData sourceSlotItem = sourceSlot.itemData;
        ItemData targetSlotItem = targetSlot.itemData;
        int sourceSlotAmount = sourceSlot.amount;
        int  targetSlotAmount = targetSlot.amount;
        
        source.Slots[sourceIdx].itemData = targetSlotItem;
        source.Slots[sourceIdx].amount = targetSlotAmount;
        target.Slots[targetIdx].itemData = sourceSlotItem;
        target.Slots[targetIdx].amount = sourceSlotAmount;
        target.OnInventoryChanged?.Invoke();
        source.OnInventoryChanged?.Invoke();
    }
}