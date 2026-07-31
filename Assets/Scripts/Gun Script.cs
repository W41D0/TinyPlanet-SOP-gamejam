using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System.Collections;
using UnityEngine.Assemblies;

public class GunScript : MonoBehaviour
{
    [Header("Solid Bullets")]
    [SerializeField] GameObject solidBullet;
    [SerializeField] float solidBulletSpeed = 10f;
    [SerializeField] float timeBetweenSolidBullets = 0.5f;
    [SerializeField] float solidBulletRecoil = 2f;
    [SerializeField] float solidBulletSpread = 5f;

    [Header("Liquid Bullets")]
    [SerializeField] GameObject liquidBullet;
    [SerializeField] float liquidBulletSpeed = 10f;
    [SerializeField] float timeBetweenLiquidBullets = 0.5f;
    [SerializeField] float liquidBulletRecoil = 2f;
    [SerializeField] float liquidBulletSpread = 5f;

    [Header("Gas Bullets")]
    [SerializeField] GameObject gasBullet;
    [SerializeField] float gasBulletSpeed = 10f;
    [SerializeField] float timeBetweenGasBullets = 0.5f;
    [SerializeField] float gasBulletRecoil = 2f;
    [SerializeField] float gasBulletSpread = 5f;

    [Header("Debug")]
    [SerializeField] Transform bulletSpawnObject;

    string currentState = "s";

    PlayerController player;
    WeaponAim weaponAim;
    GameObject gun;
    Rigidbody2D gunRB;
    Vector2 bulletSpawnPosition;
    Quaternion bulletSpawnRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.parent.GetComponent<PlayerController>();
        weaponAim = transform.parent.GetComponent<WeaponAim>();
        bulletSpawnObject = transform.Find("Bullet Spawn").GetComponent<Transform>();
        gun = gameObject;
        gunRB = gun.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetIsShooting() && !player.GetIsOnShootCooldown())
        {
            StartCoroutine(ShootMethod());
        }
    }

    private void Shoot(GameObject prefab, float spread, float speed, float recoil)
    {
        bulletSpawnPosition = bulletSpawnObject.position;
        bulletSpawnRotation = bulletSpawnObject.rotation;

        float zSpread = Random.Range(-spread, spread);
        Quaternion randomZRotation = Quaternion.Euler(0f, 0f, zSpread);
        bulletSpawnRotation = bulletSpawnRotation * randomZRotation;

        GameObject spawnedBullet = Instantiate(prefab, bulletSpawnPosition, bulletSpawnRotation);

        Rigidbody2D bulletRB = spawnedBullet.GetComponent<Rigidbody2D>();
        bulletRB.linearVelocity = spawnedBullet.transform.up * speed;
        weaponAim.ApplyRecoil(recoil);
    }

    public void ShootSolid()
    {
        Shoot(solidBullet, solidBulletSpread, solidBulletSpeed, solidBulletRecoil);
    }

    public void ShootLiquid()
    {
        Shoot(liquidBullet, liquidBulletSpread, liquidBulletSpeed, liquidBulletRecoil);
    }

    public void ShootGas()
    {
        Shoot(gasBullet, gasBulletSpread, gasBulletSpeed, gasBulletRecoil);
    }

    IEnumerator ShootMethod()
    {
        player.SetIsOnShootCooldown(true);
        switch (currentState)
        {
            case "s":
                ShootSolid();
                yield return new WaitForSeconds(timeBetweenSolidBullets);
                break;
                
            case "l":
                ShootLiquid();
                yield return new WaitForSeconds(timeBetweenLiquidBullets);
                break;
                
            case "g":
                ShootGas();
                yield return new WaitForSeconds(timeBetweenGasBullets);
                break;
        }
        player.SetIsOnShootCooldown(false);
    }
}
