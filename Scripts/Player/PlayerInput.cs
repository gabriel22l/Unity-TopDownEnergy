using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    private InputActions inputActions;
    private InputActionMap currentActiveMap;
    public Vector2 MoveInput {get; private set;}

    private UIController uiController; //reference for subscription to event

    public event Action MenuOpenInputEvent;
    public event Action MenuCloseInputEvent;
    public event Action InteractEvent;
    public event Action InteractHoldEvent;
    public event Action<int> HotbarInputEvent;
    public event Action PrimaryActionEvent;
    public event Action SecondaryActionEvent;
    public event Action PauseInputEvent;
    
    private Dictionary<InputAction, int> hotbarInputIndexDictionary;

    #region initialization and UI event handlers
    public void Initialize(UIController uiC)
    {
        this.uiController = uiC;
        uiController.OnUIOpen += OnUIOpen;
        uiController.OnUIClose += OnUIClose;
    }
    private void OnUIOpen()
    {
        SwitchActionMap(inputActions.UI);
    }
    private void OnUIClose()
    {
        SwitchActionMap(inputActions.Gameplay);
    }
    #endregion
    #region  Input Action Callbacks/ unity lifecycle
    private void Awake()
    {
        inputActions = new InputActions();
        
        hotbarInputIndexDictionary = new Dictionary<InputAction, int>
        {
            { inputActions.Gameplay.Hotbar0, 0 },
            { inputActions.Gameplay.Hotbar1, 1 },
            { inputActions.Gameplay.Hotbar2, 2 },
            { inputActions.Gameplay.Hotbar3, 3 }
        };
    }
    private void OnDestroy()
    {
        if(uiController != null)
        {
            uiController.OnUIOpen -= OnUIOpen;
            uiController.OnUIClose -= OnUIClose;
        }
        
        inputActions?.Dispose();
    }
    private void OnEnable()
    {
        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Move.canceled += OnMove;
        inputActions.Gameplay.OpenMenu.performed += OnOpenMenuInput;
        inputActions.Gameplay.Interact.performed += OnInteract;
        inputActions.Gameplay.InteractSecondary.performed += OnInteractHold;
        HandleHotbarSubscription(true);
        inputActions.Gameplay.PrimaryAction.performed += OnPrimaryAction;
        inputActions.Gameplay.SecondaryAction.performed += OnSecondaryAction;
        inputActions.Gameplay.OpenPauseMenu.performed += OnPauseInput;

        inputActions.UI.CloseMenu.performed += OnCloseMenuInput;
        inputActions.UI.Disable();
        SwitchActionMap(inputActions.Gameplay);
    }
    private void OnDisable()
    {
        inputActions.Gameplay.Move.performed -= OnMove;
        inputActions.Gameplay.Move.canceled -= OnMove;
        inputActions.Gameplay.OpenMenu.performed -= OnOpenMenuInput;
        inputActions.Gameplay.Interact.performed -= OnInteract;
        inputActions.Gameplay.InteractSecondary.performed -= OnInteractHold;
        HandleHotbarSubscription(false);
        inputActions.Gameplay.PrimaryAction.performed -= OnPrimaryAction;
        inputActions.Gameplay.SecondaryAction.performed -= OnSecondaryAction;
        inputActions.Gameplay.OpenPauseMenu.performed -= OnPauseInput;
        
        inputActions.UI.CloseMenu.performed -= OnCloseMenuInput;
        
        currentActiveMap?.Disable();
        currentActiveMap = null;
    }
    private void HandleHotbarSubscription(bool sub)
    {
        if (sub)
        {
            inputActions.Gameplay.Hotbar0.performed += OnHotbarInput;
            inputActions.Gameplay.Hotbar1.performed += OnHotbarInput;
            inputActions.Gameplay.Hotbar2.performed += OnHotbarInput;
            inputActions.Gameplay.Hotbar3.performed += OnHotbarInput;
        }
        else
        {
            inputActions.Gameplay.Hotbar0.performed -= OnHotbarInput;
            inputActions.Gameplay.Hotbar1.performed -= OnHotbarInput;
            inputActions.Gameplay.Hotbar2.performed -= OnHotbarInput;
            inputActions.Gameplay.Hotbar3.performed -= OnHotbarInput;
        }
    }
    #endregion
    #region input callback methods
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    private void OnOpenMenuInput(InputAction.CallbackContext ctx)
    {
        MenuOpenInputEvent?.Invoke();
    }
    private void OnCloseMenuInput(InputAction.CallbackContext ctx)
    {
        MenuCloseInputEvent?.Invoke();
    }
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        InteractEvent?.Invoke();
    }
    private void OnInteractHold(InputAction.CallbackContext ctx)
    {
        InteractHoldEvent?.Invoke();
    }
    private void OnHotbarInput(InputAction.CallbackContext ctx)
    {
        if(hotbarInputIndexDictionary.TryGetValue(ctx.action, out int index))
            HotbarInputEvent?.Invoke(index);
    }
    private void OnPrimaryAction(InputAction.CallbackContext ctx)
    {
        PrimaryActionEvent?.Invoke();
    }
    private void OnSecondaryAction(InputAction.CallbackContext ctx)
    {
        SecondaryActionEvent?.Invoke();
    }
    private void OnPauseInput(InputAction.CallbackContext ctx)
    {
        PauseInputEvent?.Invoke();
    }
    #endregion
    private void SwitchActionMap(InputActionMap actionMap)
    {
        if (actionMap == currentActiveMap) return;
        currentActiveMap?.Disable();
        actionMap.Enable();
        currentActiveMap = actionMap;
    }
}