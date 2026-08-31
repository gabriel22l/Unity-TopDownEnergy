using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalUIController : MonoBehaviour, IInteractableMenu
{
    [SerializeField] private TerminalUIPage terminalUIPage;
    [SerializeField] private StructuresUIPage structuresUIPage;
    [SerializeField] private InfoUIPage infoUIPage;

    private MenuController menuController;
    private GameObject currentPage;
    
    public event Action OnUIClose;
    
    // Initializes each page
    // Subscribes to StructuresUIPage.OnBuildSuccess
    // Subscribes StructuresUIPage to MenuController.OnTabChange
    // Enables default page
    public void Initialize(BaseManagerViewModel viewModel)
    {
        if (terminalUIPage != null) 
            terminalUIPage.Initialize(this, viewModel);
        
        if (structuresUIPage != null)
        {
            structuresUIPage.Initialize(viewModel);
            structuresUIPage.OnBuildSuccess += OnStructureBuild;
        }
        
        if (infoUIPage != null) 
            infoUIPage.Initialize(viewModel);
        
        if (terminalUIPage != null)
        {
            SwitchActivePage(terminalUIPage.gameObject);
        }

        menuController = GetComponent<MenuController>();
        if (menuController != null && structuresUIPage != null)
        {
            menuController.OnTabChange += structuresUIPage.OnTabChange;
        }
    }

    // Unsubscribes from StructuresUIPage.OnStructureBuild
    // Unsubscribes StructuresUIPage from MenuController.OnTabChange
    // Returns to TerminalUIPage
    private void OnDisable()
    {
        if (structuresUIPage != null)
            structuresUIPage.OnBuildSuccess -= OnStructureBuild;
        
        if (menuController != null && structuresUIPage != null)
            menuController.OnTabChange -= structuresUIPage.OnTabChange;

        GoBackToTerminal();
        
        OnUIClose?.Invoke();
    }

    // Enables StructuresUIPage and
    // sets selected base slot index
    public void EnableStructurePage(int slotIndex)
    {
        if (structuresUIPage == null) return;
        structuresUIPage.SetSelectedBaseSlot(slotIndex);
        SwitchActivePage(structuresUIPage.gameObject);
    }

    public void GoBackToTerminal()
    {
        if (terminalUIPage == null) return;
        SwitchActivePage(terminalUIPage.gameObject);
    }

    private void SwitchActivePage(GameObject page)
    {
        if (page == null) return;
        if (currentPage == page) return;
        if (currentPage != null) currentPage.SetActive(false);
        page.SetActive(true);
        currentPage = page;
    }

    // Removes onClick listeners and adds new listener
    public static void AddOnClickEvent(GameObject uiSlot, int index, Action<int> callback)
    {
        if (uiSlot == null || callback == null) return;
        var button = uiSlot.GetComponent<UnityEngine.UI.Button>();
        if (button == null)
        {
            Debug.LogError("UI Slot GameObject does not have a Button component");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(index));
    }

    private void OnStructureBuild()
    {
        GoBackToTerminal();
    }
}