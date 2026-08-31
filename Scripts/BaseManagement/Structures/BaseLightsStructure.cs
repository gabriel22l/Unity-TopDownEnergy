using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseLightsStructure : EnergyConsumer
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;
    
    private List<GameObject> lightObjects;
    private List<LightPost> lights;
    
    private DayNightCycle dayNightCycle;
    private bool IsNight => dayNightCycle != null && dayNightCycle.IsNight;
    
    private void OnDisable()
    {
        if(dayNightCycle != null)
            dayNightCycle.OnTimeOfDayChanged -= HandleTimeOfDayChanged;
    }
    public override void Initialize(BaseManager baseManager)
    {
        baseManager.EnergyController.RegisterEnergyConsumer(this);
        lightObjects = baseManager.lightPosts;
        lights = baseManager.lightObjects;
        EnableLightObjects();
        EnableLights(false);

        dayNightCycle = World.Instance?.DayNightCycle;
        if(dayNightCycle != null)
        {
            dayNightCycle.OnTimeOfDayChanged += HandleTimeOfDayChanged;
        }
        else
        {
            Debug.LogError("No DayNightCycle found in World instance. BaseLightsStructure requires a DayNightCycle to function.");
            return;
        }
        HandleTimeOfDayChanged();
    }
    private void EnableLightObjects()
    {
        foreach (GameObject l in lightObjects)
        {
            l.SetActive(true);
        }
    }
    private void EnableLights(bool enable)
    {
        spriteRenderer.sprite = enable ? enabledSprite : disabledSprite;
        foreach (LightPost l in lights )
        {
            l.EnableLight(enable);
        }
    }
    public override void Power()
    {
        if (isPowered) return;
        isPowered = true;
        EnableLights(true);
    }
    public override void UnPower()
    {
        if(!isPowered) return;
        isPowered = false;
        EnableLights(false);
    }

    private void HandleTimeOfDayChanged()
    {
        if (!IsNight && IsAvailable)
        {
            UnPower();
            IsAvailable = false;
        } else if (IsNight && !IsAvailable)
        {
            IsAvailable = true;
        }
    }
}
