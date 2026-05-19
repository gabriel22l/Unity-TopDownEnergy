using DefaultNamespace;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseTerminalUI : MonoBehaviour
{
    [SerializeField] private Sprite energyIcon;
    [Header("Terminal Page UI Elements")]
    [SerializeField] private GameObject baseUISlotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GameObject terminalPage;
    
    [Header("Structure Page UI Elements")]
    [SerializeField] private GameObject structuresPage;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeSlotPrefab;

    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private TextMeshProUGUI detailsNameText;
    [SerializeField] private GameObject costContainer;
    [SerializeField] private GameObject costRowPrefab;
    [SerializeField] private Button buildButton;
    
    [SerializeField] private Color uncraftableRecipeIconColor = new Color(1,1,1,0.4f);
    [SerializeField] private Color32 uncraftableRecipeBackgroundColor = new Color32(255, 255, 255, 0);
    [SerializeField] private Color32 insufficientResourceTextColor = new Color32(200, 0, 0, 255);
    [SerializeField] private Color insufficientResourceIconColor = new Color(1, 1, 1, 0.6f);

    [Header("Info Page UI Elements")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Image energyBarImg;
    //UI
    private MenuController menuController;
    private GameObject[] uiBaseSlots;
    private GameObject[] recipeUISlots;
    private GameObject currentPage;
    private int selectedBaseSlotIndex;
    private int selectedRecipeIndex;
    
    //Data
    private BaseManager baseManager;
    private List<BaseSlot> baseSlots;
    private List<StructureRecipeSO> playerRecipeList;
    
    public void Initialize(BaseManager baseManagerInstance, List<StructureRecipeSO> playerRecipes)
    {
        //Data
        this.baseManager = baseManagerInstance;
        baseSlots = baseManager.BaseSlots;
        this.playerRecipeList = playerRecipes;
        baseManager.OnSlotsChanged += RefreshBaseSlots;
        baseManager.EnergyController.OnEnergyChanged += SetEnergyText;
        baseManager.EnergyController.OnEnergyChanged += RefreshRecipeSlots;
        
        //UI
        structuresPage.SetActive(false);
        SwitchActivePage(terminalPage);
        InstantiateSlots();
        SetEnergyText();
        
        menuController = GetComponent<MenuController>();
        if (menuController != null) menuController.OnTabChange += OnTabChange;
    }
    private void OnDisable()
    {
        //clear references
        ClearTerminalPage();
        ClearStructurePage();
        if (baseManager != null)
        {
            baseManager.OnSlotsChanged -= RefreshBaseSlots;
            baseManager.EnergyController.OnEnergyChanged -= SetEnergyText;
            baseManager.EnergyController.OnEnergyChanged -= RefreshRecipeSlots;
            baseManager = null;
        }
        baseSlots = null;

        selectedBaseSlotIndex = -1;
        selectedRecipeIndex = -1;
        
        if(menuController != null) menuController.OnTabChange -= OnTabChange;
    }
    
    #region  Terminal Page
    private void RefreshBaseSlots(List<BaseSlot> baseSlots)
    {
        this.baseSlots = baseSlots;
        DestroyUISlots();
        InstantiateSlots();
    }
    private void InstantiateSlots()
    {
        uiBaseSlots = new GameObject[baseSlots.Count];
        for(int i = 0; i < baseSlots.Count; i++)
        {
            GameObject currentUISlot = Instantiate(baseUISlotPrefab, slotsContainer);
            uiBaseSlots[i] = currentUISlot;
            
            UISlot slot = currentUISlot.GetComponent<UISlot>();
            if(slot == null) Debug.LogError("UISlot does not have a UISlot script component");

            if (baseSlots[i].IsEmpty)
            {
                slot.Clear();
            }
            else
            {
                slot.SetValues(baseSlots[i].StructureData.structureName, baseSlots[i].StructureData.structureSprite);
            }

            //add onclick listener
            int currentIndex = i;
            AddOnClickEvent(currentUISlot, currentIndex, EnableStructurePage);
        }
    }
    private void DestroyUISlots()
    {
        if (uiBaseSlots == null) return;
        foreach (GameObject uiSlot in uiBaseSlots )
        {
            Destroy(uiSlot);
        }
        uiBaseSlots = null;
    }
    private void ClearTerminalPage()
    {
        DestroyUISlots();
    }
    #endregion

    #region Structure Page
    public void EnableStructurePage(int slotIndex)
    {
        selectedBaseSlotIndex = slotIndex;
        SwitchActivePage(structuresPage);
        ClearStructurePage(); //rebuild to keep recipe craftable state updated
        InstantiateRecipeSlots();
        buildButton.interactable = false;
    }
    private void InstantiateRecipeSlots()
    {
        recipeUISlots = new GameObject[playerRecipeList.Count];
        for (int i = 0; i < playerRecipeList.Count; i++)
        {
            GameObject currentUISlot = Instantiate(recipeSlotPrefab, recipeContainer);
            recipeUISlots[i] = currentUISlot;
            
            //add the structure sprite from the scriptable object
            UISlot uiSlotScript = currentUISlot.GetComponent<UISlot>();
            if (uiSlotScript == null) Debug.LogError("RecipeSlot does not have a UISlot script component");
            
            uiSlotScript.SetValues("", playerRecipeList[i].icon);
            
            //add onclick event
            int currentIndex = i;
            AddOnClickEvent(currentUISlot, currentIndex, SelectRecipeSlot);
            
            bool canBeBuilt = CanRecipeBeBuilt(playerRecipeList[i]);
            //if not enough resources, gray out
            if (!canBeBuilt)
            {
                uiSlotScript.SetIconColor(uncraftableRecipeIconColor);
                uiSlotScript.SetSlotBackgroundColor(uncraftableRecipeBackgroundColor);
            }
        }
    }
    private void RefreshRecipeSlots()
    {
        if(recipeUISlots == null) return;
        for (int i = 0; i < recipeUISlots.Length; i++)
        {
            UISlot recipeUIScript = recipeUISlots[i].GetComponent<UISlot>();
            bool canBeBuilt = CanRecipeBeBuilt(playerRecipeList[i]);
            if (!canBeBuilt)
            {
                recipeUIScript.SetIconColor(uncraftableRecipeIconColor);
                recipeUIScript.SetSlotBackgroundColor(uncraftableRecipeBackgroundColor);
            }
            else
            {
                recipeUIScript.SetIconColor(Color.white);
                recipeUIScript.SetSlotBackgroundColor(Color.white);
            }
            if (selectedRecipeIndex == i && selectedBaseSlotIndex >= 0)
            {
                bool isSlotEmpty = baseSlots[selectedBaseSlotIndex].IsEmpty;
                buildButton.interactable = isSlotEmpty && canBeBuilt;
            }
        }
        RefreshDetailsPageData();
    }
    private void RefreshDetailsPageData()
    {
        if (selectedRecipeIndex < 0 || selectedRecipeIndex >= playerRecipeList.Count) return;
        if (selectedBaseSlotIndex < 0 || selectedBaseSlotIndex >= baseSlots.Count) return;
        Transform[] children = new Transform[costContainer.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = costContainer.transform.GetChild(i);
            UISlot uiSlotScript = children[i].GetComponent<UISlot>();
            if (uiSlotScript == null) continue;
            bool hasEnergyRow = playerRecipeList[selectedRecipeIndex].energyCost > 0;
            if (hasEnergyRow && i == children.Length - 1)
            {
                bool hasEnergy = baseManager.HasEnergy(playerRecipeList[selectedRecipeIndex].energyCost);
                if (hasEnergy)
                {
                    uiSlotScript.SetIconColor(Color.white);
                    uiSlotScript.SetTextColor(Color.black);
                }
                else
                {
                    uiSlotScript.SetIconColor(insufficientResourceIconColor);
                    uiSlotScript.SetTextColor(insufficientResourceTextColor);
                }
            }
            else
            {
                bool hasResource = baseManager.RequestCheckResource(playerRecipeList[selectedRecipeIndex].resources[i]);
                if (hasResource)
                {
                    uiSlotScript.SetIconColor(Color.white);
                    uiSlotScript.SetTextColor(Color.black);
                } 
                else
                {
                    uiSlotScript.SetIconColor(insufficientResourceIconColor);
                    uiSlotScript.SetTextColor(insufficientResourceTextColor);
                }
            }
        }
    }
    private void DestroyRecipeSlots()
    {
        if(recipeUISlots == null) return;
        foreach (GameObject recipeSlot in recipeUISlots)
        {
            Destroy(recipeSlot);
        }
        recipeUISlots = null;
    }
    public void SelectRecipeSlot(int index)
    {
        selectedRecipeIndex = index;
        StructureRecipeSO recipe = playerRecipeList[index];
        SetDetailsPageData(recipe);
        
        //check if enough resources, set button as interactable if true
        bool canBeBuilt = CanRecipeBeBuilt(recipe);
        bool slotEmpty = baseSlots[selectedBaseSlotIndex].IsEmpty;
        buildButton.interactable = canBeBuilt && slotEmpty;
    }
    private void SetDetailsPageData(StructureRecipeSO recipe)
    {
        ClearDetailsPage(); //clear to avoid stacking of rows
        
        detailsPanel.SetActive(true);
        //set name
        detailsNameText.text = recipe.structureResult.structureName;
        
        //instantiate cost rows and set the icon and amount 
        List<ResourceCost>  resources = recipe.resources;
        foreach (ResourceCost resource in resources)
        {
            GameObject costRow = Instantiate(costRowPrefab, costContainer.transform);
            UISlot costRowScript = costRow.GetComponent<UISlot>();
            
            costRowScript.SetValues(resource.amount.ToString(),  resource.itemDataSo.itemIcon);
            
            //make text red and image semi transparent if not enough resources 
            bool hasResource = baseManager.RequestCheckResource(resource);
            if (!hasResource)
            {
                costRowScript.SetTextColor(insufficientResourceTextColor);
                costRowScript.SetIconColor(insufficientResourceIconColor);
            }

        }
        if (recipe.energyCost > 0)
        {
            GameObject energyRow = Instantiate(costRowPrefab, costContainer.transform);
            UISlot energyRowScript = energyRow.GetComponent<UISlot>();
            energyRowScript.SetValues(recipe.energyCost.ToString(), energyIcon);
            if (!baseManager.HasEnergy(recipe.energyCost))
            {
                energyRowScript.SetTextColor(insufficientResourceTextColor);
                energyRowScript.SetIconColor(insufficientResourceIconColor);
            }
        }
    }
    private bool CanRecipeBeBuilt(StructureRecipeSO recipe)
    {
        if (selectedBaseSlotIndex < 0 || selectedBaseSlotIndex >= baseSlots.Count) return false;
        BaseSlot baseSlot = baseSlots[selectedBaseSlotIndex];
        if (!baseSlot.IsEmpty) return false;
        return baseManager.HasResources(recipe.resources, recipe.energyCost) && baseManager.CanBuildStructure(recipe.structureResult);
    }
    private void ClearDetailsPage()
    {
        //Clear existing ui elements
        foreach (Transform child in costContainer.transform)
        {
            Destroy(child.gameObject);
        }
        detailsNameText.text = "";
        detailsPanel.SetActive(false);
    }
    private void ClearStructurePage()
    {
        ClearDetailsPage();
        DestroyRecipeSlots();
    }
    public void GoBackToTerminal() //Go back button on click
    {
        selectedBaseSlotIndex = -1;
        selectedRecipeIndex = -1;
        SwitchActivePage(terminalPage);
    }
    public void OnBuildButtonClick()
    {
        //return if invalid index
        if(selectedBaseSlotIndex < 0 || selectedBaseSlotIndex >= baseSlots.Count || 
           selectedRecipeIndex < 0 || selectedRecipeIndex >= playerRecipeList.Count) return;
        
        int baseSlotIndex = selectedBaseSlotIndex;
        StructureRecipeSO recipe = playerRecipeList[selectedRecipeIndex];
        bool built = baseManager.TryBuild(baseSlotIndex, recipe);
        if(built) GoBackToTerminal();
        buildButton.interactable = false;
    }
    private void OnTabChange()
    {
        ClearDetailsPage();
        selectedRecipeIndex = -1;
        buildButton.interactable = false;
    }
    #endregion
    #region Info Page
    private void SetEnergyText()
    {
        if (baseManager == null) return;
        float currentEnergy = baseManager.EnergyController.CurrentEnergy;
        float maxEnergy = baseManager.EnergyController.MaxEnergy;

        float percentEnergy = maxEnergy == 0 ? 0 : currentEnergy / maxEnergy;
        
        energyText.text = $"{currentEnergy}/{maxEnergy}";
        energyBarImg.fillAmount = percentEnergy;
    }
    #endregion
    private void SwitchActivePage(GameObject page)
    {
        if(currentPage == page) return;
        if (currentPage != null) currentPage.SetActive(false);
        
        page.SetActive(true);
        currentPage = page;
    }
    private void AddOnClickEvent(GameObject uiSlot, int index, Action<int> callback)
    {
        Button button = uiSlot.GetComponent<Button>();
        if(button == null)  button = uiSlot.AddComponent<Button>();
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(index));
    }
}