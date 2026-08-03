// NormalEnemy.cs
using UnityEngine;

// This ensures Unity automatically adds our new scripts to the prefab
[RequireComponent(typeof(EnemyHealth), typeof(EnemyAttacks))]
public class NormalEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 3f;
    
    private Transform playerTarget;
    private Rigidbody2D rb;
    private EnemyHealth healthScript;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
        
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<EnemyHealth>(); // Grab the health script
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Check if the health script says we are currently knocked back
        if (healthScript.IsKnockedBack) return; 

        // If not knocked back, chase the player!
        if (playerTarget != null && rb != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }
}