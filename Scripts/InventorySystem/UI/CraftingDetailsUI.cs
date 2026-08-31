using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftingDetailsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI craftingName;
    [SerializeField] private GridLayoutGroup rowsGrid;
    [SerializeField] private UISlot resourceRowPrefab;

    [SerializeField] private RectTransform pageRectTransform;
    [SerializeField] private RectTransform gridRect;
    private float detailsPagePadding = 100f;
    
    private List<UISlot> resourceRows = new List<UISlot>();

    private int currentRecipeIndex = -1;
    
    [SerializeField] private UIAnimator uiAnimator;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if(canvasGroup != null) canvasGroup.alpha = 0f;
    }
    private void OnDisable()
    {
        currentRecipeIndex = -1;
        canvasGroup.alpha = 0f;
    }
    public void Show(CraftingRecipeViewData cViewData, string itemName)
    {
        currentRecipeIndex = cViewData.index;
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        
        uiAnimator?.FadeIn();
        
        craftingName.text = itemName;
        RefreshRows(cViewData.resourcesViewData);
    }
    public void Hide()
    {
        currentRecipeIndex = -1;
        uiAnimator?.FadeOut();
    }
    public void Refresh(List<CraftingRecipeViewData> cViewDataL)
    {
        if(!gameObject.activeInHierarchy || currentRecipeIndex < 0) return;
        List<ResourceViewData> resourceViewDataL = cViewDataL[currentRecipeIndex].resourcesViewData;
        RefreshRows(resourceViewDataL);
    }
    private void RefreshRows(List<ResourceViewData> rViewDataL)
    {
        if(rViewDataL == null || !gameObject.activeInHierarchy) return;
        if(rViewDataL.Count < resourceRows.Count) //if there are more rows than resources, clear the extra rows
        {
            RemoveOutOfRangeRows(rViewDataL);
        }
        for(int i = 0; i < rViewDataL.Count; i++)
        {
            if (i >= resourceRows.Count)
            {
                AddRow(rViewDataL[i]);
                continue;
            }
            resourceRows[i].SetValues(rViewDataL[i].requiredAmount.ToString(), rViewDataL[i].resourceIcon);
            resourceRows[i].SetIconColor(rViewDataL[i].hasEnough ? Color.white : Color.red);
            resourceRows[i].SetTextColor(rViewDataL[i].hasEnough ? Color.black : Color.red);
        }
        HandleTransform();
    }
    private void AddRow(ResourceViewData rViewData)
    {
        UISlot row = Instantiate(resourceRowPrefab, rowsGrid.transform);
        resourceRows.Add(row);
        row.SetValues(rViewData.requiredAmount.ToString(), rViewData.resourceIcon);
        row.SetIconColor(rViewData.hasEnough ? Color.white : Color.red);
        row.SetTextColor(rViewData.hasEnough ? Color.black : Color.red);
    }
    private void RemoveOutOfRangeRows(List<ResourceViewData> rViewDataL)
    {
        for(int j = rViewDataL.Count; j < resourceRows.Count; j++)
        {
            if(resourceRows[j] == null) continue;
            Destroy(resourceRows[j].gameObject);
        }
        resourceRows.RemoveRange(rViewDataL.Count, resourceRows.Count - rViewDataL.Count);
    }
    private void HandleTransform()
    {
        if (pageRectTransform  == null || gridRect == null)
        {
            Debug.LogError("PageRectTransform or GridRect is null");
            return;
        }
        float cellSizeY = rowsGrid.cellSize.y;
        int cellNum = resourceRows.Count;
        
        gridRect.sizeDelta = new Vector2(gridRect.sizeDelta.x, 
        (cellSizeY + rowsGrid.spacing.y) * Mathf.CeilToInt((float)cellNum / rowsGrid.constraintCount));
        
        pageRectTransform.sizeDelta = new Vector2(pageRectTransform.sizeDelta.x, gridRect.sizeDelta.y + detailsPagePadding);
    }
}