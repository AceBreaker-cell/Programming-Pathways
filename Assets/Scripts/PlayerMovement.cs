using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.3f;  // jarak raycast ke dinding

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    private float moveInput;
    private bool isGrounded;
    private bool isTouchingWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Wall check — raycast kiri & kanan
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, groundLayer);
        RaycastHit2D hitLeft  = Physics2D.Raycast(transform.position, Vector2.left,  wallCheckDistance, groundLayer);

        // Cek apakah nabrak dinding sesuai arah gerak
        isTouchingWall = (moveInput > 0 && hitRight.collider != null) ||
                         (moveInput < 0 && hitLeft.collider != null);

        // Jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJump();
        }

        // Animation
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);

        // Flip character
        if (moveInput > 0) sprite.flipX = false;
        else if (moveInput < 0) sprite.flipX = true;
    }

    void FixedUpdate()
    {
        if (isTouchingWall)
        {
            // Nabrak dinding — stop gerak horizontal, tetap bisa jatuh
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            // Normal movement
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Visualize wall check rays
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, Vector2.left  * wallCheckDistance);
    }
}