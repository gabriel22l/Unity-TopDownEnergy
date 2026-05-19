using UnityEngine;

[CreateAssetMenu(fileName = "itemDataSO", menuName = "Scriptable Objects/itemDataSO")]
public class itemDataSO : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public int maxStackAmount;
}
