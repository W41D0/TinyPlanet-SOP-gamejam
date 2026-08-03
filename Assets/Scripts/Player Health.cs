using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] Slider healthBar;
    [SerializeField] Image fillImage; 
    [SerializeField] Gradient healthGradient; 
    
    [Header("Health & Status")]
    [SerializeField] private float totalHealth = 100f;
    [SerializeField] private float stunTime = 0f; 
    [SerializeField] float deathMenuDelay = 3f;     
    
    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private int flashCount = 6;
    [SerializeField] private SpriteRenderer spriteRenderer; 

    [Header("Extra")]
    [SerializeField] public float ThornsDamage = 0;

    [Header("Visual Effects")]
    [Tooltip("Fallback popup prefab used if the attacker doesn't specify one.")]
    [SerializeField] private GameObject damagePopupPrefab;

    private float currentHealth;
    bool isDead = false;
    
    private Rigidbody2D rb;
    private PlayerController movementScript;
    private Coroutine knockbackCoroutine;
    private Coroutine invincibilityCoroutine; 

    private bool isInvincible = false;
    private int lastDamageFrame = -1;
    private float highestDamageThisFrame = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerController>();

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        healthBar.maxValue = totalHealth;
        SetHealth(totalHealth);
    }

    void Update()
    {
        if (currentHealth <= 0f)
        {
            if(!isDead)
            {
                isDead = true;
                print("you lose");
            }
        }
    }

    public void SetHealth(float health)
    {
        if (health <= totalHealth && health >= 0f)
        {
            currentHealth = health;
        }
        else if (health > totalHealth)
        {
            currentHealth = totalHealth;
        }
        else if (health < 0f)
        {
            currentHealth = 0f;
        }
        
        healthBar.value = currentHealth;
        healthBar.gameObject.SetActive(currentHealth < totalHealth);

        if (fillImage != null)
        {
            fillImage.color = healthGradient.Evaluate(currentHealth / totalHealth);
        }
    }

    public void TakeDamage(float damage, float knockbackForce, Transform enemyTransform, bool triggersInvincibility = true, bool ignoresIframes = false, GameObject popupPrefabOverride = null)
    {
        if (isDead) return;

        bool hitOnSameFrame = (Time.frameCount == lastDamageFrame);

        if (isInvincible && !ignoresIframes && !hitOnSameFrame) return; 

        float actualDamageToApply = damage;

        if (hitOnSameFrame)
        {
            if (damage > highestDamageThisFrame)
            {
                actualDamageToApply = damage - highestDamageThisFrame; 
                highestDamageThisFrame = damage;

                if (enemyTransform != null && rb != null)
                {
                    ApplyKnockback(knockbackForce, enemyTransform);
                }
            }
            else
            {
                return; 
            }
        }
        else
        {
            lastDamageFrame = Time.frameCount;
            highestDamageThisFrame = damage;
            
            if (triggersInvincibility)
            {
                if (invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);
                invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
            }

            if (enemyTransform != null && rb != null)
            {
                ApplyKnockback(knockbackForce, enemyTransform);
            }
        }

        SetHealth(currentHealth - actualDamageToApply);

        // Use the attacker's popup prefab (already colored for its element) if it gave us one, otherwise our own default.
        GameObject prefabToUse = popupPrefabOverride != null ? popupPrefabOverride : damagePopupPrefab;

        if (prefabToUse != null && actualDamageToApply > 0)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), 0f);
            GameObject popup = Instantiate(prefabToUse, transform.position + randomOffset, Quaternion.identity);
            
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(actualDamageToApply);
            }
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            originalColor.a = 1f; 
            spriteRenderer.color = originalColor;

            float flashDuration = invincibilityDuration / (flashCount * 2);

            for (int i = 0; i < flashCount; i++)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
                yield return new WaitForSeconds(flashDuration);
                
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
            
            spriteRenderer.color = originalColor; 
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }

        isInvincible = false;
    }

    public void Heal(float healing)
    {
        SetHealth(currentHealth + healing);
    }

    public void setTotalHealth(float maxHealth)
    {
        if (totalHealth < maxHealth)
        {
            float healthPercentage = currentHealth / totalHealth;
            SetHealth(healthPercentage * maxHealth);
        }
        else
        {
            totalHealth = maxHealth;
        }

        healthBar.maxValue = totalHealth;
        healthBar.value = currentHealth;
    }

    public void IncreaseMaxHealth(float increase)
    {
        float healthPercentage = currentHealth / totalHealth;
        totalHealth += increase;
        
        healthBar.maxValue = totalHealth;
        SetHealth(healthPercentage * totalHealth);
    }

    public void DecreaseMaxHealth(float decrease)
    {
        totalHealth -= decrease;
        
        healthBar.maxValue = totalHealth;
        SetHealth(currentHealth);
    }

    public float getTotalHealth()
    {
        return totalHealth;
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    void ApplyKnockback(float knockbackForce, Transform enemy)
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        if (movementScript != null) 
            movementScript.enabled = false;

        rb.linearVelocity = Vector2.zero;
        
        Vector2 knockbackDir = (transform.position - enemy.position).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        knockbackCoroutine = StartCoroutine(KnockbackRecoveryRoutine());
    }

    IEnumerator KnockbackRecoveryRoutine()
    {
        yield return new WaitForSeconds(stunTime);
        EnableMovement();
    }

    void EnableMovement()
    {
        if (!isDead && movementScript != null)
            movementScript.enabled = true;
    }
}
