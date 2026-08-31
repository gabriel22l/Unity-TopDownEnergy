using System;
using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    private DayNightCycle dayNightCycle;
    private float timer;
    private float updateFrequency = 0.1f;
    private void Start()
    {
        dayNightCycle = World.Instance?.DayNightCycle;
        if (clockText == null && !TryGetComponent(out clockText))
        {
            Debug.LogWarning("Clock Text not found");
        }
        if (dayNightCycle == null)
        {
            Debug.LogWarning("DayNightCycle not found");
        }
        timer = updateFrequency;
    }
    private void Update()
    {
        if (!clockText || !dayNightCycle) return;
        if (timer >= updateFrequency)
        {
            timer -= updateFrequency;
            clockText.text = dayNightCycle?.GetFormattedTime();
        }
        timer += Time.deltaTime;
    }
}