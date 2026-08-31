using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InfoUIPage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Image energyBarImg;

    private BaseManagerViewModel bmViewModel;
    
    public void Initialize(BaseManagerViewModel viewModel)
    {
        if(bmViewModel != null) bmViewModel.OnDataChanged -= SetEnergyText;
        
        if(viewModel == null)
        {
            Debug.LogError("Null BaseManagerViewModel passed to InfoUIPage");
            return;
        }
        
        this.bmViewModel = viewModel;
        if(gameObject.activeInHierarchy) bmViewModel.OnDataChanged += SetEnergyText;
        
        SetEnergyText();
    }

    private void OnEnable()
    {
        if (bmViewModel != null)
        {
            bmViewModel.OnDataChanged -= SetEnergyText;
            bmViewModel.OnDataChanged += SetEnergyText;
            SetEnergyText();
        }
    }

    private void OnDisable()
    {
        if (bmViewModel != null)
            bmViewModel.OnDataChanged -= SetEnergyText;
    }
    
    // Get Data & set UI values
    public void SetEnergyText()
    {
        if (energyBarImg == null || energyText == null)
        {
            Debug.LogWarning("Energy UI components not assigned in InfoUIPage.");
            return;
        }
        if (bmViewModel == null) return;
        
        EnergyViewData energyData = bmViewModel.GetEnergyData();

        energyText.text = $"{energyData.currentEnergy}/{energyData.maxEnergy}";
        energyBarImg.fillAmount = energyData.energyPercentage;
    }
}
