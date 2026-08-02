using UnityEngine;
using UnityEngine.UI; 

public class NormalEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Enemy Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;
    
    [Header("Damage Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float playerKnockbackForce = 10f;
    [SerializeField] private bool ignoresIframes = false;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Player-Hit Popup")]
    [SerializeField] private GameObject playerHitPopupPrefab;

    [Header("UI Settings")]
    [SerializeField] private GameObject damagePopupPrefab;
    
    private Slider healthBar; 
    private Transform playerTarget;
    private Rigidbody2D rb;
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

        if (playerTarget != null && rb != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
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
            knockbackTimer = 0.2f;
            rb.AddForce(pushVector, ForceMode2D.Impulse);
        }
    }

    void Die() { Destroy(gameObject); }
}