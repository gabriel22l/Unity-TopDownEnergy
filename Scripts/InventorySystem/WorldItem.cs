using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] public ItemData itemData;
    [SerializeField] private PickUpHandler animator;
    public int amount = 1;

    public int TryPickUp(InteractionContext ctx)
    {
        InventoryController inv = ctx.playerContext.InventoryController;
        int remaining = inv.AddItem(itemData, amount);
        if (remaining == 0)
        {
           if(animator) animator.PlayPickUpAndDestroy(ctx.playerContext.transform);
           else Destroy(gameObject);
           return 0;
        }
        amount = remaining;
        return amount;
    }
}
