using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GunScript gun;
    Rigidbody2D rb;
    Vector2 moveDirection;
    Vector2 lastMoveDirection = Vector2.right; // Remembers direction so you can dash from a standstill

    [HideInInspector] public float SpeedMultiplier = 1f;
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float SprintMultiplier = 1.5f;
    [HideInInspector] public float DashCooldownMultiplier = 1f;
    [HideInInspector] public float DashSpeedMultiplier = 1f;
    float walkSpeed;
    bool isSprinting;

    bool isShooting;
    public bool IsShooting { get => isShooting; set => isShooting = value; }

    bool isOnShootCooldown;
    public bool IsOnShootCooldown { get => isOnShootCooldown; set => isOnShootCooldown = value; }

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float ghostSpawnRate = 0.05f;
    [SerializeField] private string enemyLayerName = "Enemy"; // Type your enemy layer name here

    [Header("Dash Audio")]
    public AudioSource audioSource; 
    public AudioClip dashClip; 

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    // --- State Variables ---
    private bool isDashing;
    private float lastDashTime = -100f; // Ensures dash is ready immediately on start
    [HideInInspector] public bool isInvincible; // Use this in your health/damage script!

    void Start()
    {
        Transform gunTransform = transform.Find("Weapon Pivot/Gun");
        if (gunTransform != null) gun = gunTransform.GetComponent<GunScript>();
        
        rb = gameObject.GetComponent<Rigidbody2D>();
        walkSpeed = baseMoveSpeed;
    }

    void Update()
    {
        // 1. Only allow normal movement if we are NOT dashing.
        // Otherwise, this overrides the dash velocity every frame!
        if (!isDashing)
        {
            rb.linearVelocity = moveDirection * walkSpeed * SpeedMultiplier;

            if (spriteRenderer != null && moveDirection.x != 0f)
            {
                spriteRenderer.flipX = moveDirection.x < 0f;
            }

            if (animator != null)
            {
                animator.SetFloat("Speed", moveDirection.magnitude);
            }
        }
    }

    void OnMove(InputValue value)
    {
        if (!isSprinting)
        {
            walkSpeed = baseMoveSpeed;
        }
        moveDirection = value.Get<Vector2>();

        // Update the last faced direction if we are actually moving
        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection.normalized;
        }
    }

    void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            walkSpeed = baseMoveSpeed * SprintMultiplier;
            isSprinting = true;
        }
        else
        {
            walkSpeed = baseMoveSpeed;
            isSprinting = false;
        }
    }

    void OnFire(InputValue value)
    {
        isShooting = value.isPressed;
    }

    // --- NEW DASH INPUT ---
    void OnDash(InputValue value)
    {
        // Check if button pressed, cooldown is over, and we aren't already dashing
        if (value.isPressed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    // --- COROUTINES ---
    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // 1. TURN ON I-FRAMES (Uses your PlayerHealth script!)
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null) health.isInvincible = true;

        if (audioSource != null && dashClip != null)
        {
            audioSource.PlayOneShot(dashClip);
        }

        Vector2 dashDir = moveDirection == Vector2.zero ? lastMoveDirection : moveDirection.normalized;

        // PHASE THROUGH ENEMIES
        int playerLayer = gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        if (enemyLayer != -1) 
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }

        rb.linearVelocity = dashDir * dashSpeed;
        Coroutine ghostRoutine = StartCoroutine(GhostTrailRoutine());

        yield return new WaitForSeconds(dashDuration);

        // CLEANUP
        StopCoroutine(ghostRoutine);
        rb.linearVelocity = Vector2.zero; 

        if (enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }

        // 2. TURN OFF I-FRAMES
        if (health != null) health.isInvincible = false;
        
        isDashing = false;
    }

    private IEnumerator GhostTrailRoutine()
    {
        while (isDashing)
        {
            // Spawn a blank GameObject
            GameObject ghost = new GameObject("Ghost_VFX");
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;
            ghost.transform.localScale = transform.localScale;

            // Give it a sprite renderer matching the player's current frame
            SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
            ghostSr.sprite = spriteRenderer.sprite;
            ghostSr.color = new Color(1f, 1f, 1f, 0.2f); // 20% opacity white
            ghostSr.sortingOrder = spriteRenderer.sortingOrder - 1; // Put it behind the player
            ghostSr.flipX = spriteRenderer.flipX;

            // Tell Unity to destroy it after 0.2 seconds
            Destroy(ghost, 0.2f);

            // Wait a tiny bit before spawning the next ghost
            yield return new WaitForSeconds(ghostSpawnRate);
        }
    }
}