using UnityEngine;
using System;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    private PlayerInput playerInput;
    
    [SerializeField] private GameObject playerMenuObject;
    [SerializeField] private GameObject baseTerminalMenu;
    
    //purpose is to get this on game manager for wiring inventory data to UI
    [field:SerializeField] public InventoryUIController InventoryUIController { get; private set; } 
    
    private GameObject currentActiveMenu;
    
    public event Action OnUIOpen;
    public event Action OnUIClose;
    public void Initialize(PlayerContext  playerContext)
    {
        this.playerInput = playerContext.PlayerInput;
        playerInput.MenuOpenInputEvent += OpenPlayerMenu;
        playerInput.MenuCloseInputEvent += CloseMenu;
    }
    private void OnDisable()
    {
        if (playerInput == null) return;
        playerInput.MenuOpenInputEvent -= OpenPlayerMenu;
        playerInput.MenuCloseInputEvent -= CloseMenu;
    }
    private void OpenPlayerMenu()
    {
        SetActiveMenu(playerMenuObject);
    }
    private void CloseMenu()
    {
        if(currentActiveMenu == null) return;
        //disable current active menu and set current active to null
        currentActiveMenu.SetActive(false);
        currentActiveMenu = null;
        
        OnUIClose?.Invoke();
    }
    public void RequestOpenTerminalMenu(BaseManager baseManager, List<StructureRecipeSO>  playerRecipes)
    {
        SetActiveMenu(baseTerminalMenu);
        if(!baseTerminalMenu.TryGetComponent(out BaseTerminalUI baseTerminalUI)) //safety
        {
            Debug.LogError("BaseTerminalUI not found in baseTerminalMenu");
            return;
        }
        baseTerminalUI.Initialize(baseManager, playerRecipes); //pass down base manager instance
    }
    private void SetActiveMenu(GameObject menu)
    {
        if (currentActiveMenu != null || menu == null) return;
        menu.SetActive(true);
        currentActiveMenu = menu;
        
        OnUIOpen?.Invoke();
    }
}