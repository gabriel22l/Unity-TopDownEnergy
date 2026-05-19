using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    private InventoryViewModel inventoryViewModel;
    [SerializeField] private GameObject slotPrefab;
    private InventoryUISlot[] uiSlots;

    public void Initialize(InventoryViewModel ivm) //assigns inventoryC reference and initializes slots
    {
        this.inventoryViewModel = ivm;
        InstantiateSlots();
    }
    private void InstantiateSlots() //only call in initialize
    {   //uiSlots amount = inventoryC slot amount
        uiSlots = new InventoryUISlot[inventoryViewModel.InventorySlotAmount]; 
        for(int i = 0; i < inventoryViewModel.InventorySlotAmount; i++) //instantiate slots, get the class containing text/img child objects
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            uiSlots[i] = slot.GetComponent<InventoryUISlot>();
            uiSlots[i].Initialize(this, i);
        }
    }
    private void OnEnable() 
    { //only refresh and subscribe to events while enabled to prevent array reference bugs
        RefreshInventoryUI(); 
        inventoryViewModel.OnInvDataChanged += RefreshInventoryUI;
    }
    private void OnDisable()
    {
        inventoryViewModel.OnInvDataChanged -= RefreshInventoryUI;
    }

    private void RefreshInventoryUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {   //go through every slot, if not empty, set icon, remove transparency, set text to item amount
            SlotViewData currentSlot = inventoryViewModel.GetInventoryViewData(i);
            uiSlots[i].UpdateViewData(currentSlot);
        }
    }
    public void RequestSwapItem(int indexTo, int indexFrom)
    {
        if(indexTo >= uiSlots.Length || indexTo < 0 || indexFrom  >= uiSlots.Length || indexFrom < 0) return;
        inventoryViewModel.RequestSwapItems(indexTo, indexFrom);
    }
    public void RequestDropItem(int index)
    {
        inventoryViewModel.RequestDropItem(index);
    }
}
