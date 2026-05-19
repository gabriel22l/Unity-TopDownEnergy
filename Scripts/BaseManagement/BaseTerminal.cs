using System.Collections.Generic;
using UnityEngine;

public class BaseTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private BaseManager baseManager;
    public void Interact(InteractionContext interactionContext)
    {
        baseManager.Bind(interactionContext.playerContext.InventoryController);
        List<StructureRecipeSO> playerStructureRecipes = interactionContext.playerContext.structureRecipes;
        interactionContext.playerContext.UiController.RequestOpenTerminalMenu(baseManager,  playerStructureRecipes);
    }
}
