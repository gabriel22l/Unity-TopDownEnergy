using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerContext playerContext;
    [SerializeField] private PlayerInput playerInput; //for subscribing to Interact event
    private InteractionContext interactionContext;

    private IInteractable currentInteractable;
    private InteractableHighlight currentHighlight;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private void Start()
    {
        if (playerContext == null)
            Debug.LogError("PlayerContext not assigned in PlayerInteraction");
        interactionContext = new InteractionContext //set interaction context
        {
            playerContext = playerContext,
        };

        if (playerInput != null)
            playerInput.InteractEvent += OnInteract;
    }
    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.InteractEvent -= OnInteract;
    }
    private void Update()
    {
        UpdateCurrentInteractable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out IInteractable interactable)) return;
        if (!nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out IInteractable interactable)) return;
        nearbyInteractables.Remove(interactable);
    }
    private void OnInteract()
    {
        currentInteractable?.Interact(interactionContext);
    }
    private IInteractable GetClosestInteractable()
    {
        if (nearbyInteractables.Count == 0)
            return null;

        float closestDistance = float.MaxValue;
        IInteractable closestInteractable = null;
        foreach (var interactable in nearbyInteractables)
        {
            if (interactable is MonoBehaviour mb)
            {
                float distance = Vector3.Distance(mb.transform.position, playerContext.transform.position);
                if (distance >= closestDistance)
                    continue;
                closestDistance = distance;
                closestInteractable = interactable;

            }
        }
        return closestInteractable;
    }
    private void UpdateCurrentInteractable()
    {
        IInteractable closestInteractable = GetClosestInteractable();
        if (closestInteractable == currentInteractable) return;
        
        currentHighlight?.SetHighlighted(false);
        currentHighlight = null;
        
        currentInteractable = closestInteractable;
        if(currentInteractable is MonoBehaviour mb)
        {
            mb.TryGetComponent(out currentHighlight);
            currentHighlight?.SetHighlighted(true);
        }
    }
}

public struct InteractionContext
{
    public PlayerContext playerContext;
}