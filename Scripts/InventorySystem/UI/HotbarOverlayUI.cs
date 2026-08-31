using UnityEngine;

public class HotbarOverlayUI : MonoBehaviour
{
    [SerializeField] private InventoryUIController inventoryUI;

    private HotbarViewModel hotbarViewModel;
    private InventoryUISlot selectedSlot;
    public void Initialize(HotbarViewModel VM)
    {
        if (VM?.InventoryVM == null)
        {
            Debug.LogWarning("Null HotbarViewModel or InventoryViewModel at HotbarOverlayUI.Initialize");
            return;
        }
        hotbarViewModel = VM;
        
        inventoryUI.Initialize(hotbarViewModel.InventoryVM);
        inventoryUI.SetSlotsPerRow(hotbarViewModel.SlotAmount);

        VM.OnSelectionChanged += UpdateSelection;
        
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        if(hotbarViewModel == null) return;
        int index = hotbarViewModel.SelectedIndex;
        if (index < 0 || index >= inventoryUI.UISlots.Count)
        {
            Debug.LogWarning("Index out of range");
            return;
        }
        
        selectedSlot?.SetSelected(false);
        inventoryUI.UISlots[index].SetSelected(true);
        selectedSlot = inventoryUI.UISlots[index];
    }
}
