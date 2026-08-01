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

    [Header("Gas Settings (Only works if Gas)")]
    [SerializeField] float knockbackForce = 2f;

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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float finalDamage = damage;
            bool isWeaknessHit = false;

            NormalEnemy normalEnemy = collision.gameObject.GetComponent<NormalEnemy>();
            if (normalEnemy != null &&
                ((typeOfBullet == BulletType.Solid && normalEnemy.myType == NormalEnemy.EnemyType.Solid) ||
                 (typeOfBullet == BulletType.Liquid && normalEnemy.myType == NormalEnemy.EnemyType.Liquid) ||
                 (typeOfBullet == BulletType.Gas && normalEnemy.myType == NormalEnemy.EnemyType.Gas)))
            {
                isWeaknessHit = true;
            }

            DasherEnemy dasherEnemy = collision.gameObject.GetComponent<DasherEnemy>();
            if (dasherEnemy != null &&
                ((typeOfBullet == BulletType.Solid && dasherEnemy.myType == DasherEnemy.EnemyType.Solid) ||
                 (typeOfBullet == BulletType.Liquid && dasherEnemy.myType == DasherEnemy.EnemyType.Liquid) ||
                 (typeOfBullet == BulletType.Gas && dasherEnemy.myType == DasherEnemy.EnemyType.Gas)))
            {
                isWeaknessHit = true;
            }

            ShooterEnemy shooterEnemy = collision.gameObject.GetComponent<ShooterEnemy>();
            if (shooterEnemy != null &&
                ((typeOfBullet == BulletType.Solid && shooterEnemy.myType == ShooterEnemy.EnemyType.Solid) ||
                 (typeOfBullet == BulletType.Liquid && shooterEnemy.myType == ShooterEnemy.EnemyType.Liquid) ||
                 (typeOfBullet == BulletType.Gas && shooterEnemy.myType == ShooterEnemy.EnemyType.Gas)))
            {
                isWeaknessHit = true;
            }

            if (isWeaknessHit)
            {
                finalDamage *= bonusDamageMultiplier;
            }

            if (typeOfBullet == BulletType.Gas)
            {
                if (hitEnemies.Contains(collision.gameObject)) return;

                hitEnemies.Add(collision.gameObject);
                collision.gameObject.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);

                Vector2 pushDirection = transform.up;
                collision.gameObject.SendMessage("ApplyKnockback", pushDirection * knockbackForce, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                collision.gameObject.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);
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