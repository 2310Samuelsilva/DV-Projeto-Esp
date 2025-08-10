using UnityEngine;

/// <summary>
/// Handles player movement, jumping, and respawn.
/// Transport stats are injected via TransportData.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Transport Data")]
    private TransportData transportData;

    // Movement stats (loaded from transportData)
    private float speed;
    private float jumpForce;
    private float handling;
    private float lowJumpMultiplier;
    private float fallMultiplier;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Respawn Settings")]
    [SerializeField] private float fallThresholdY = -10f;
    private Vector2 spawnPoint;

    // Components
    private Rigidbody2D rb;

    // State
    private bool isGrounded;
    private bool doJump;

    // -------------------- Public Methods --------------------

    /// <summary>
    /// Initializes player movement stats from the given TransportData.
    /// </summary>
    public void Initialize(TransportData data)
    {
        transportData = data;

        speed = data.speedMultiplier;
        jumpForce = data.jumpForce;
        handling = data.handling;
        lowJumpMultiplier = data.lowJumpMultiplier;
        fallMultiplier = data.fallMultiplier;

        Debug.Log($"Initialized player: {data.transportName} | Speed: {speed}");
    }

    // -------------------- Unity Methods --------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPoint = transform.position; // Initial spawn position
    }

    private void Update()
    {
        HandleInput();
        CheckFallRespawn();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleJump();
        ApplyBetterJumpPhysics();
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // -------------------- Movement Logic --------------------

    private void HandleInput()
    {
        if (Input.GetButtonDown("Jump"))
            doJump = true;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleJump()
    {
        if (doJump && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        doJump = false;
    }

    private void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            // Falling
            rb.linearVelocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Short hop
            rb.linearVelocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
    }

    // -------------------- Respawn Logic --------------------

    private void CheckFallRespawn()
    {
        if (transform.position.y < fallThresholdY)
            Respawn();
    }

    private void Respawn()
    {
        transform.position = spawnPoint;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Player respawned.");
    }
}