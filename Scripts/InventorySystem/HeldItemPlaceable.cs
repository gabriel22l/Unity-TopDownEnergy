public class HeldItemPlaceable : HeldItem
{
    public override void SecondaryAction(ItemActionContext ctx)
    {
        if(ctx == null || ctx.index < 0 || ctx.playerContext?.InventoryController == null)
            return;
        ctx.playerContext.InventoryController.DropItem(ctx.index);
    }
}