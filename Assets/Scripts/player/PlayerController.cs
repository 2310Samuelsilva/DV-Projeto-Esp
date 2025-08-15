using System;
using UnityEngine;

/// <summary>
/// Handles player movement, jumping, and respawn.
/// Transport stats are injected via TransportData.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Transport Data")]
    [SerializeField] private PlayerTransportData playerTransportData;


    // Movement stats (loaded from transportData)
    // private float speed;
    // private float jumpForce;
    // private float handling;
    // private float lowJumpMultiplier;
    // private float fallMultiplier;
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Rotation Kill Check")]
    [SerializeField] private Transform rotationKillCheck;
    [SerializeField] private float rotationKillCheckRadius = 0.2f;

    [Header("Respawn Settings")]
    [SerializeField] private float fallThresholdY = -10f;
    private Vector2 spawnPoint;

    // Components
    private Rigidbody2D rb;

    // State
    private bool isGrounded;
    private bool doJump;
    private float horizontalInput;

    private float rotationVelocity;


    // -------------------- Public Methods --------------------

    /// <summary>
    /// Initializes player movement stats from the given TransportData.
    /// </summary>
    public void Initialize(PlayerTransportData playerTransportData)
    {
        this.playerTransportData = playerTransportData;
        Debug.Log($"Initialized player: {playerTransportData.transportName}" + $" Level: {playerTransportData.level}");
        Debug.Log($"moveSpeed: {playerTransportData.GetMoveSpeed()}");
        rotationVelocity = 0f;
        
    }

    public float MoveSpeed() => playerTransportData.GetMoveSpeed();

    // -------------------- Unity Methods --------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        HandleInput();
        CheckRotationKill();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyMovementPhysics();
    }

    // -------------------- Gizmos --------------------
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rotationKillCheck.position, rotationKillCheckRadius);
        }
    }

    // -------------------- Movement Logic --------------------

    private void HandleInput()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
            doJump = true;
    }

    private void HandleHorizontal()
    {
        if (isGrounded)
        {
            rotationVelocity = 0f;
            return;
        }

         // Rotate with input
        float targetRotationVelocity = -horizontalInput * playerTransportData.GetRotationAcceleration();

        // Accelerate toward target rotation velocity
        rotationVelocity = Mathf.MoveTowards(rotationVelocity, targetRotationVelocity, playerTransportData.GetRotationAcceleration() * Time.deltaTime);

        // Apply damping if no input
        if (Mathf.Approximately(horizontalInput, 0f))
        {
            rotationVelocity = Mathf.Lerp(rotationVelocity, 0f, playerTransportData.GetRotationDamp() * Time.deltaTime);
        }

        // Clamp rotation speed
        rotationVelocity = Mathf.Clamp(rotationVelocity, -playerTransportData.GetMaxRotationVelocity(), playerTransportData.GetMaxRotationVelocity());

        // Apply rotation
        transform.Rotate(0f, 0f, rotationVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (doJump && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, playerTransportData.GetJumpForce());

        doJump = false;
    }

    private void ApplyMovementPhysics()
    {
        HandleHorizontal();
        HandleJump();

        // Jump / Fall Physics
        if (rb.linearVelocity.y < 0)
        {
            // Falling
            rb.linearVelocity += (playerTransportData.GetFallMultiplier() - 1) * Time.fixedDeltaTime * Vector2.up;
        }
        // Jumping but not pressing jump -> Jump low
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Short hop
            rb.linearVelocity += (playerTransportData.GetLowJumpMultiplier() - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
    }

    // -------------------- Checks --------------------
    private void CheckRotationKill()
    {
        // Check top half of circle
        if (Physics2D.OverlapCircle(rotationKillCheck.position, rotationKillCheckRadius, groundLayer))
        {
            LevelManager.Instance.EndLevel();
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

}