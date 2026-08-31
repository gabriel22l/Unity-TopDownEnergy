using System;
using UnityEngine;
using System.Collections.Generic;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] private RecipeUISlot recipePrefab;
    [SerializeField] private Transform recipeGrid;
    [SerializeField] private CraftingDetailsUI detailsUI;
    private List<RecipeUISlot> recipeSlots = new List<RecipeUISlot>();
    private CraftingViewModel viewModel;

    #region Initialization & Unity Callbacks
    private void OnEnable()
    {
        if (viewModel != null)
        {
            viewModel.OnDataChanged -= RefreshUI;
            viewModel.OnDataChanged += RefreshUI;
        }
        RefreshUI();
    }
    private void OnDisable()
    {
        if (viewModel != null)
        {
            viewModel.OnDataChanged -= RefreshUI;
        }
        detailsUI?.Hide();
    }
    
    //Binds and subscribes to new ViewModel if the object is active
    //refreshes UI
    //unsubscribes from old ViewModel if not null
    public void Initialize(CraftingViewModel craftingViewModel)
    {
        if(viewModel != null)
            viewModel.OnDataChanged -= RefreshUI; 
        
        this.viewModel = craftingViewModel;
        
        if (viewModel != null && gameObject.activeInHierarchy)
            viewModel.OnDataChanged += RefreshUI;
        
        RefreshUI();
    }
    #endregion
    
    //Called on ViewModel.OnDataChanged, gets new view data list
    //removes slots if recipes removed, adds slots if recipes added
    //updates all slots with new data, calls detailsUI.Refresh
    private void RefreshUI()
    {
        if (viewModel == null) return;
        List<CraftingRecipeViewData> viewData = viewModel.GetAllRecipeViewData();
        
        //if more UI slots than data, remove out of range slots
        if(viewData.Count < recipeSlots.Count)
        {
            RemoveOutOfRangeSlots(viewData);
        }
        
        for(int i = 0; i < viewData.Count; i++)
        {
            //if more data than UI slots, add new slots for remaining data
            if (i >= recipeSlots.Count)
            {
                AddSlot(viewData[i]);
                continue;
            }
            recipeSlots[i].SetData(viewData[i]);
        }
        detailsUI?.Refresh(viewData);
    }
    
    //Destroys and removes slots beyond the recipe count (emd of the list).
    private void RemoveOutOfRangeSlots(List<CraftingRecipeViewData> viewData)
    {
        for(int j = viewData.Count; j < recipeSlots.Count; j++)
        {
            if(recipeSlots[j] == null) continue;
            Destroy(recipeSlots[j].gameObject);
        }
        recipeSlots.RemoveRange(viewData.Count, recipeSlots.Count - viewData.Count);
    }
    
    //instantiates UI slot, adds slot to list, initializes it and sets its data from received ViewData
    private void AddSlot(CraftingRecipeViewData viewData)
    {
        RecipeUISlot slot = Instantiate(recipePrefab, recipeGrid);
        recipeSlots.Add(slot);
        slot.Initialize(this);
        slot.SetData(viewData);
    }

    public void RequestCraft(int index)
    {
        viewModel?.RequestCraft(index);
    }
    
    //Called by button OnClick, wired in inspector
    public void TogglePage()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
    
    //Shows the details panel for the given recipe. Called by RecipeUISlot on pointer enter.
    public void ShowDetails(CraftingRecipeViewData cViewData, string recipeName)
    {
        detailsUI?.Show(cViewData, recipeName);
    }
    
    //Hides the details panel. Called by RecipeUISlot on pointer exit.
    public void HideDetails()
    {
        detailsUI?.Hide();
    }
}