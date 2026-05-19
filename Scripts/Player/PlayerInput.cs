using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputActions inputActions;
    private InputActionMap currentActiveMap;
    public Vector2 MoveInput {get; private set;}

    private UIController uiController; //reference for subscription to event

    public event Action MenuOpenInputEvent;
    public event Action MenuCloseInputEvent;
    public event Action InteractEvent;

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
    }
    private void OnEnable()
    {
        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Move.canceled += OnMove;
        inputActions.Gameplay.OpenMenu.performed += OnOpenMenuInput;
        inputActions.Gameplay.Interact.performed += OnInteract;

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
        inputActions.UI.CloseMenu.performed -= OnCloseMenuInput;
        
        if(uiController != null)
        {
            uiController.OnUIOpen -= OnUIOpen;
            uiController.OnUIClose -= OnUIClose;
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
    #endregion
    private void SwitchActionMap(InputActionMap actionMap)
    {
        if (actionMap == currentActiveMap) return;
        currentActiveMap?.Disable();
        actionMap.Enable();
        currentActiveMap = actionMap;
    }
}