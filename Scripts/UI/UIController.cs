using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    private PlayerInput playerInput;
    //the ui is meant to be wired to the player instance,
    //holding the reference allows for access to player data and reinforces the binding
    private PlayerContext playerContext;
    
    [SerializeField] private GameObject playerMenuObject;
    [field:SerializeField] public TerminalUIController BaseTerminalMenu {get; private set;}
    [field:SerializeField] public StorageUIController StorageMenu { get; private set; }
    
    [field:SerializeField] public PlayerInventoryUI PlayerInvUI { get; private set; } 
    [field:SerializeField] public CraftingUIController CraftingUIController { get; private set; }
    
    [SerializeField] private HotbarOverlayUI hotbarOverlayUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;
    
    private GameObject currentActiveMenu;
    
    public event Action OnUIOpen;
    public event Action OnUIClose;

    private GameManagerVM gameManagerVm;
    
    #region Initialization/subscriptions/Unity lifetime
    public void Initialize(PlayerContext  ctx, GameManagerVM vm)
    {
        this.gameManagerVm = vm;
        this.playerContext = ctx;
        this.playerInput = ctx.PlayerInput;
        playerInput.MenuOpenInputEvent += OpenPlayerMenu;
        playerInput.MenuCloseInputEvent += CloseMenu;
        playerInput.PauseInputEvent += OpenPauseMenu;
    }
    private void OnDisable()
    {
        if (playerInput == null) return;
        playerInput.MenuOpenInputEvent -= OpenPlayerMenu;
        playerInput.MenuCloseInputEvent -= CloseMenu;
        playerInput.PauseInputEvent -= OpenPauseMenu;
    }
    private void Start()
    {
        InitializeGameplayUI();
    }
    #endregion
    private void InitializeGameplayUI()
    {
        if (playerContext?.HotbarViewModel?.InventoryVM == null)
        {
            Debug.LogWarning("PlayerContext, HotbarVM or HotbarVM.InventoryVM is null");
            return;
        }
        hotbarOverlayUI.Initialize(playerContext.HotbarViewModel);
    }
    #region Player Input events menu handling
    private void OpenPlayerMenu()
    {
        if(playerContext?.InventoryViewModel == null) return;
        PlayerInvUI.Initialize(playerContext.InventoryViewModel, playerContext.HotbarViewModel);
        CraftingUIController.Initialize(playerContext.CraftingViewModel);
        SetActiveMenu(playerMenuObject);
    }
    public void CloseMenu()
    {
        if(currentActiveMenu == null) return;
        //disable current active menu and set current active to null
        currentActiveMenu.SetActive(false);
        currentActiveMenu = null;
        
        OnUIClose?.Invoke();
        
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
    }
    private void OpenPauseMenu()
    {
        if (pauseMenuUI == null) return;
        
        pauseMenuUI.Initialize(gameManagerVm, this);
        SetActiveMenu(pauseMenuUI.gameObject);
    }
    #endregion
    #region External menu requests
    public void RequestOpenTerminalMenu(BaseManagerViewModel bmViewModel)
    {
        if(BaseTerminalMenu == null || bmViewModel == null) return;
        BaseTerminalMenu.Initialize(bmViewModel);
        SetActiveMenu(BaseTerminalMenu.gameObject);
    }
    public bool RequestOpenStorageMenu(InventoryViewModel storageViewModel, InventoryViewModel playerViewModel, HotbarViewModel hotbarViewModel)
    {
        if(StorageMenu  == null || storageViewModel == null || playerViewModel == null) return false;
        StorageMenu.Initialize(storageViewModel, playerViewModel, hotbarViewModel);
        SetActiveMenu(StorageMenu.gameObject);
        return true;
    }
    #endregion
    #region Helper methods
    private void SetActiveMenu(GameObject menu)
    {
        if (currentActiveMenu != null || menu == null) return;
        menu.SetActive(true);
        currentActiveMenu = menu;
        
        OnUIOpen?.Invoke();
    }
    #endregion
}