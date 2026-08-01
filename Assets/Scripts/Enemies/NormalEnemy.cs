using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Enemy Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float damage = 1f;

    private Transform playerTarget;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }

    void Update()
    {
        if (playerTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);
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

    public void ApplyKnockback(Vector2 pushVector)
    {
        transform.position = (Vector2)transform.position + pushVector;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}