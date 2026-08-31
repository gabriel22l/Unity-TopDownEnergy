using UnityEngine;

public class ItemWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField] private WorldItem worldItem;
    
    public void Interact(InteractionContext interactionContext)
    {
        worldItem.TryPickUp(interactionContext);
    }
    public void InteractSecondary(InteractionContext interactionContext)
    {
        worldItem.TryPickUp(interactionContext);
    }
}