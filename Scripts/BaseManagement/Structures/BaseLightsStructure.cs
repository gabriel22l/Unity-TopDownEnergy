using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseLightsStructure : EnergyConsumer
{
    private List<GameObject> lightPosts;
    private List<Light2D> lightObjects;
    
    [SerializeField] private float lightIntensity = 0.8f;

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
        lightPosts = baseManager.lightPosts;
        lightObjects = baseManager.lightObjects;
        EnableLightPosts();
        DisableLights();

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
    private void EnableLightPosts()
    {
        foreach (GameObject l in lightPosts)
        {
            l.SetActive(true);
        }
    }
    private void DisableLights()
    {
        foreach (Light2D l in lightObjects )
        {
            l.intensity = 0;
        }
    }
    public override void Power()
    {
        if (isActive) return;
        isActive = true;
        foreach (Light2D l in lightObjects )
        {
            l.intensity = lightIntensity;
        }
    }
    public override void UnPower()
    {
        if(!isActive) return;
        isActive = false;
        DisableLights();
    }

    private void HandleTimeOfDayChanged()
    {
        if (!IsNight && IsConsuming)
        {
            UnPower();
            IsConsuming = false;
        } else if (IsNight && !IsConsuming)
        {
            IsConsuming = true;
        }
    }
}
