using UnityEngine;
using UnityEngine.UI;

public class ExplodingEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 5f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionTriggerRange = 2f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 5f;
    [SerializeField] private float playerKnockbackForce = 15f;
    [SerializeField] private bool ignoresIframes = false;
    [SerializeField] private float fuseTime = 1f;

    [Header("Player-Hit Popup")]
    [Tooltip("Popup prefab shown on the PLAYER when this enemy deals damage. Assign the Solid/Liquid/Gas text prefab matching this enemy's type.")]
    [SerializeField] private GameObject playerHitPopupPrefab;

    [Header("Visuals & UI")]
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private Color prepColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private GameObject damagePopupPrefab;
    
    private Slider healthBar;
    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;
    private bool isExploding = false;
    private float fuseTimer;
    private float knockbackTimer = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
            float scale = explosionRadius * 2f;
            rangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        }

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
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f && rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (playerTarget == null) return;

        if (!isExploding)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            if (rb != null) rb.linearVelocity = direction * speed;

            float distance = Vector2.Distance(transform.position, playerTarget.position);
            if (distance <= explosionTriggerRange) StartExplosion();
        }
        else
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            
            fuseTimer -= Time.deltaTime;
            if (fuseTimer <= 0f) Explode();
        }
    }

    void StartExplosion()
    {
        isExploding = true;
        fuseTimer = fuseTime;
        if (rangeIndicator != null) rangeIndicator.SetActive(true);
        if (spriteRenderer != null) spriteRenderer.color = prepColor;
    }

    void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(explosionDamage, playerKnockbackForce, transform, true, ignoresIframes, playerHitPopupPrefab);
                }
            }
        }
        Destroy(gameObject);
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

        if (health <= 0) Destroy(gameObject);
    }

    public void ApplyKnockback(Vector2 pushVector)
    {
        if (rb != null && !isExploding)
        {
            rb.linearVelocity = Vector2.zero;
            knockbackTimer = 0.2f;
            rb.AddForce(pushVector, ForceMode2D.Impulse);
        }
    }
}