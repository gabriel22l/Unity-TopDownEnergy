using System;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryController storageInventory;
    private InventoryViewModel  storageIVM;

    private StorageUIController storageUI;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    
    [SerializeField] private WorldItem worldItem;

    #region Unity lifecycle
    private void Awake()
    {
        if (storageInventory == null && !TryGetComponent(out storageInventory)) 
        {
            Debug.LogError("Storage: InventoryController not found.");
            return;
        }
        storageIVM = new InventoryViewModel(storageInventory, 0, storageInventory.slotAmount);
    }
    private void OnDestroy()
    {
        //Safety if subscription is still active when object is destroyed, should not be but just in case
        if(storageUI != null)
            storageUI.OnUIClose -= OnInteractableUIClose;
        storageUI = null;
    }
    #endregion
    public void Interact(InteractionContext ctx)
    {
        if (storageUI != null) storageUI.OnUIClose -= OnInteractableUIClose;
        
        UIController uiC = ctx.playerContext.UiController;
        bool uiSuccess = 
            uiC.RequestOpenStorageMenu(storageIVM, ctx.playerContext.InventoryViewModel, ctx.playerContext.HotbarViewModel);
        if (uiSuccess && uiC.StorageMenu != null)
        {
            storageUI = uiC.StorageMenu;
            storageUI.OnUIClose += OnInteractableUIClose;
            if(spriteRenderer != null && openSprite != null && closedSprite != null)
                spriteRenderer.sprite = openSprite;
        }
    }
    private void OnInteractableUIClose()
    {
        if(spriteRenderer != null && openSprite != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
        if(storageUI != null)
            storageUI.OnUIClose -= OnInteractableUIClose;
        storageUI = null;
    }
    public void InteractSecondary(InteractionContext ctx)
    {
        if (!storageInventory.IsEmpty) return;
        worldItem.TryPickUp(ctx);
    }
}