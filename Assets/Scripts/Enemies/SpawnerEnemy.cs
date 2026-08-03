using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class SpawnerEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float speed = 3f;

    [Header("Movement Settings")]
    [SerializeField] private float stoppingDistance = 5f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnInterval = 3f;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private EnemyHealth healthScript;
    private float spawnTimer;

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

        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (playerTarget == null) return;
        if (healthScript.IsKnockedBack) return;

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
}