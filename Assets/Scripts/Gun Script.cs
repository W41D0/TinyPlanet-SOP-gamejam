using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] Transform bulletSpawnObject;
    
    GameObject gun;
    Rigidbody2D gunRB;
    Vector2 bulletSpawnPosition;
    Quaternion bulletSpawnRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletSpawnObject = transform.Find("Bullet Spawn").GetComponent<Transform>();
        gun = gameObject;
        gunRB = gun.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(GameObject Bullet, float speed, float recoil)
    {
        bulletSpawnPosition = bulletSpawnObject.position;
        bulletSpawnRotation = bulletSpawnObject.rotation;
        GameObject spawnedBullet = Instantiate(Bullet, bulletSpawnPosition, bulletSpawnRotation);
        Rigidbody2D bulletRB = spawnedBullet.GetComponent<Rigidbody2D>();
        bulletRB.linearVelocity = spawnedBullet.transform.up * speed;
        // gunRB.AddForce(-gun.transform.up * recoil, ForceMode2D.Impulse);
    }
}
