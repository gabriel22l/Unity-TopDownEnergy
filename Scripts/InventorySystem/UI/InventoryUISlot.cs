using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image slotImg;
    public Image itemImg;
    public TextMeshProUGUI itemTxt;
    public int slotIndex;
    public InventoryUIController InventoryUIController {get; private set;}
    private SlotViewData viewData;
    
    [SerializeField] private Color defaultColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color selectedColor = new Color(1, 1, 1, 1f);

    private void OnEnable()
    {
        SetSelected(false);
    }
    public void Initialize(InventoryUIController invUI)
    {
        InventoryUIController = invUI;
    }

    #region ViewData & UI values setters
    //sets data and index, updates UI
    public void UpdateViewData(SlotViewData viewData)
    {
        this.viewData = viewData;
        this.slotIndex = viewData.index;
        SetUIFromData(viewData);
    }
    
    // Sets color, text and sprite values based on the provided view data. Clears slot if no item is present
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
    #endregion
    
    #region drop events
    //Gets source slot from dropped DragAndDrop component
    //swaps if source and target share the same controller, transfers if they differ
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;
        
        DragAndDrop dragAndDrop = droppedObject.GetComponent<DragAndDrop>();
        if (dragAndDrop == null || !dragAndDrop.IsDragging) return;
        
        InventoryUISlot sourceSlot = dragAndDrop.sourceSlot;
        if (sourceSlot == null) return;

        InventoryUIController sourceSlotUIController = sourceSlot.InventoryUIController;
        InventoryUIController targetSlotUIController = InventoryUIController;
        
        int sourceIndex = sourceSlot.slotIndex;
        int thisIndex = slotIndex;

        if (sourceSlotUIController.InvViewModel.InvController == targetSlotUIController.InvViewModel.InvController)
        {
            InventoryUIController.RequestSwapItem(thisIndex, sourceIndex);
        }
        else
        {
            InventoryUIController.RequestTransferItem(sourceSlotUIController, sourceIndex, targetSlotUIController, thisIndex);
        }
    }
    // Called on item dropped out of UI
    public void OnItemDropOut()
    {
        InventoryUIController.RequestDropItem(slotIndex);
    }
    #endregion

    #region Pointer events
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetSelected(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetSelected(false);
    }
    #endregion
    public void SetSelected(bool selected)
    {
        if (slotImg == null)
        {
            Debug.LogWarning("Tried to set selected but slotImg is null");
            return;
        }
    slotImg.color = selected ? selectedColor : defaultColor;
    }
}