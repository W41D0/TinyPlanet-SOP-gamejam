using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 5f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionTriggerRange = 2f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 5f;
    [SerializeField] private float fuseTime = 1f;

    [Header("Visuals")]
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private Color prepColor = new Color(1f, 0f, 0f, 0.3f);

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isExploding = false;
    private float fuseTimer;

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
            originalColor = spriteRenderer.color;
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
            float scale = explosionRadius * 2f;
            rangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        if (!isExploding)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);

            float distance = Vector2.Distance(transform.position, playerTarget.position);
            if (distance <= explosionTriggerRange)
            {
                StartExplosion();
            }
        }
        else
        {
            fuseTimer -= Time.deltaTime;
            if (fuseTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void StartExplosion()
    {
        isExploding = true;
        fuseTimer = fuseTime;

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = prepColor;
        }
    }

    void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject);
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