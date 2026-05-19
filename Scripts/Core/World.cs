using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
    [field:SerializeField]public DayNightCycle DayNightCycle { get; private set; }

    //Do not access world or its subsystems on Awake,
    //use Start instead. This is to ensure that all singletons are properly initialized before being accessed.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if(DayNightCycle == null) 
            DayNightCycle = GetComponentInChildren<DayNightCycle>();
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
