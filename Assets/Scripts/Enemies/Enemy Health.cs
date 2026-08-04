using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public EnemyType myType = EnemyType.None;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [Header("UI Settings")]
    [SerializeField] private GameObject damagePopupPrefab;
    private Slider healthBar;

    private Rigidbody2D rb;
    private float knockbackTimer = 0f;
    
    public bool IsKnockedBack => knockbackTimer > 0f; 

    [Header("Events")]
    public UnityEvent OnDeath; 

    void Start()
    {
        maxHealth = maxHealth * EnemyDifficultyManager.Instance.healthMultiplier;
        
        currentHealth = maxHealth;
        
        rb = GetComponent<Rigidbody2D>();
        
        healthBar = GetComponentInChildren<Slider>(true);
        if (healthBar != null)
        {            
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f && rb != null) 
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(float amount, bool isCrit = false)
    {
        currentHealth -= amount;

        if(CoinBag.Instance != null)
            CoinBag.Instance.AddCoins(amount);
        
        if (healthBar != null) 
        {
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(true);
        }

        if (damagePopupPrefab != null && amount > 0)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), 0f);
            GameObject popup = Instantiate(damagePopupPrefab, transform.position + randomOffset, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            
            if (popupScript != null) popupScript.Setup(amount, isCrit);
        }

        if (currentHealth <= 0) Die();
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

    private void Die() 
    { 
        OnDeath?.Invoke();
        Destroy(gameObject); 
    }
}