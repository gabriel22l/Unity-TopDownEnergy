using UnityEngine;

[CreateAssetMenu(fileName = "itemData", menuName = "Scriptable Objects/itemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public HeldItem heldItemPrefab;
    public int maxStackAmount;
}
