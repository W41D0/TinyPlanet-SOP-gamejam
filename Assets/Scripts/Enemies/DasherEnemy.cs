using UnityEngine;
using UnityEngine.UI;

public class DasherEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float normalSpeed = 3f;
    
    [Header("Damage Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float playerKnockbackForce = 10f;
    [SerializeField] private bool ignoresIframes = false;
    [SerializeField] private float damageCooldown = 1f; 

    [Header("Player-Hit Popup")]
    [Tooltip("Popup prefab shown on the PLAYER when this enemy deals damage. Assign the Solid/Liquid/Gas text prefab matching this enemy's type.")]
    [SerializeField] private GameObject playerHitPopupPrefab;

    [Header("Dash Settings")]
    [SerializeField] private float dashPrepTime = 0.5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.3f; 
    [SerializeField] private float dashRecoveryTime = 1f;
    [SerializeField] private float dashCooldown = 3f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color prepColor = Color.red;

    [Header("UI Settings")]
    [SerializeField] private GameObject damagePopupPrefab;
    private Slider healthBar;

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; 
    
    private enum State { Chasing, Preparing, Dashing, Recovering, KnockedBack } 
    private State currentState;
    
    private float stateTimer;
    private Vector2 dashDirection; 
    private float currentDamageCooldown = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = normalColor;
        
        rb = GetComponent<Rigidbody2D>(); 
        if (rb != null)
        {
            rb.gravityScale = 0f; 
            rb.freezeRotation = true;
        }
        
        currentState = State.Chasing;
        stateTimer = dashCooldown;

        healthBar = GetComponentInChildren<Slider>(true);
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
            healthBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentDamageCooldown > 0f) currentDamageCooldown -= Time.deltaTime;
        if (playerTarget == null) return;

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

            case State.KnockedBack:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    rb.linearVelocity = Vector2.zero; 
                    currentState = State.Chasing;
                    stateTimer = dashCooldown; 
                    if (spriteRenderer != null) spriteRenderer.color = normalColor;
                }
                break;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentDamageCooldown <= 0f)
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, playerKnockbackForce, transform, true, ignoresIframes, playerHitPopupPrefab);
                currentDamageCooldown = damageCooldown;
            }
        }
    }

    public void TakeDamage(float amount, bool isCrit = false)
    {
        health -= amount;

        if (healthBar != null) 
        {
            healthBar.value = health;
            healthBar.gameObject.SetActive(true);
        }

        if (damagePopupPrefab != null && amount > 0)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), 0f);
            GameObject popup = Instantiate(damagePopupPrefab, transform.position + randomOffset, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            
            if (popupScript != null)
            {
                popupScript.Setup(amount, isCrit);
            }
        }

        if (health <= 0) Die();
    }

    public void ApplyKnockback(Vector2 pushVector)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = State.KnockedBack;
            stateTimer = 0.2f; 
            rb.AddForce(pushVector, ForceMode2D.Impulse);
        }
    }

    void Die() { Destroy(gameObject); }
}