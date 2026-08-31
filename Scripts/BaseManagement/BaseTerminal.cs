using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private BaseManager baseManager;
    private BaseManagerViewModel viewModel;
    private TerminalUIController terminalUI;

    private void Awake()
    {
        if (baseManager == null)
        {
            Debug.LogWarning("null BaseManager");
            return;
        }
        viewModel = new BaseManagerViewModel(baseManager);
    }
    private void OnDestroy()
    {
        if (terminalUI != null)
        {
            terminalUI.OnUIClose -= OnInteractableUIClose;
            terminalUI = null;
        }
        baseManager?.UnBind();
        viewModel?.Dispose();
    }
    public void Interact(InteractionContext interactionContext)
    {
        if (terminalUI != null) terminalUI.OnUIClose -= OnInteractableUIClose;
        baseManager.Bind(interactionContext.playerContext.InventoryController, interactionContext.playerContext.structureRecipes);
        interactionContext.playerContext.UiController.RequestOpenTerminalMenu(viewModel);

        terminalUI = interactionContext.playerContext.UiController.BaseTerminalMenu;
        terminalUI.OnUIClose += OnInteractableUIClose;
    }
    public void InteractSecondary(InteractionContext interactionContext)
    {
        return;
    }
    private void OnInteractableUIClose()
    {
        baseManager?.UnBind();
        if(terminalUI != null) terminalUI.OnUIClose -= OnInteractableUIClose;
    }
}
