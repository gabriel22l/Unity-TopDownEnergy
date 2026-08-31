using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private Animator animator;

    private int lastInputXp;
    private int lastInputYp;
    private int inputX;
    private int inputY;
    private int isWalkingP;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();

        lastInputXp = Animator.StringToHash("LastInputX");
        lastInputYp = Animator.StringToHash("LastInputY");
        inputX = Animator.StringToHash("InputX");
        inputY = Animator.StringToHash("InputY");
        isWalkingP = Animator.StringToHash("IsWalking");
    }
    private void Update()
    {
        Vector2 facingDir = playerMovement.FacingDirection;
        
        //isWalking
        animator.SetBool(isWalkingP, playerInput.MoveInput.magnitude > 0.1f);
        
        //inputX/Y and lastInputX/Y now driven by facing direction instead of raw input
        animator.SetFloat(inputX, facingDir.x);
        animator.SetFloat(inputY, facingDir.y);
        animator.SetFloat(lastInputXp, facingDir.x);
        animator.SetFloat(lastInputYp, facingDir.y);
    }
}