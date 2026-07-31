using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System.Collections;
using UnityEngine.Assemblies;
using System.ComponentModel.Design.Serialization;

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

    [Header("Plasma Bullets")]
    [SerializeField] private GameObject plasmaBullet;
    public GameObject PlasmaBullet {get => plasmaBullet; set => plasmaBullet = value;}

    [SerializeField] private float plasmaBulletSpeed = 10f;
    public float PlasmaBulletSpeed {get => plasmaBulletSpeed; set => plasmaBulletSpeed = value;}

    [SerializeField] private float timeBetweenPlasmaBullets = 0.5f;
    public float TimeBetweenPlasmaBullets {get => timeBetweenPlasmaBullets; set => timeBetweenPlasmaBullets = value;}

    [SerializeField] private float plasmaBulletRecoil = 2f;
    public float PlasmaBulletRecoil {get => plasmaBulletRecoil; set => plasmaBulletRecoil = value;}

    [SerializeField] private float plasmaBulletSpread = 5f;
    public float PlasmaBulletSpread {get => plasmaBulletSpread; set => plasmaBulletSpread = value;}

//----------------------------------------------------------------------------------------------------

    float currentTotalMeter = 0f;
    

    [Header("Meter Behaviour")]
    [SerializeField] private float maxSolidMeter = 3f;
    [SerializeField] private float maxLiquidMeter = 3f;
    [SerializeField] private float maxGasMeter = 3f;
    [SerializeField] private float maxPlasmaMeter = 2f;

    public float SolidMeter => Mathf.Clamp(currentTotalMeter, 0, maxSolidMeter);

    public float LiquidMeter => Mathf.Clamp(currentTotalMeter - maxSolidMeter, 0, maxLiquidMeter);

    public float GasMeter => Mathf.Clamp(currentTotalMeter - (maxSolidMeter + maxLiquidMeter), 0, maxGasMeter);

    public float PlasmaMeter = 0f;

    [SerializeField] private float meterDepletionRate = 1f;
    public float MeterDepletionRate {get => meterDepletionRate; set => meterDepletionRate = value;}

    [SerializeField] private float timeBeforeCooldown = 1f;
    public float TimeBeforeCooldown {get => timeBeforeCooldown; set => timeBeforeCooldown = value;}

    float timeSinceLastShot = 0f;
//----------------------------------------------------------------------------------------------------
    
    Transform bulletSpawnObject;

    string currentState = "s";
    bool inPlasmaMode = false;

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
        updateState();
        
        if(player.IsShooting)
        {
            if(currentState != "p")
                currentTotalMeter += Time.deltaTime;
            if (!player.IsOnShootCooldown)
                StartCoroutine(ShootMethod());
        }
        else
        {
            timeSinceLastShot += Time.deltaTime;
            if(currentState != "p")
                decreaseMeter();
        }

        if(currentState == "p")
        {
            PlasmaMeter += Time.deltaTime;
            if (PlasmaMeter > maxPlasmaMeter)
            {
                PlasmaMeter = 0f;
                currentTotalMeter = 0;
            }
        }

        Debug.Log(currentTotalMeter + "My State is: " + currentState);
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

        timeSinceLastShot = 0f;
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

    public void ShootPlasma()
    {
        Shoot(plasmaBullet, plasmaBulletSpread, plasmaBulletSpeed, plasmaBulletRecoil);
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

            case "p":
                ShootPlasma();
                yield return new WaitForSeconds(timeBetweenPlasmaBullets);
                break;
        }
        player.IsOnShootCooldown = false;
    }

    public void updateState()
    {
        if (currentTotalMeter < maxSolidMeter)
        {
            currentState = "s";
        }
        else if (currentTotalMeter < maxSolidMeter + maxLiquidMeter)
        {
            currentState = "l";
        }
        else if (currentTotalMeter < maxSolidMeter + maxLiquidMeter + maxGasMeter)
        {
            currentState = "g";
        }
        else 
        {
            currentState = "p"; 
        }
    }

    void decreaseMeter()
    {
        if (timeSinceLastShot > timeBeforeCooldown)
        {
            currentTotalMeter -= Time.deltaTime;
        }
    }
}
