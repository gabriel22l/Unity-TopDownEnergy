using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    [SerializeField] private float movementSpeed = 3f;
    
    public Vector2 FacingDirection { get; private set; }

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
            FacingDirection = processedMoveInput.normalized;
    }
}