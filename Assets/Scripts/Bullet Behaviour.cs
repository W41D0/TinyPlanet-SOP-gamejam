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

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                float finalDamage = damage;
                bool isWeaknessHit = typeOfBullet == BulletType.Plasma;

                if (!isWeaknessHit)
                {
                    if ((typeOfBullet == BulletType.Solid && enemyHealth.myType == EnemyType.Solid) ||
                        (typeOfBullet == BulletType.Liquid && enemyHealth.myType == EnemyType.Liquid) ||
                        (typeOfBullet == BulletType.Gas && enemyHealth.myType == EnemyType.Gas))
                    {
                        isWeaknessHit = true;
                    }
                }

                if (isWeaknessHit) finalDamage *= bonusDamageMultiplier;

                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                Vector2 knockbackVector = pushDirection * knockbackForce;

                enemyHealth.TakeDamage(finalDamage, isWeaknessHit);
                enemyHealth.ApplyKnockback(knockbackVector);
            }
            
            if (!canPierce) 
                Destroy(gameObject);
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