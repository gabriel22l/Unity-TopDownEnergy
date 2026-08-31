using UnityEngine;
using System.Collections.Generic;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private DamageDealerType damageDealerType;
    private DamageDealerContext damageDealerContext;
    
    private Collider2D hitCollider;
    private List<Collider2D> hits = new List<Collider2D>();
    
    private void Awake()
    {
        damageDealerContext = new DamageDealerContext(damageDealerType, damage);
        
        hitCollider = GetComponent<Collider2D>();
        if (hitCollider == null)
        {
            Debug.LogWarning("DamageHandler: No Collider2D attached to this GameObject.");
            return;
        }
        if(hitCollider.enabled)
        {
            hitCollider.enabled = false;
        }
    }
    public void EnableHitCollider()
    {
        hits.Clear();
        if (!hitCollider) return;
        hitCollider.enabled = true;
    }
    public void DisableHitCollider()
    {
        if (!hitCollider) return;
        hitCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") || hits.Contains(other) || !other.TryGetComponent(out DamageTaker damageTaker))
            return;
        
        hits.Add(other);
        damageTaker.TakeDamage(damageDealerContext);
    }
}

public class DamageDealerContext
{
    public DamageDealerType DamageType {get; private set;}
    public float DamageAmount {get; private set;}
    public DamageDealerContext(DamageDealerType damageDealerType, float damage)
    {
        this.DamageType = damageDealerType;
        this.DamageAmount = damage;
    }
}