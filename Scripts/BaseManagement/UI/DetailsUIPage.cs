using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DetailsUIPage : MonoBehaviour
{
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private TextMeshProUGUI detailsNameText;
    [SerializeField] private GameObject costContainer;
    [SerializeField] private UISlot costRowPrefab;
    
    [SerializeField] private Color32 insufficientResourceTextColor = new Color32(200, 0, 0, 255);
    [SerializeField] private Color insufficientResourceIconColor = new Color(1, 1, 1, 0.6f);
    [SerializeField] private Sprite energyIcon;
    
    private List<UISlot> rows = new List<UISlot>();
    
    //sets panel active and fills data
    public void SetDetailsPageData(StructureRecipeData recipeData)
    {
        //check null
        if(detailsPanel == null || detailsNameText == null || costContainer == null || costRowPrefab == null)
        {
            Debug.LogError("DetailsUIPage is missing references to UI components.");
            return;
        }
        
        if(!detailsPanel.activeInHierarchy) 
            detailsPanel.SetActive(true);
        RefreshRows(recipeData);
    }
    
    //sets name text, syncs slot count,
    //sets resource values & sets energy value if recipe.requiresEnergy
    private void RefreshRows(StructureRecipeData recipeData)
    {
        detailsNameText.text = recipeData.structureName;
        
        bool showEnergyRow = recipeData.requiresEnergy;
        List <ResourceViewData> resourceDataList = recipeData.resources;
        int targetRows = recipeData.requiresEnergy ? resourceDataList.Count + 1 : resourceDataList.Count;
        
        //if more rows than data, remove excess
        if (targetRows < rows.Count)
        {
            for (int i = rows.Count - 1; i >= targetRows; i--)
            {
                Destroy(rows[i].gameObject);
                rows.RemoveAt(i);
            }
        }
        
        // if more data than rows, add rows
        if(targetRows > rows.Count)
        {
            for (int i = rows.Count; i < targetRows; i++)
            {
                AddRow();
            }
        }
        
        //Set Resource Values
        for (int i = 0; i < resourceDataList.Count; i++)
        {
            SetRowValues(rows[i], resourceDataList[i]);
        }
        
        //Handle Energy Row
        if(showEnergyRow)
        {
            SetEnergyRowValues(rows[rows.Count - 1], recipeData);
        }
    }
    
    // Instantiates & Adds to rows List
    private void AddRow()
    {
        UISlot row = Instantiate(costRowPrefab, costContainer.transform);
        rows.Add(row);
    }
    private void SetRowValues(UISlot row, ResourceViewData resourceData)
    {
        row.SetValues(resourceData.requiredAmount.ToString(), resourceData.resourceIcon);
        row.SetIconColor(resourceData.hasEnough ? Color.white : insufficientResourceIconColor);
        row.SetTextColor(resourceData.hasEnough ? Color.black : insufficientResourceTextColor);
    }
    private void SetEnergyRowValues(UISlot row, StructureRecipeData recipeData)
    {
        row.SetValues(recipeData.energyCost.ToString(), energyIcon);
        row.SetIconColor(recipeData.enoughEnergy ? Color.white : insufficientResourceIconColor);
        row.SetTextColor(recipeData.enoughEnergy ? Color.black : insufficientResourceTextColor);
    }

    public void HideDetails()
    {
        detailsPanel?.SetActive(false);
    }
}