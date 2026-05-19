using System;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color normalColor =  Color.white;
    private Color highlightColor = new Color(1.5f,1.5f,1.5f);
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetHighlighted(bool isHighlighted)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = isHighlighted ? highlightColor : normalColor;
    }
}
