using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Image fillImage; 
    [SerializeField] Gradient healthGradient; 
    
    [SerializeField] private float totalHealth = 100f;
    [SerializeField] private float stunTime = 0f; 
    [SerializeField] float deathMenuDelay = 3f;     
    
    private float currentHealth;
    bool isDead = false;
    
    private Rigidbody2D rb;
    private PlayerController movementScript;
    private Coroutine knockbackCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerController>();

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

    public void TakeDamage(float damage, float knockbackForce, Transform enemyTransform)
    {
        if (isDead) return;

        SetHealth(currentHealth - damage);

        if (enemyTransform != null && rb != null)
        {
            ApplyKnockback(knockbackForce, enemyTransform);
        }
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

    [ContextMenu("Test Health")]
    void healthTest()
    {
        SetHealth(currentHealth / 2f);
    }

    [ContextMenu("Test Health 2")]
    void healthTest2()
    {
        TakeDamage(20, 2, transform);
    }
}