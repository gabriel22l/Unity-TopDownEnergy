using System;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private DamageDealerType allowedDamageDealerType;

    private void Awake()
    {
        if (health == null && !TryGetComponent(out health))
        {
            Debug.LogWarning("No Health component");
        }
    }
    public bool TakeDamage(DamageDealerContext ctx)
    {
        if(health == null) return false;
        if(allowedDamageDealerType != DamageDealerType.None && ctx.DamageType != allowedDamageDealerType)
            return false;
        
        return health.TakeDamage(ctx.DamageAmount);
    }
}
