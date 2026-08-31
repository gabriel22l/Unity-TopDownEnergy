using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour, IInteractable
{
    [SerializeField] private WorldItem worldItem;
    [SerializeField] private Light2D light2d;

    public void Interact(InteractionContext ctx){}
    public void InteractSecondary(InteractionContext ctx)
    {
        int remaining = worldItem.TryPickUp(ctx);
        if(remaining <= 0) light2d.enabled = false;
    }
}
