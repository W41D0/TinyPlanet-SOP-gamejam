using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;

    [Header("Movement Settings")]
    [SerializeField] private float stoppingDistance = 5f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnInterval = 3f;

    private Transform playerTarget;
    private float spawnTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);
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
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}