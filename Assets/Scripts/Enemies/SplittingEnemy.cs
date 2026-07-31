using UnityEngine;

public class SplittingEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float healthDecayPerSecond = 1f;

    [Header("Split Settings")]
    [SerializeField] private GameObject prefabToSpawnOnDeath;
    [SerializeField] private int spawnCount = 2;
    [SerializeField] private float spawnOffset = 0.5f;

    private Transform playerTarget;
    private float decayTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);
        }

        decayTimer += Time.deltaTime;
        if (decayTimer >= 1f)
        {
            TakeDamage(healthDecayPerSecond);
            decayTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
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
        Destroy(gameObject);
    }
}