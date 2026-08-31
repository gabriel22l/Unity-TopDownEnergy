using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StructuresUIPage : MonoBehaviour
{
    [SerializeField] private DetailsUIPage detailsPage;
    [SerializeField] private Button buildButton;
    [SerializeField] private StructureRecipeUI recipeSlotPrefab;
    [SerializeField] private Transform recipeContainer;
    
    private BaseManagerViewModel bmViewModel;
    private List<StructureRecipeUI> recipeUISlots = new List<StructureRecipeUI>();
    private StructureRecipeUI selectedRecipeSlot;
    private int selectedBaseSlotIndex = -1;

    public event Action OnBuildSuccess;

    #region Initialization & Event Handling
    private void OnEnable()
    {
        if (bmViewModel == null) return;
        bmViewModel.OnDataChanged -= RefreshRecipeSlots;
        bmViewModel.OnDataChanged += RefreshRecipeSlots;
        RefreshRecipeSlots();
    }
    private void OnDisable()
    {
        ClearSelection();
        if (bmViewModel == null) return;
        bmViewModel.OnDataChanged -= RefreshRecipeSlots;
    }
    public void Initialize(BaseManagerViewModel viewModel)
    {
        if(this.bmViewModel != null) bmViewModel.OnDataChanged -= RefreshRecipeSlots;
        if(viewModel == null)
        {
            Debug.LogError("Null BaseManagerViewModel passed to StructuresUIPage");
            return;
        }
        
        this.bmViewModel = viewModel;
        if(gameObject.activeInHierarchy) bmViewModel.OnDataChanged += RefreshRecipeSlots;
    }
    #endregion
    public void SetSelectedBaseSlot(int slotIndex)
    {
        selectedBaseSlotIndex = slotIndex;
        if (buildButton != null) buildButton.interactable = false;
    }
    
    //get data, instantiate slot, set data, on click event to select recipe
    private void RefreshRecipeSlots()
    {
        List<StructureRecipeData> recipeDataList = bmViewModel.GetAllStructureRecipeData(selectedBaseSlotIndex);
        
        //remove out of range if uiSlots are more than data
        if(recipeUISlots.Count > recipeDataList.Count)
        {
            for(int i = recipeUISlots.Count - 1; i >= recipeDataList.Count; i--)
            {
                Destroy(recipeUISlots[i].gameObject);
                recipeUISlots.RemoveAt(i);
            }
        }
        
        //add slots if data is more than uiSlots
        if(recipeUISlots.Count < recipeDataList.Count)
        {
            for(int i = recipeUISlots.Count; i < recipeDataList.Count; i++)
            {
                AddSlot();
            }
        }
        
        // set updated data for current slots
        for (int i = 0; i < recipeDataList.Count; i++)
        {
            if(recipeUISlots[i] == null) continue;
            recipeUISlots[i].SetData(recipeDataList[i]);
        }
        
        UpdateSelection();
    }
    // instantiate, add to list & set on click event to select recipe
    private void AddSlot()
    {
        StructureRecipeUI slot = Instantiate(recipeSlotPrefab, recipeContainer);
        recipeUISlots.Add(slot);
        AddOnClickEvent(slot, SelectRecipeSlot);
    }
    public void SelectRecipeSlot(StructureRecipeUI slot)
    {
        if (slot == null)
        {
            ClearSelection();
            return;
        }
        selectedRecipeSlot?.SetSelected(false);
        
        selectedRecipeSlot = slot;
        slot.SetSelected(true);
        
        if(buildButton != null)buildButton.interactable = slot.RecipeData.canBuild;
        
        detailsPage?.SetDetailsPageData(slot.RecipeData);
    }
    private void ClearSelection()
    {
        selectedRecipeSlot?.SetSelected(false);
        selectedRecipeSlot = null;
        detailsPage?.HideDetails();
         if(buildButton != null) buildButton.interactable = false;
    }
    private void UpdateSelection()
    {
        if (selectedRecipeSlot == null)
        {
            ClearSelection();
            return;
        }
        buildButton.interactable = selectedRecipeSlot.RecipeData.canBuild;
        detailsPage?.SetDetailsPageData(selectedRecipeSlot.RecipeData);
    }
    private void AddOnClickEvent(StructureRecipeUI slot, Action<StructureRecipeUI> callback)
    {
        Button button = slot.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("StructureRecipeUI Slot GameObject does not have a Button component");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(slot));
    }
    public void OnBuildButtonClick()
    {
        if(bmViewModel == null || selectedRecipeSlot == null || selectedBaseSlotIndex < 0) return;
        bool success = bmViewModel.TryBuildRecipe(selectedBaseSlotIndex, selectedRecipeSlot.RecipeData.index);
        
        if(success)
        {
            ClearSelection();
            OnBuildSuccess?.Invoke();
        }
        buildButton.interactable = false;
    }
    public void OnTabChange()
    {
        ClearSelection();
    }
}