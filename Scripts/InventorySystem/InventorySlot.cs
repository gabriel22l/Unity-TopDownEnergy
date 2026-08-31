[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int amount;
    public bool IsEmpty => itemData == null || amount <= 0;
    public bool IsFull => itemData != null && amount >= itemData.maxStackAmount;
}
