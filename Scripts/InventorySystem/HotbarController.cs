using UnityEngine;
using System;

public class HotbarController : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private InventoryController inventoryController;
    [field: SerializeField] public int SlotAmount { get; private set; } = 4;
    public int SelectedIndex { get; private set; }
    
    public event Action OnSelectionChanged;
    public event Action<ItemData, int> OnItemSelectionChanged;

    #region Unity Events
    private void OnEnable()
    {
        if (input == null || inventoryController == null)
        {
            Debug.LogWarning("null PlayerInput or InventoryController");
            return;
        }
        input.HotbarInputEvent += SetSelectedIndex;
        inventoryController.OnInventoryChanged += UpdateItemSelection;
    }
    private void OnDisable()
    {
        if(input)
        {
            input.HotbarInputEvent -= SetSelectedIndex;
        }
        if (inventoryController)
        {
            inventoryController.OnInventoryChanged -= UpdateItemSelection;
        }
    }
    private void Start()
    {
        UpdateItemSelection();
    }
    #endregion
    public void SetSelectedIndex(int index)
    {
        if (index < 0 || index >= SlotAmount)
        {
            Debug.LogError($"Invalid index {index}");
            return;
        }
        SelectedIndex = index;
        OnSelectionChanged?.Invoke();
        
        UpdateItemSelection();
    }
    private void UpdateItemSelection()
    {
        if (inventoryController == null || 
            inventoryController.Slots == null || 
            inventoryController.Slots.Length < SlotAmount)
        {
            return;
        }
        
        ItemData data = inventoryController?.Slots[SelectedIndex]?.itemData;
        OnItemSelectionChanged?.Invoke(data, SelectedIndex);
    }
}