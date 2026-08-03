    using UnityEngine;

    [RequireComponent(typeof(EnemyHealth), typeof(EnemyAttacks))]
    public class ShooterEnemy : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] private float speed = 3f;
        
        [Header("Movement Settings")]
        [SerializeField] private float stoppingDistance = 6f; 
        [SerializeField] private float retreatDistance = 4f;  

        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float shootingRange = 7f;
        [SerializeField] private float fireRate = 1.5f;
        [SerializeField] private float spreadAngle = 15f;
        
        private EnemyAttacks enemyAttacks;
        
        private Transform playerTarget;
        private Rigidbody2D rb;
        private EnemyHealth healthScript;
        private float fireTimer;

        void Start()
        {
            speed = speed * EnemyDifficultyManager.Instance.speedMultiplier;
            stoppingDistance = stoppingDistance * EnemyDifficultyManager.Instance.stoppingDistanceMultiplier;
            retreatDistance = retreatDistance * EnemyDifficultyManager.Instance.retreatDistanceMultiplier;
            shootingRange = shootingRange * EnemyDifficultyManager.Instance.shootingRangeMultiplier;
            fireRate = fireRate * EnemyDifficultyManager.Instance.fireRateMultiplier;
            spreadAngle = spreadAngle * EnemyDifficultyManager.Instance.spreadMultiplier;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
            
            rb = GetComponent<Rigidbody2D>();
            healthScript = GetComponent<EnemyHealth>();
            enemyAttacks = GetComponent<EnemyAttacks>();
            
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
            }
        }

        void Update()
        {
            if (playerTarget == null) return;
            if (healthScript.IsKnockedBack) return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (rb != null)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;

                if (distanceToPlayer > stoppingDistance) rb.linearVelocity = direction * speed;
                else if (distanceToPlayer < retreatDistance) rb.linearVelocity = -direction * speed;
                else rb.linearVelocity = Vector2.zero;
            }

            fireTimer -= Time.deltaTime;

            if (distanceToPlayer <= shootingRange && distanceToPlayer >= retreatDistance && fireTimer <= 0f)
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
            
            GameObject spawnedBullet = Instantiate(bulletPrefab, transform.position, rotation);

            // 2. Instantiate with the properties we need instantly
            spawnedBullet.GetComponent<EnemyBullet>().Initialize(
                (EnemyBullet.EnemyType)healthScript.myType, 
                enemyAttacks.playerHitPopupPrefab,
                enemyAttacks.damage 
            );
        }
    }