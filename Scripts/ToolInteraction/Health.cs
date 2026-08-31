using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public float CurrentHealth =>  currentHealth;
    public float MaxHealth => maxHealth;
    
    public float HealthPercentage => Mathf.Clamp01(currentHealth / maxHealth);

    [SerializeField] private float invulnerabilityTime = 0f;
    private bool isInvulnerable;
    
    public event Action OnHealthChange;
    public event Action OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        OnHealthChange?.Invoke();
    }

    public bool TakeDamage(float damage)
    {
        if(currentHealth <= 0 ||  damage <= 0 || isInvulnerable) return false;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChange?.Invoke();
        OnDamageTaken?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        StartCoroutine(InvulnerabilityCoroutine());
        return true;
    }
    private void Die()
    {
        OnDeath?.Invoke();
    }
    private System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable =  true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }
    [ContextMenu("Trigger Death")]
    public void TriggerDeath()
    {
        TakeDamage(maxHealth);
    }
}