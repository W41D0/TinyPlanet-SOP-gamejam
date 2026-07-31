using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System.Collections;
using UnityEngine.Assemblies;

public class GunScript : MonoBehaviour
{
    [Header("Solid Bullets")]
    [SerializeField] private GameObject solidBullet;
    public GameObject SolidBullet {get => solidBullet; set => solidBullet = value;}

    [SerializeField] private float solidBulletSpeed = 10f;
    public float SolidBulletSpeed {get => solidBulletSpeed; set => solidBulletSpeed = value;}

    [SerializeField] private float timeBetweenSolidBullets = 0.5f;
    public float TimeBetweenSolidBullets {get => timeBetweenSolidBullets; set => timeBetweenSolidBullets = value;}

    [SerializeField] private float solidBulletRecoil = 2f;
    public float SolidBulletRecoil {get => solidBulletRecoil; set => solidBulletRecoil = value;}

    [SerializeField] private float solidBulletSpread = 5f;
    public float SolidBulletSpread {get => solidBulletSpread; set => solidBulletSpread = value;}

//----------------------------------------------------------------------------------------------------

    [Header("Liquid Bullets")]
    [SerializeField] private GameObject liquidBullet;
    public GameObject LiquidBullet {get => liquidBullet; set => liquidBullet = value;}

    [SerializeField] private float liquidBulletSpeed = 10f;
    public float LiquidBulletSpeed {get => liquidBulletSpeed; set => liquidBulletSpeed = value;}

    [SerializeField] private float timeBetweenLiquidBullets = 0.5f;
    public float TimeBetweenLiquidBullets {get => timeBetweenLiquidBullets; set => timeBetweenLiquidBullets = value;}

    [SerializeField] private float liquidBulletRecoil = 2f;
    public float LiquidBulletRecoil {get => liquidBulletRecoil; set => liquidBulletRecoil = value;}

    [SerializeField] private float liquidBulletSpread = 5f;
    public float LiquidBulletSpread {get => liquidBulletSpread; set => liquidBulletSpread = value;}

//----------------------------------------------------------------------------------------------------

    [Header("Gas Bullets")]
    [SerializeField] private GameObject gasBullet;
    public GameObject GasBullet {get => gasBullet; set => gasBullet = value;}

    [SerializeField] private float gasBulletSpeed = 10f;
    public float GasBulletSpeed {get => gasBulletSpeed; set => gasBulletSpeed = value;}

    [SerializeField] private float timeBetweenGasBullets = 0.5f;
    public float TimeBetweenGasBullets {get => timeBetweenGasBullets; set => timeBetweenGasBullets = value; }

    [SerializeField] private float gasBulletRecoil = 2f;
    public float GasBulletRecoil {get => gasBulletRecoil; set => gasBulletRecoil = value;}

    [SerializeField] private float gasBulletSpread = 5f;
    public float GasBulletSpread {get => gasBulletSpread; set => gasBulletSpread = value;}

//----------------------------------------------------------------------------------------------------

    float currentTotalMeter = 0f;

    [Header("Meter Behaviour")]
    [SerializeField] private float maxSolidMeter = 3f;
    [SerializeField] private float maxLiquidMeter = 3f;
    [SerializeField] private float maxGasMeter = 3f;


    public float SolidMeter => Mathf.Clamp(currentTotalMeter, 0, maxSolidMeter);

    public float LiquidMeter => Mathf.Clamp(currentTotalMeter - maxSolidMeter, 0, maxLiquidMeter);

    public float GasMeter => Mathf.Clamp(currentTotalMeter - (maxSolidMeter + maxLiquidMeter), 0, maxGasMeter);
//----------------------------------------------------------------------------------------------------
    
    [Header("Debug")]
    [SerializeField] Transform bulletSpawnObject;

    string currentState = "s";

    PlayerController player;
    WeaponAim weaponAim;
    GameObject gun;
    Rigidbody2D gunRB;
    Vector2 bulletSpawnPosition;
    Quaternion bulletSpawnRotation;

//----------------------------------------------------------------------------------------------------

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
        if(player.IsShooting && !player.IsOnShootCooldown)
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
        player.IsOnShootCooldown = true;
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
        player.IsOnShootCooldown = true;
    }
}
