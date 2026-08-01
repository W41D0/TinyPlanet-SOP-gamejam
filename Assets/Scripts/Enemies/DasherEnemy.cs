using UnityEngine;

public class DasherEnemy : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }

    [Header("Base Settings")]
    public EnemyType myType = EnemyType.None; 
    [SerializeField] private float health = 10f;
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float damage = 1f;

    [Header("Dash Settings")]
    [SerializeField] private float dashPrepTime = 0.5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashRecoveryTime = 1f;
    [SerializeField] private float dashCooldown = 3f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color prepColor = Color.red;

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    
    private enum State { Chasing, Preparing, Dashing, Recovering }
    private State currentState;
    
    private float stateTimer;
    private Vector2 dashTargetPosition;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
        
        currentState = State.Chasing;
        stateTimer = dashCooldown;
    }

    void Update()
    {
        if (playerTarget == null) return;

        switch (currentState)
        {
            case State.Chasing:
                transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, normalSpeed * Time.deltaTime);
                stateTimer -= Time.deltaTime;
                
                if (stateTimer <= 0f)
                {
                    currentState = State.Preparing;
                    stateTimer = dashPrepTime;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = prepColor;
                    }
                }
                break;

            case State.Preparing:
                stateTimer -= Time.deltaTime;
                
                if (stateTimer <= 0f)
                {
                    currentState = State.Dashing;
                    Vector2 direction = (playerTarget.position - transform.position).normalized;
                    dashTargetPosition = (Vector2)transform.position + direction * dashDistance;
                    
                    stateTimer = (dashDistance / dashSpeed) + 0.2f;
                }
                break;

            case State.Dashing:
                transform.position = Vector2.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.deltaTime);
                stateTimer -= Time.deltaTime;

                if (Vector2.Distance(transform.position, dashTargetPosition) < 0.1f || stateTimer <= 0f)
                {
                    currentState = State.Recovering;
                    stateTimer = dashRecoveryTime;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = normalColor;
                    }
                }
                break;

            case State.Recovering:
                stateTimer -= Time.deltaTime;
                
                if (stateTimer <= 0f)
                {
                    currentState = State.Chasing;
                    stateTimer = dashCooldown;
                }
                break;
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