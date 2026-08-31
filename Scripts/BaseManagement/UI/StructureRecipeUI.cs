using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]
public class StructureRecipeUI : MonoBehaviour
{
    [SerializeField] private Image slotImg;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color uncraftableRecipeIconColor = new Color(1, 1, 1, 0.4f);
    

    public StructureRecipeData RecipeData { get; private set; }

    public void SetData(StructureRecipeData data)
    {
        this.RecipeData = data;
        
        SetValues(data.structureName, data.icon);
        if (!TryGetComponent(out Button button))
        {
            Debug.LogWarning("No button on UISlot prefab");
            return;
        }
        SetIconColor(data.canBuild ? Color.white : uncraftableRecipeIconColor);
    }
    public void SetValues(string txt, Sprite sprite)
    {
        if(text != null)
            text.text = txt;
        
        if (iconImg != null)
        {
            iconImg.sprite = sprite;
            iconImg.color = Color.white;
        }
    }
    public void Clear()
    {
        if (iconImg != null)
        {
            iconImg.sprite = null;
            iconImg.color = new Color32(0, 0, 0, 0);
        }
        if(text != null)
            text.text = "";
    }
    public void SetTextColor(Color color)
    {
        if(text != null)
            text.color = color;
    }
    public void SetIconColor(Color color)
    {
        if(iconImg != null)
            iconImg.color = color;
    }
    public void SetSlotBackgroundColor(Color color)
    {
        if(slotImg != null)
            slotImg.color = color;
    }
    public void SetSelected(bool selected)
    {
        if(slotImg == null) return;
        slotImg.color = selected ? slotImg.color * 1.5f : Color.white;
    }
}

