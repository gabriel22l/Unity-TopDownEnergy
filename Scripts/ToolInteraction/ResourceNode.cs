using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private ItemData resourceData;
    
    [SerializeField] private int minDropAmount = 1;
    [SerializeField] private int maxDropAmount = 3;
    
    [SerializeField] private DropHandler dropHandler;

    private void OnEnable()
    {
        if (health == null && !TryGetComponent(out health))
        {
            Debug.LogWarning("No health component found");
            return;
        }
        if (!dropHandler)
        {
            Debug.LogWarning("No Drop Handler found");
            return;
        }
        health.OnDeath += HandleDeath;
    }
    private void OnDisable()
    {
        if(health) health.OnDeath -= HandleDeath;
    }
    private void HandleDeath()
    {
        InstantiateResources();
        Destroy(gameObject);
    }
    private void InstantiateResources()
    {
        int amount = Random.Range(minDropAmount, maxDropAmount + 1);
        for (int i = 0; i < amount; i++)
        {
            dropHandler.TryPlaceInNearestRandomPos(resourceData, 1);
        }
    }
}