using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;
    public float constantSpeed = 2.0f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Physics")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Respawn Settings")]
    public Vector2 spawnPoint;
    public float fallThresholdY = -10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool doJump;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPoint = transform.position; // set initial spawn
    }

    void Update()
    {
        // Respawn if player falls too low
        if (transform.position.y < fallThresholdY)
        {
            Debug.Log("Respawn");
            Respawn();
        }

        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            doJump = true;
        }
    }

    void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Move player
        //float moveX =  constantSpeed + (moveSpeed * horizontalInput);
        float moveX =  moveSpeed * horizontalInput;

        rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);

        // Jump
        if (doJump && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Apply better jump physics
        if (rb.linearVelocity.y < 0)
        {
            // Player is falling, apply fall multiplier
            rb.linearVelocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Player is jumping but not holding jump button, apply low jump multiplier
            rb.linearVelocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }

        doJump = false;
    }

    private void Respawn()
    {
        transform.position = spawnPoint;
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}