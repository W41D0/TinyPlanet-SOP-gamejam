using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class ExplodingEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float speed = 5f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionTriggerRange = 2f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 5f;
    [SerializeField] private float playerKnockbackForce = 15f;
    [SerializeField] private bool ignoresIframes = false;
    [SerializeField] private float fuseTime = 1f;
    [SerializeField] private GameObject playerHitPopupPrefab;

    [Header("Visuals")]
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private Color prepColor = new Color(1f, 0f, 0f, 0.3f);
    
    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private EnemyHealth healthScript;
    private bool isExploding = false;
    private float fuseTimer;

    void Start()
    {
        speed = speed * EnemyDifficultyManager.Instance.speedMultiplier;
        explosionDamage = explosionDamage * EnemyDifficultyManager.Instance.explosionDamageMultiplier;
        explosionRadius = explosionRadius * EnemyDifficultyManager.Instance.radiusMultiplier;
        playerKnockbackForce = playerKnockbackForce * EnemyDifficultyManager.Instance.knockbackForceMultiplier;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<EnemyHealth>();

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
    }

    void Update()
    {
        if (playerTarget == null) return;
        if (healthScript.IsKnockedBack) return;

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
}