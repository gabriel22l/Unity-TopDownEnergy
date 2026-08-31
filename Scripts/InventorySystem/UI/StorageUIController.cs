using UnityEngine;
using System;

public class StorageUIController : MonoBehaviour, IInteractableMenu
{
    [SerializeField] private InventoryUIController storageInvUIController;
    [SerializeField] private PlayerInventoryUI playerInvUI;

    public event Action OnUIClose;

    public void Initialize(InventoryViewModel storageIVM, InventoryViewModel playerIVM, HotbarViewModel hotbarVM)
    {
        if(storageInvUIController == null || playerInvUI == null  
                                          || hotbarVM == null || storageIVM == null || playerIVM == null)
        {
            Debug.LogError("StorageUIController: InventoryUIControllers not assigned in inspector or null Inventory view model");
            return;
        }
        
        storageInvUIController?.Initialize(storageIVM);
        playerInvUI?.Initialize(playerIVM,  hotbarVM);
    }

    private void OnDisable()
    {
        OnUIClose?.Invoke();
    }
}
