using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    private DayNightCycle dayNightCycle;
    [SerializeField]private Light2D light2D;

    private void Start()
    {
        dayNightCycle = World.Instance?.DayNightCycle;
        if(dayNightCycle != null)
            dayNightCycle.OnTimeOfDayChanged += HandleTimeOfDayChanged;
        HandleTimeOfDayChanged();
    }
    private void OnDisable()
    {
        if(dayNightCycle != null)
            dayNightCycle.OnTimeOfDayChanged -= HandleTimeOfDayChanged;
    }
    private void HandleTimeOfDayChanged()
    {
        if (dayNightCycle && dayNightCycle.IsNight)
            light2D.intensity = 0.6f;
        else
            light2D.intensity = 0f;
    }
}
