using System;
using UnityEngine;

public class SolarPanel : EnergyProducer
{
    private DayNightCycle dayNightCycle;
    private bool IsDay => dayNightCycle != null && !dayNightCycle.IsNight;

    private SpriteRenderer spriteRenderer;
    private Color activeColor =  Color.white;
    private Color inactiveColor = Color.gray;
    private bool isActive;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        World worldInstance = World.Instance;
        if (worldInstance == null)
        {
            Debug.LogError("No World instance found. SolarPanel requires a DayNightCycle to function.");
            return;
        }
        
        dayNightCycle = worldInstance.DayNightCycle;
        if (dayNightCycle == null)
        {
            Debug.LogError("No DayNightCycle found in World instance. SolarPanel requires a DayNightCycle to function.");
            return;
        }
        
        dayNightCycle.OnTimeOfDayChanged += HandleTimeOfDayChanged;
        HandleTimeOfDayChanged();
    }
    private void OnDisable()
    {
        if(dayNightCycle != null)
            dayNightCycle.OnTimeOfDayChanged -= HandleTimeOfDayChanged;
    }
    public override float ProduceEnergy()
    {
        if (!IsDay)
            return 0;
        return energyPerTick;
    }
    private void SetActiveVisual(bool active)
    {
        isActive = active;
        if (spriteRenderer == null) return;
        spriteRenderer.color = 
            active ? activeColor : inactiveColor;
    }
    private void HandleTimeOfDayChanged()
    {
        SetActiveVisual(IsDay);
    }
}