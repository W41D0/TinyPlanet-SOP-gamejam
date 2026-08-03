using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    [Header("Behavior Toggles")]
    public bool dealsContactDamage = true;
    public bool isProjectile = false;

    [Header("Damage Settings")]
    public float damage = 1f;
    public float playerKnockbackForce = 10f;
    public bool ignoresIframes = false;
    public float damageCooldown = 1f;

    [Header("Recoil & Thorns")]
    public float selfKnockbackForce = 5f; 
    public float selfDamageOnHit = 0f;

    [Header("Visuals")]
    public GameObject playerHitPopupPrefab;

    private float currentDamageCooldown = 0f;

    void Start()
    {
        damage = damage * EnemyDifficultyManager.Instance.damageMultiplier;
        playerKnockbackForce = playerKnockbackForce * EnemyDifficultyManager.Instance.knockbackMultiplier;    
    }

    void Update()
    {
        if (ignoresIframes && currentDamageCooldown > 0f) 
        {
            currentDamageCooldown -= Time.deltaTime;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!dealsContactDamage || isProjectile) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            TryDealDamage(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isProjectile) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            TryDealDamage(collision.gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }

    private void TryDealDamage(GameObject target)
    {
        if (ignoresIframes)
        {
            if (currentDamageCooldown > 0f) return; 
            
            HitPlayer(target);
            currentDamageCooldown = damageCooldown; 
        }
        else
        {
            HitPlayer(target);
        }
    }

    private void HitPlayer(GameObject target)
    {
        PlayerHealth ph = target.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage, playerKnockbackForce, transform, true, ignoresIframes, playerHitPopupPrefab);
        }

        if (isProjectile)
        {
            Destroy(gameObject); 
        }
        else 
        {
            EnemyHealth myHealth = GetComponent<EnemyHealth>();
            if (myHealth != null)
            {
                if (selfKnockbackForce > 0f)
                {
                    Vector2 pushDirection = (transform.position - target.transform.position).normalized;
                    myHealth.ApplyKnockback(pushDirection * selfKnockbackForce);
                }

                if (selfDamageOnHit > 0f)
                {
                    myHealth.TakeDamage(selfDamageOnHit, false);
                }
            }
        }
    }
}