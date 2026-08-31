using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIController : MonoBehaviour
{
    public InventoryViewModel InvViewModel {get; private set;}
    [SerializeField] private GameObject slotPrefab;
    public List<InventoryUISlot> UISlots { get; private set; } = new List<InventoryUISlot>();
    
    [Header("Transform References")]
    [SerializeField] private GridLayoutGroup containerGridLayout;
    [SerializeField] private int slotsPerRow = 5;
    [SerializeField] private int slotSize = 75;
    
    private List<SlotViewData> viewDataList = new List<SlotViewData>();

    #region Unity Lifecycle
    private void OnEnable() 
    {
        //only refresh and subscribe to events while enabled to prevent array reference bugs
        if(InvViewModel == null) return;
        
        InvViewModel.OnInvDataChanged -= RefreshInventoryUI; 
        InvViewModel.OnInvDataChanged += RefreshInventoryUI;
        
        RefreshInventoryUI(); 
    }
    private void OnDisable()
    {
        if(InvViewModel == null) return;
        
        InvViewModel.OnInvDataChanged -= RefreshInventoryUI;
    }
    #endregion
    
    //Binds to new IVM, handles transform, subscribes and refreshes if already active
    public void Initialize(InventoryViewModel ivm)
    {
        if(ivm == null || ivm.InventorySlotAmount == 0) return;
        
        this.InvViewModel = ivm;
        HandleTransform();
        
        if(gameObject.activeInHierarchy)
        {
            InvViewModel.OnInvDataChanged -= RefreshInventoryUI; 
            InvViewModel.OnInvDataChanged += RefreshInventoryUI;
            RefreshInventoryUI(); //refresh ui if already active, otherwise wait for OnEnable
        }
    }
    
    #region Slot Handling
    //Syncs slot pool to current slot amount, fetches view data and updates all slots
    private void RefreshInventoryUI()
    {
        if (InvViewModel == null) return;

        SyncSlotPool(InvViewModel.InventorySlotAmount);
        
        InvViewModel.GetInventorySlots(viewDataList);
        for (int i = 0; i < viewDataList.Count; i++)
        {
            UISlots[i].UpdateViewData(viewDataList[i]);
        }
    }
    
    //Removes slots from end of list if over target count, adds slots if under
    private void SyncSlotPool(int targetCount)
    {
        // Remove from tail if over
        for (int i = UISlots.Count - 1; i >= targetCount; i--)
        {
            if (UISlots[i] != null) Destroy(UISlots[i].gameObject);
            UISlots.RemoveAt(i);
        }

        // Add to tail if short
        while (UISlots.Count < targetCount)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            InventoryUISlot uiSlot = slot.GetComponent<InventoryUISlot>();
            uiSlot.Initialize(this);
            UISlots.Add(uiSlot);
        }
    }
    #endregion
    
    #region View -> ViewModel requests
    public void RequestSwapItem(int indexTo, int indexFrom)
    {
        //if(indexTo >= uiSlots.Count || indexTo < 0 || indexFrom  >= uiSlots.Count || indexFrom < 0) return;
        InvViewModel.RequestSwapItems(indexTo, indexFrom);
    }
    public void RequestTransferItem(InventoryUIController sourcC, int sourceIdx, InventoryUIController targetC, int targetIdx)
    {
        InvViewModel.RequestTransferItem(sourcC.InvViewModel, sourceIdx, targetC.InvViewModel, targetIdx);
    }
    public void RequestDropItem(int index)
    {
        InvViewModel.RequestDropItem(index);
    }
    #endregion

    //Sizes inventory panel and container based on slot count, slots per row and slot size
    private void HandleTransform()
    {
        //return if null ref, adjust cellSize
        if(containerGridLayout == null || InvViewModel == null) return;
        if (!containerGridLayout.TryGetComponent(out RectTransform containerRect)) return;
        
        containerGridLayout.cellSize = new Vector2(slotSize, slotSize);
        
        //calculate grid x and y size
        int slotAmount =  InvViewModel.InventorySlotAmount;
        int totalXSize = slotsPerRow * slotSize;
        int totalYSize = Mathf.CeilToInt((float)slotAmount / slotsPerRow) * slotSize;
        
        //set sizes, add additional padding to inventoryPanel
        containerRect.sizeDelta = new Vector2(totalXSize, totalYSize);
    }
    
    //purpose is for hotbar to set its slotAmount automatically
    //avoids the need to set it manually if hotbar.slotAmount changes
    public void SetSlotsPerRow(int amount)
    {
        slotsPerRow = amount;
    }
}