using UnityEngine;

[RequireComponent(typeof(EnemyAttacks))]
public class EnemyBullet : MonoBehaviour
{
    public enum EnemyType { Solid, Liquid, Gas, None }
    
    [HideInInspector] public EnemyType myType = EnemyType.None;

    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    public void Initialize(EnemyType type, GameObject popupPrefab, float inheritedDamage)
    {
        myType = type;
        
        EnemyAttacks enemyAttacks = GetComponent<EnemyAttacks>();
        if (enemyAttacks != null)
        {
            enemyAttacks.playerHitPopupPrefab = popupPrefab;
            enemyAttacks.damage = inheritedDamage;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}