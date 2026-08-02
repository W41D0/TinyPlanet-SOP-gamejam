using UnityEngine;
using UnityEngine.UI;

public class ShooterEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;
    
    [Header("Movement Settings")]
    [SerializeField] private float stoppingDistance = 6f; 
    [SerializeField] private float retreatDistance = 4f;  

    [Header("Contact Damage Settings")]
    [SerializeField] private float contactDamage = 1f;
    [SerializeField] private float playerKnockbackForce = 5f;
    [SerializeField] private bool ignoresIframes = false;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Player-Hit Popup")]
    [Tooltip("Popup prefab shown on the PLAYER when this enemy's contact damage hits them. Assign the Solid/Liquid/Gas text prefab matching this enemy's type.")]
    [SerializeField] private GameObject playerHitPopupPrefab;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootingRange = 7f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float spreadAngle = 15f;

    [Header("UI Settings")]
    [SerializeField] private GameObject damagePopupPrefab;
    private Slider healthBar;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private float fireTimer;
    private float currentDamageCooldown = 0f;
    private float knockbackTimer = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
        
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
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
        if (currentDamageCooldown > 0f) currentDamageCooldown -= Time.deltaTime;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f && rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (rb != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;

            if (distanceToPlayer > stoppingDistance) rb.linearVelocity = direction * speed;
            else if (distanceToPlayer < retreatDistance) rb.linearVelocity = -direction * speed;
            else rb.linearVelocity = Vector2.zero;
        }

        fireTimer -= Time.deltaTime;

        if (distanceToPlayer <= shootingRange && distanceToPlayer >= retreatDistance && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float finalAngle = angle + Random.Range(-spreadAngle, spreadAngle);
        
        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
        GameObject spawnedBullet = Instantiate(bulletPrefab, transform.position, rotation);

        EnemyBullet bulletScript = spawnedBullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.myType = (EnemyBullet.EnemyType)myType;
            bulletScript.playerHitPopupPrefab = playerHitPopupPrefab;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentDamageCooldown <= 0f)
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage, playerKnockbackForce, transform, true, ignoresIframes, playerHitPopupPrefab);
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
            knockbackTimer = 0.2f;
            rb.AddForce(pushVector, ForceMode2D.Impulse);
        }
    }

    void Die() { Destroy(gameObject); }
}