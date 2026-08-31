using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryUIController  inventoryUI;
    [SerializeField] private InventoryUIController hotbarInventoryUI;

    public void Initialize(InventoryViewModel inventoryViewModel, HotbarViewModel hotbarViewModel)
    {
        if (inventoryViewModel == null || hotbarViewModel == null || hotbarViewModel.InventoryVM == null 
            || inventoryUI == null || hotbarInventoryUI == null)
        {
            Debug.LogWarning("PlayerInventoryUI.Initialize() called with null parameters or null serialized references");
            return;
        }
        inventoryUI.Initialize(inventoryViewModel);
        hotbarInventoryUI.Initialize(hotbarViewModel.InventoryVM);
    }
}
