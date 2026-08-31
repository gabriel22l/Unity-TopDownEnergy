using System;
using UnityEngine;
using System.Collections.Generic;

public class TerminalUIPage : MonoBehaviour
{
    [SerializeField] private UISlot baseUISlotPrefab;
    [SerializeField] private Transform slotsContainer;

    private TerminalUIController controller;
    private List<UISlot> uiSlots = new List<UISlot>();
    
    private BaseManagerViewModel viewModel;

    private void OnEnable()
    {
        if (viewModel == null) return;
        
        viewModel.OnDataChanged -= TriggerRefresh;
        viewModel.OnDataChanged += TriggerRefresh;
        TriggerRefresh();
    }
    private void OnDisable()
    {
        if (viewModel == null) return;
        
        viewModel.OnDataChanged -= TriggerRefresh;
    }
    
    //Unsubscribe from previous if not null, subscribe to new if active
    public void Initialize(TerminalUIController ctrl, BaseManagerViewModel viewModel)
    {
        if(this.viewModel != null) this.viewModel.OnDataChanged -= TriggerRefresh;  
        if (ctrl == null || viewModel == null)
        {
            Debug.LogError("Null parameters for TerminalUIPage");
            return;
        }
        
        this.viewModel = viewModel;
        controller = ctrl;
        
        //subscribe if already enabled, otherwise wait for OnEnable
        if(gameObject.activeInHierarchy) viewModel.OnDataChanged += TriggerRefresh;
        
        TriggerRefresh();
    }

    private void TriggerRefresh()
    {
        if(viewModel == null) return;
        List<BaseSlotData> slotDataList = viewModel.GetAllBaseSlotData();
        RefreshBaseSlots(slotDataList);
    }
    
    //Remove from end if more slots than data, add if more data than slots, update all existing slots
    public void RefreshBaseSlots(List<BaseSlotData> slotDataList)
    {
        if (slotDataList == null) return;

        //remove excess slots if more ui slots than data
        if (slotDataList.Count < uiSlots.Count)
        {
            for (int i = uiSlots.Count - 1; i >= slotDataList.Count; i--)
            {
                if (uiSlots[i] != null) Destroy(uiSlots[i].gameObject);
                uiSlots.RemoveAt(i);
            }
        }
        
        //add slots if more data than ui slots
        if(slotDataList.Count > uiSlots.Count)
        {
            for (int i = uiSlots.Count; i < slotDataList.Count; i++)
            {
                AddSlot(slotDataList[i]);
            }
        }
        //update all existing slots with new data
        for (int i = 0; i < slotDataList.Count && i < uiSlots.Count; i++)
        {
            if(uiSlots[i] == null) continue;
            SetSlotData(uiSlots[i], slotDataList[i]);
        }
    }
    //Instantiate, set index, set icon and text, add on click event
    private void AddSlot(BaseSlotData slotData)
    {
        UISlot slot = Instantiate(baseUISlotPrefab, slotsContainer);
        
        SetSlotData(slot, slotData);
        
        uiSlots.Add(slot);
    }
    private void SetSlotData(UISlot slot, BaseSlotData slotData)
    {
        if(controller  != null) 
            TerminalUIController.AddOnClickEvent(slot.gameObject, slotData.index, controller.EnableStructurePage);
        
        slot.SetIndex(slotData.index);
        
        if(slotData.isEmpty)
            slot.Clear();
        else
            slot.SetValues(slotData.structureName, slotData.icon);
    }
}