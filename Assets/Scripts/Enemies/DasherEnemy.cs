using UnityEngine;

[RequireComponent(typeof(EnemyHealth), typeof(EnemyAttacks))]
public class DasherEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float normalSpeed = 3f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashPrepTime = 0.5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.3f; 
    [SerializeField] private float dashRecoveryTime = 1f;
    [SerializeField] private float dashCooldown = 3f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color prepColor = Color.red;

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; 
    private EnemyHealth healthScript;
    
    private enum State { Chasing, Preparing, Dashing, Recovering } 
    private State currentState;
    
    private float stateTimer;
    private Vector2 dashDirection; 

    void Start()
    {
        normalSpeed = normalSpeed * EnemyDifficultyManager.Instance.speedMultiplier;
        dashPrepTime = dashPrepTime * EnemyDifficultyManager.Instance.prepTimeMultiplier;
        dashSpeed = dashSpeed * EnemyDifficultyManager.Instance.dashSpeedMultiplier;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = normalColor;
        
        rb = GetComponent<Rigidbody2D>(); 
        healthScript = GetComponent<EnemyHealth>();
        
        if (rb != null)
        {
            rb.gravityScale = 0f; 
            rb.freezeRotation = true;
        }
        
        currentState = State.Chasing;
        stateTimer = dashCooldown;
    }

    void Update()
    {
        if (playerTarget == null) return;
        
        if (healthScript.IsKnockedBack) return; 

        switch (currentState)
        {
            case State.Chasing:
                Vector2 directionToPlayer = (playerTarget.position - transform.position).normalized;
                rb.linearVelocity = directionToPlayer * normalSpeed;
                
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = State.Preparing;
                    stateTimer = dashPrepTime;
                    rb.linearVelocity = Vector2.zero; 
                    if (spriteRenderer != null) spriteRenderer.color = prepColor;
                }
                break;

            case State.Preparing:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = State.Dashing;
                    stateTimer = dashDuration;
                    dashDirection = (playerTarget.position - transform.position).normalized;
                }
                break;

            case State.Dashing:
                rb.linearVelocity = dashDirection * dashSpeed;
                
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = State.Recovering;
                    stateTimer = dashRecoveryTime;
                    rb.linearVelocity = Vector2.zero; 
                    if (spriteRenderer != null) spriteRenderer.color = normalColor;
                }
                break;

            case State.Recovering:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = State.Chasing;
                    stateTimer = dashCooldown;
                }
                break;
        }
    }
}