using System;
using UnityEngine;

public class ActiveItemController : MonoBehaviour
{
    [SerializeField] HotbarController hotbarController;
    [SerializeField] PlayerContext playerContext;
    [SerializeField] PlayerInput  playerInput;
    [SerializeField] private Transform activeItemPivot;
    
    private ItemData currentItem;
    private HeldItem heldObject;
    private int currentSlotIndex = -1;
    
    private ItemActionContext itemActionContext;
    
    #region Unity Events
    private void Awake()
    {
        itemActionContext = new ItemActionContext(playerContext, currentSlotIndex);
    }
    private void OnEnable()
    {
        if (hotbarController)
        {
            hotbarController.OnItemSelectionChanged += UpdateSelectedItem;
        }
        else
        {
            Debug.LogWarning("HotbarController is null");
        }
        if (playerInput)
        {
            playerInput.PrimaryActionEvent += OnPrimaryAction;
            playerInput.SecondaryActionEvent += OnSecondaryAction;
        }
        else
        {
            Debug.LogWarning("playerInput is null");
        }
    }
    private void OnDisable()
    {
        if(hotbarController != null)
            hotbarController.OnItemSelectionChanged -= UpdateSelectedItem;
        if(playerInput != null)
        {
            playerInput.PrimaryActionEvent -= OnPrimaryAction;
            playerInput.SecondaryActionEvent -= OnSecondaryAction;
        }
    }
    #endregion
    private void UpdateSelectedItem(ItemData data, int index)
    {
        //Debug.Log(data == null ? $"nullData, index: {index}" : $"{data.itemName}, index: {index}");
        this.currentSlotIndex = index;
        itemActionContext.index = index;
        UpdateActiveItem(data);
    }
    // Returns if same Item, sets current ItemData
    // Instantiates held object
    // Calls HeldItem.CancelAction before destroying
    private void UpdateActiveItem(ItemData itemData)
    {
        if (activeItemPivot == null)
        {
            Debug.LogWarning("ActiveItemPivot is null");
            return;
        }
        if(itemData == currentItem) return;
        
        currentItem = itemData;
        if(heldObject != null)
        {
            heldObject.CancelAction(itemActionContext);
            Destroy(heldObject?.gameObject);
        }
        if (itemData != null && itemData.heldItemPrefab != null)
        {
            heldObject = Instantiate(itemData.heldItemPrefab, activeItemPivot);
        }
    }
    private void OnPrimaryAction()
    {
        heldObject?.PrimaryAction(itemActionContext);
    }
    private void OnSecondaryAction()
    {
        heldObject?.SecondaryAction(itemActionContext);
    }
}

public class ItemActionContext
{
    public PlayerContext playerContext;
    public int index;
    public ItemActionContext (PlayerContext playerContext, int index)
    {
        this.playerContext = playerContext;
        this.index = index;
    }
}