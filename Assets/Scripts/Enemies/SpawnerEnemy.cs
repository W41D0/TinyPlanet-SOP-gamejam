using UnityEngine;
using UnityEngine.UI;

public class SpawnerEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;

    [Header("Movement Settings")]
    [SerializeField] private float stoppingDistance = 5f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnInterval = 3f;

    [Header("UI Settings")]
    [SerializeField] private GameObject damagePopupPrefab;
    private Slider healthBar;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private float spawnTimer;
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

        spawnTimer = spawnInterval;

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

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (rb != null)
        {
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
            else rb.linearVelocity = Vector2.zero;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEntity();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEntity()
    {
        if (prefabToSpawn != null) Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
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
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            knockbackTimer = 0.2f;
            rb.AddForce(pushVector, ForceMode2D.Impulse);
        }
    }
}