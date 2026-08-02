using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public enum BulletType { Solid, Liquid, Gas, Plasma }

    [Header("General Settings")]
    [SerializeField] private BulletType typeOfBullet = BulletType.Solid;
    [SerializeField] float maxLifeTime = 3f;
    [SerializeField] float maxRange = 15f;
    [SerializeField] float damage = 1f;
    [SerializeField] float bonusDamageMultiplier = 2f;
    [SerializeField] float knockbackForce = 2f;
    [SerializeField] bool canPierce = false;

    [Header("Gas Settings (Only works if Gas)")]
    [SerializeField] bool gasGrowth = false;
    [SerializeField] float gasGrowthAmmount = 0.2f;

    private Vector3 startPostion;
    private List<GameObject> hitEnemies = new List<GameObject>();

    void Start()
    {
        startPostion = transform.position;
        StartCoroutine(DestroyAfterCertainTime());
    }

    void Update()
    {
        float distanceTraveled = Vector3.Distance(startPostion, transform.position);
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
        
        if (typeOfBullet == BulletType.Gas && gasGrowth)
        {
            Vector3 currentScale = gameObject.transform.localScale;
            currentScale.x += gasGrowthAmmount * Time.deltaTime;
            gameObject.transform.localScale = currentScale;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hitEnemies.Contains(collision.gameObject)) return;
            hitEnemies.Add(collision.gameObject);

            NormalEnemy normalEnemy = collision.gameObject.GetComponent<NormalEnemy>();
            DasherEnemy dasherEnemy = collision.gameObject.GetComponent<DasherEnemy>();
            ShooterEnemy shooterEnemy = collision.gameObject.GetComponent<ShooterEnemy>();
            SpawnerEnemy spawnerEnemy = collision.gameObject.GetComponent<SpawnerEnemy>();
            SplittingEnemy splittingEnemy = collision.gameObject.GetComponent<SplittingEnemy>();
            ExplodingEnemy explodingEnemy = collision.gameObject.GetComponent<ExplodingEnemy>();

            float finalDamage = damage;
            bool isWeaknessHit = typeOfBullet == BulletType.Plasma;

            if (!isWeaknessHit)
            {
                if (normalEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && normalEnemy.myType == NormalEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && normalEnemy.myType == NormalEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && normalEnemy.myType == NormalEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }

                if (dasherEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && dasherEnemy.myType == DasherEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && dasherEnemy.myType == DasherEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && dasherEnemy.myType == DasherEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }

                if (shooterEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && shooterEnemy.myType == ShooterEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && shooterEnemy.myType == ShooterEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && shooterEnemy.myType == ShooterEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }

                if (spawnerEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && spawnerEnemy.myType == SpawnerEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && spawnerEnemy.myType == SpawnerEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && spawnerEnemy.myType == SpawnerEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }

                if (splittingEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && splittingEnemy.myType == SplittingEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && splittingEnemy.myType == SplittingEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && splittingEnemy.myType == SplittingEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }

                if (explodingEnemy != null &&
                    ((typeOfBullet == BulletType.Solid && explodingEnemy.myType == ExplodingEnemy.EnemyType.Solid) ||
                    (typeOfBullet == BulletType.Liquid && explodingEnemy.myType == ExplodingEnemy.EnemyType.Liquid) ||
                    (typeOfBullet == BulletType.Gas && explodingEnemy.myType == ExplodingEnemy.EnemyType.Gas)))
                {
                    isWeaknessHit = true;
                }
            }
                if (isWeaknessHit)
                {
                    finalDamage *= bonusDamageMultiplier;
                }

            Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
            Vector2 knockbackVector = pushDirection * knockbackForce;

            if (normalEnemy != null) { normalEnemy.TakeDamage(finalDamage, isWeaknessHit); normalEnemy.ApplyKnockback(knockbackVector); }
            if (dasherEnemy != null) { dasherEnemy.TakeDamage(finalDamage, isWeaknessHit); dasherEnemy.ApplyKnockback(knockbackVector); }
            if (shooterEnemy != null) { shooterEnemy.TakeDamage(finalDamage, isWeaknessHit); shooterEnemy.ApplyKnockback(knockbackVector); }
            if (spawnerEnemy != null) { spawnerEnemy.TakeDamage(finalDamage, isWeaknessHit); spawnerEnemy.ApplyKnockback(knockbackVector); }
            if (splittingEnemy != null) { splittingEnemy.TakeDamage(finalDamage, isWeaknessHit); splittingEnemy.ApplyKnockback(knockbackVector); }
            if (explodingEnemy != null) { explodingEnemy.TakeDamage(finalDamage, isWeaknessHit); explodingEnemy.ApplyKnockback(knockbackVector); }
            
            if (!canPierce)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    IEnumerator DestroyAfterCertainTime()
    {
        yield return new WaitForSeconds(maxLifeTime);
        Destroy(gameObject);
    }
}