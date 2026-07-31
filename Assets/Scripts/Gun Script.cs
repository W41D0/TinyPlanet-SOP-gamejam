using Unity.Mathematics;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] Transform bulletSpawnObject;
    
    Vector2 bulletSpawnPosition;
    Quaternion bulletSpawnRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletSpawnObject = transform.Find("Bullet Spawn").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(GameObject Bullet, float speed)
    {
        bulletSpawnPosition = bulletSpawnObject.position;
        bulletSpawnRotation = bulletSpawnObject.rotation;
        GameObject spawnedBullet = Instantiate(Bullet, bulletSpawnPosition, bulletSpawnRotation);
        Rigidbody2D bulletRB = spawnedBullet.GetComponent<Rigidbody2D>();
        bulletRB.linearVelocity = spawnedBullet.transform.up * speed;
    }
}
