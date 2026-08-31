using System;
using UnityEngine;

public class ActiveItemVisualController : MonoBehaviour
{
    [SerializeField] private Transform activeItemPivot;
    [SerializeField] private PlayerMovement playerMovement;
    
    private float scaleSize = 0.75f;
    private readonly float offset = 1f;

    private void OnEnable()
    {
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement is null");
            return;
        }
        playerMovement.OnFacingDirectionChanged += UpdateHeldItemDirection;
    }

    private void OnDisable()
    {
        if(playerMovement != null)
            playerMovement.OnFacingDirectionChanged -= UpdateHeldItemDirection;
    }
    
    private void UpdateHeldItemDirection(Vector2 direction)
    {

        bool facingDown = direction.x ==  0 && direction.y < 0;
        bool facingUp = direction.x == 0 && direction.y > 0;
        bool facingLeft = direction.x < 0;
        bool facingRight = direction.x > 0;
        
        Vector3 scale = Vector3.one * scaleSize;
        if (facingDown)
        {
            scale.y = -1 * scaleSize;
            activeItemPivot.localPosition = new Vector3(0, -offset * 0.25f, 0);
        }
        if (facingUp)
        {
            scale.x = -1 * scaleSize;
            activeItemPivot.localPosition = new Vector3(0, offset * 1.5f, 0);
        }
        if (facingLeft)
        {
            scale.x = -1 * scaleSize;
            activeItemPivot.localPosition = new Vector3(-offset, 0, 0);
        }
        if (facingRight)
        {
            scale.x = scaleSize;
            activeItemPivot.localPosition = new Vector3(offset, 0, 0);
        }
        activeItemPivot.localScale = scale;
    }
}