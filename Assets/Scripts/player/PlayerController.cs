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

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.2f); // width, height
    [SerializeField] private LayerMask groundLayer;

    [Header("Rotation Kill Check")]
    [SerializeField] private Transform rotationKillCheck;
    [SerializeField] private float rotationKillCheckRadius = 0.2f;
    [SerializeField] private AudioSource movementAudio;
    [SerializeField] private float fadeDuration = 2f; // units per second
    [SerializeField] private ParticleSystem movementParticles;

    private GameOptions gameOptions;
    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // State
    private bool isGrounded;
    private bool doJump;
    private float horizontalInput;
    private float rotationVelocity;

    // -------------------- Public Methods --------------------
    public void Initialize(PlayerTransportData playerTransportData)
    {
        this.playerTransportData = playerTransportData;
        rotationVelocity = 0f;

        Debug.Log($"Initialized player: {playerTransportData.GetName()} Level: {playerTransportData.GetLevel()}");
    }

    public float MoveSpeed() => playerTransportData.GetMoveSpeed();

    public void PlayerLost() => LevelManager.Instance.EndLevel();

    // -------------------- Unity Methods --------------------
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //movementAudio = GetComponent<AudioSource>();

        gameOptions = GameManager.Instance.GetGameOptions();
        movementAudio.volume = gameOptions.SFXVolume;

        if (gameOptions == null)
        {
            Debug.LogError("GameOptions not found!");
            return;
        }        
    }

    private void Update()
    {
        if(LevelManager.Instance.IsPaused()){ return; }
        CheckGrounded();
        HandleInput();
        CheckRotationKill();
        UpdateAnimator();
        UpdateAudio();
        UpdateParticles();
    }

    private void FixedUpdate()
    {
        if(LevelManager.Instance.IsPaused()){ return; }
        ApplyMovementPhysics();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            LevelManager.Instance.ObstacleHit();
            EffectsManager.Instance.ObstacleHit(other.transform.position);

            Destroy(other.gameObject, 0.1f);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player hit obstacle.");
            LevelManager.Instance.ObstacleHit();
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

        float targetRotationVelocity = -horizontalInput * playerTransportData.GetRotationAcceleration();
        rotationVelocity = Mathf.MoveTowards(rotationVelocity, targetRotationVelocity, playerTransportData.GetRotationAcceleration() * Time.deltaTime);

        if (Mathf.Approximately(horizontalInput, 0f))
            rotationVelocity = Mathf.Lerp(rotationVelocity, 0f, playerTransportData.GetRotationDamp() * Time.deltaTime);

        rotationVelocity = Mathf.Clamp(rotationVelocity, -playerTransportData.GetMaxRotationVelocity(), playerTransportData.GetMaxRotationVelocity());
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

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (playerTransportData.GetFallMultiplier() - 1) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (playerTransportData.GetLowJumpMultiplier() - 1) * Time.fixedDeltaTime * Vector2.up;
    }

    // -------------------- Audio --------------------
    private void UpdateAudio()
    {
        bool shouldPlay = isGrounded;

        float targetVolume = shouldPlay ? gameOptions.SFXVolume : 0f;
        movementAudio.volume = Mathf.MoveTowards(movementAudio.volume, targetVolume, fadeDuration * Time.deltaTime);
        // Play if volume > 0 and not already playing
        if (movementAudio.volume > 0f && !movementAudio.isPlaying)
            movementAudio.Play();
        
        // Stop if volume reaches 0
        if (movementAudio.volume <= 0f && movementAudio.isPlaying)
            movementAudio.Stop();
        }

    // -------------------- Particles --------------------
    private void UpdateParticles()
    {
        if (movementParticles == null) return;

        if (isGrounded)
        {
            if (!movementParticles.isPlaying)
                movementParticles.Play();
        }
        else
        {
            if (movementParticles.isPlaying)
            {
                movementParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    // -------------------- Checks --------------------
    private void CheckRotationKill()
    {
        if (Physics2D.OverlapCircle(rotationKillCheck.position, rotationKillCheckRadius, groundLayer))
        {
            Debug.Log("Rotation kill");
            //PlayerLost();
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCapsule(
            groundCheck.position,
            groundCheckSize,
            CapsuleDirection2D.Horizontal, // can be Horizontal or Vertical
            0f, // rotation in degrees
            groundLayer
        );
    }

    // -------------------- Animator --------------------
    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded && rb.linearVelocity.y > 0);
        animator.SetBool("IsFalling", !isGrounded && rb.linearVelocity.y < 0);
    }

    // -------------------- Gizmos --------------------
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rotationKillCheck.position, rotationKillCheckRadius);
        }
    }
}