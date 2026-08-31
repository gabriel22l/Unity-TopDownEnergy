using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color normalColor =  Color.white;
    private Color highlightColor = new Color(1.5f,1.5f,1.5f);
    private void Awake()
    {
        if (spriteRenderer == null && !TryGetComponent(out spriteRenderer))
            Debug.LogWarning($"SpriteRenderer not assigned! at {gameObject.name}");
    }
    public void SetHighlighted(bool isHighlighted)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = isHighlighted ? highlightColor : normalColor;
    }
}
