using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float contactDamage = 1f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootingRange = 7f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float spreadAngle = 15f;

    private Transform playerTarget;
    private float fireTimer;

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
        if (playerTarget == null) return;

        transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        fireTimer -= Time.deltaTime;

        if (distanceToPlayer <= shootingRange && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float finalAngle = angle + Random.Range(-spreadAngle, spreadAngle);
        
        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
        Instantiate(bulletPrefab, transform.position, rotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("TakeDamage", contactDamage, SendMessageOptions.DontRequireReceiver);
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

    public void ApplyKnockback(Vector2 pushVector)
    {
        transform.position = (Vector2)transform.position + pushVector;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}