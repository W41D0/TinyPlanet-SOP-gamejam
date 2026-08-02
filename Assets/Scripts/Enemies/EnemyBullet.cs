using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }
    
    // Hidden in inspector because the ShooterEnemy will set it automatically
    [HideInInspector] public EnemyType myType = EnemyType.None;

    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 3f;
    
    [Header("Impact Settings")]
    [SerializeField] private float playerKnockbackForce = 5f;
    [SerializeField] private bool ignoresIframes = false;

    // Hidden in inspector because the ShooterEnemy will set it automatically, same as myType.
    [HideInInspector] public GameObject playerHitPopupPrefab;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, playerKnockbackForce, transform, true, ignoresIframes, playerHitPopupPrefab);
            }
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}