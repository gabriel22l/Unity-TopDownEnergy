using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Input -> UIController
    [SerializeField] private PlayerContext playerContext;
    [SerializeField] private UIController uiController;
    
    //inventory -> UI
    private InventoryViewModel invViewModel;
    private void Awake()
    {   //wire input to UI
        
        Application.targetFrameRate = 60;
        
        uiController.Initialize(playerContext);
        playerContext.Initialize(uiController);
        playerContext.PlayerInput.Initialize(uiController);
        
        //wire inventory data to UI
        InventoryUIController inventoryUIController = uiController.InventoryUIController;
        invViewModel = new InventoryViewModel(playerContext.InventoryController);
        
        inventoryUIController.Initialize(invViewModel);
    }
    private void OnDestroy() //unsubscribe invViewModel from InvController
    {
        invViewModel?.Dispose();
    }
}
