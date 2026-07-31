using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] Transform bulletSpawnObject;
    WeaponAim weaponAim;
    GameObject gun;
    Rigidbody2D gunRB;
    Vector2 bulletSpawnPosition;
    Quaternion bulletSpawnRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponAim = transform.parent.GetComponent<WeaponAim>();
        bulletSpawnObject = transform.Find("Bullet Spawn").GetComponent<Transform>();
        gun = gameObject;
        gunRB = gun.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(GameObject Bullet, float speed, float recoil, float spread)
    {
        bulletSpawnPosition = bulletSpawnObject.position;
        bulletSpawnRotation = bulletSpawnObject.rotation;


        float zSpread = Random.Range(-spread, spread);
        Quaternion randomZRotation = Quaternion.Euler(0f, 0f, zSpread);
        bulletSpawnRotation = bulletSpawnRotation * randomZRotation;

        GameObject spawnedBullet = Instantiate(Bullet, bulletSpawnPosition, bulletSpawnRotation);
        
        Rigidbody2D bulletRB = spawnedBullet.GetComponent<Rigidbody2D>();
        bulletRB.linearVelocity = spawnedBullet.transform.up * speed;
        weaponAim.ApplyRecoil(recoil);
    }

    
}
