using UnityEngine;

public static class ItemDropUtility 
{
    public static void DropItem(ItemData itemData, int amount, Vector3 dropPosition)
    {
        if(itemData == null || itemData.itemPrefab == null || amount <= 0) return;
        
        GameObject itemPrefab = itemData.itemPrefab;
        GameObject spawnedObj = 
            Object.Instantiate(itemPrefab, dropPosition, Quaternion.identity); //instantiate object
        
        WorldItem worldItem = spawnedObj.GetComponent<WorldItem>();
        if(worldItem != null) worldItem.amount = amount; 
    }
}
