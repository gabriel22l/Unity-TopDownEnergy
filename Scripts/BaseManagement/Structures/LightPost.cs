using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPost : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float lightIntensity = 0.8f;
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;

    private void Awake()
    {
        if(spriteRenderer == null && TryGetComponent(out spriteRenderer))
            Debug.LogWarning("LightPost script requires a SpriteRenderer");
        if(light2D == null)
            Debug.LogWarning("LightPost script requires a Light2D");
    }
    public void EnableLight(bool enable)
    {
        if (spriteRenderer == null || light2D == null) return;
        light2D.intensity = enable ? lightIntensity : 0;
        spriteRenderer.sprite = enable ? enabledSprite : disabledSprite;
    }
}
