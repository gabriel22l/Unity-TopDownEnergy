using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour, IDropHandler
{
    public Image itemImg;
    public TextMeshProUGUI itemTxt;
    public int slotIndex;
    private InventoryUIController inventoryUIController;
    private SlotViewData viewData;
    
    public void Initialize(InventoryUIController invUI, int index)
    {
        inventoryUIController = invUI;
        slotIndex = index;
    }

    public void UpdateViewData(SlotViewData viewData)
    {
        this.viewData = viewData;
        SetUIFromData(viewData);
    }
    private void SetUIFromData(SlotViewData viewData)
    {
        if(!viewData.HasItem)
        {
            ClearSlot();
            return;
        }
        itemImg.sprite = viewData.Sprite;
        itemTxt.text = viewData.ItemAmount.ToString();
        itemImg.color = Color.white;
    }
    private void ClearSlot()
    {
        itemImg.sprite = null;
        itemTxt.text = "";
        itemImg.color = Color.clear;
    }
    #region drop events
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;
        
        DragAndDrop dragAndDrop = droppedObject.GetComponent<DragAndDrop>();
        if (dragAndDrop == null) return;
        
        InventoryUISlot droppedParentUiSlot = dragAndDrop.sourceSlot;
        if (droppedParentUiSlot == null) return;
        
        int droppedObjIndex = droppedParentUiSlot.slotIndex;
        inventoryUIController.RequestSwapItem(slotIndex, droppedObjIndex);
    }
    public void OnItemDropOut()
    {
        inventoryUIController.RequestDropItem(slotIndex);
    }
    #endregion
}
