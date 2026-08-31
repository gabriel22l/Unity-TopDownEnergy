using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RecipeUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private CraftingUIController craftingUIController;
    private CraftingRecipeViewData viewData;
    private int index;
    [SerializeField] private Image background;
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemNameText;

    //Binds to craftingController, wires button onClick to RequestCraft with this slot's index
    public void Initialize(CraftingUIController craftingController)
    {
        this.craftingUIController = craftingController;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => craftingController.RequestCraft(index));
    }
    
    //Updates slot visuals and interactability from viewData
    public void SetData(CraftingRecipeViewData viewData)
    {
        this.viewData = viewData;
        this.index = viewData.index;
        icon.sprite = viewData.itemIcon;
        itemNameText.text = $"{viewData.itemName} x{viewData.outputAmount}";
        button.interactable = viewData.canCraft;
    }
    
    #region Pointer Events
    //Shows details panel for this recipe
    public void OnPointerEnter(PointerEventData eventData)
    {
        craftingUIController?.ShowDetails(viewData, viewData.itemName);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        craftingUIController?.HideDetails();
    }
    
    //Reserved for future use (e.g. tooltip follow)
    public void OnPointerMove(PointerEventData eventData)
    {
        
    }
    #endregion
}
