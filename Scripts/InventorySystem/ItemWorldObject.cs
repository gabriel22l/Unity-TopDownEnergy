using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour, IInteractable
{
    [SerializeField] private itemDataSO itemDataSo;
    public int amount = 1;
    
    [SerializeField] private LayerMask obstacleLayerMask;
    
    private Rigidbody2D rigidBody;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    public void Interact(InteractionContext interactionContext)
    {
        InventoryController inventoryController = interactionContext.playerContext.InventoryController;
        if (inventoryController == null) return;
        int remainingAmount = inventoryController.AddItem(itemDataSo, amount);
        if (remainingAmount == 0)
        {
            GameObject player = interactionContext.playerContext.gameObject;
            if (player == null)
            {
                Destroy(gameObject);
            } else StartCoroutine(PickUpAnimDestroy(player.transform));
        }
        else amount = remainingAmount;
    }
    private IEnumerator PickUpAnimDestroy(Transform playerTransform)
    {
        if (rigidBody != null) rigidBody.simulated = false;
        float duration = 0.2f;
        float timer = 0;
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.position = Vector3.Lerp(startPosition, playerTransform.position, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}