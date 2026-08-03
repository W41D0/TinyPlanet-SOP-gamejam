using UnityEngine;

[RequireComponent(typeof(EnemyHealth), typeof(EnemyAttacks))]
public class SplittingEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float healthDecayPerSecond = 1f;

    [Header("Split Settings")]
    [SerializeField] private GameObject prefabToSpawnOnDeath;
    [SerializeField] private int spawnCount = 2;
    [SerializeField] private float spawnOffset = 0.5f;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private EnemyHealth healthScript;
    private float decayTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
        
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<EnemyHealth>();
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        // Automatically tell the Health script to call "Split" right before this enemy dies
        if (healthScript != null)
        {
            healthScript.OnDeath.AddListener(Split);
        }
    }

    void Update()
    {
        if (healthScript.IsKnockedBack) return;

        if (playerTarget != null && rb != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        decayTimer += Time.deltaTime;
        if (decayTimer >= 1f)
        {
            // Use the universal TakeDamage method to hurt itself over time
            healthScript.TakeDamage(healthDecayPerSecond); 
            decayTimer = 0f;
        }
    }

    void Split()
    {
        if (prefabToSpawnOnDeath != null)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * spawnOffset;
                Vector2 spawnPos = (Vector2)transform.position + randomOffset;
                Instantiate(prefabToSpawnOnDeath, spawnPos, Quaternion.identity);
            }
        }
    }
}