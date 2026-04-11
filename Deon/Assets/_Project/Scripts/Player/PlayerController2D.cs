using UnityEngine;

/// <summary>
/// 2D Player Controller for DEON's simulated worlds.
/// Handles left/right movement, jumping, and optional ladder climbing.
/// Attach to the Player root GameObject inside the 2DPlayer Prefab.
/// </summary>
/// 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Animator animator; 

    [Header("Ladder")]
    [SerializeField] private float climbSpeed = 0f;

    // --- Private State ---
    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _isOnLadder;
    private bool _dialogueActive;

    // Called by NPCInteractionTrigger to lock/unlock movement during VN dialogue
    public bool IsDialogueActive
    {
        get => _dialogueActive;
        set
        {
            _dialogueActive = value;
            // Freeze horizontal + jumping when dialogue is open
            if (_dialogueActive)
                _rb.linearVelocity = Vector2.zero;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Block all input while VN dialogue is running
        if (_dialogueActive) return;

        HandleMovement();
        HandleJump();
        HandleLadder();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrow
        _rb.linearVelocity = new Vector2(h * moveSpeed, _rb.linearVelocity.y);

        // Flip sprite to face direction of movement
        
        // if (h != 0)
        //     transform.localScale = new Vector3(Mathf.Sign(h), 1f, 1f);

        if(h!=0)
        {
            if (h > 0)
            {
                animator.SetBool("IsWalkingRight", true);
                animator.SetBool("IsWalkingLeft", false);
            }
            else
            {
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingLeft", true);
            } 
        }
        else
        {
            animator.SetBool("IsWalkingRight", false);
            animator.SetBool("IsWalkingLeft", false);
        }

    }

    private void HandleJump()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded && Input.GetButtonDown("Jump")) // Space Bar
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleLadder()
    {
        if (!_isOnLadder) return;

        float v = Input.GetAxisRaw("Vertical");
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, v * climbSpeed);
        _rb.gravityScale = (v != 0) ? 0f : 1f; // Kill gravity while climbing
    }

    // Called by trigger colliders tagged "Ladder"
    public void SetOnLadder(bool onLadder)
    {
        _isOnLadder = onLadder;
        if (!onLadder)
            _rb.gravityScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}