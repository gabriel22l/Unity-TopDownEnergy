using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    [SerializeField] private float movementSpeed = 3f;
    
    public Vector2 FacingDirection { get; private set; }
    public event Action<Vector2> OnFacingDirectionChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        
        FacingDirection = Vector2.down;
    }
    private void FixedUpdate()
    {
        Vector2 processedMoveInput =
            playerInput.MoveInput.magnitude > 1 ? 
                playerInput.MoveInput.normalized : 
                playerInput.MoveInput;
        
        rb.linearVelocity = processedMoveInput * movementSpeed;

        if (processedMoveInput.magnitude > 0.1f)
        {
            Vector2 lastFacingDirection = FacingDirection;
            FacingDirection = processedMoveInput.normalized;
            if(lastFacingDirection != FacingDirection)
                OnFacingDirectionChanged?.Invoke(FacingDirection);
        }
    }
}