using UnityEngine;
using System.Collections;
using UnityEngine.Assemblies;
using System.ComponentModel.Design.Serialization;
using UnityEngine.UIElements;

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

    // REPLACE YOUR METER PROPERTIES WITH THESE:
    public float MaxSolid => maxSolidMeter * (stats != null ? stats.solidMatterTimeMult : 1f);
    public float MaxLiquid => maxLiquidMeter * (stats != null ? stats.liquidMatterTimeMult : 1f);
    public float MaxGas => maxGasMeter * (stats != null ? stats.gasMatterTimeMult : 1f);
    public float MaxPlasma => maxPlasmaMeter * (stats != null ? stats.plasmaMatterTimeMult : 1f);

    public float SolidMeter => Mathf.Clamp(currentTotalMeter, 0, MaxSolid);
    public float LiquidMeter => Mathf.Clamp(currentTotalMeter - MaxSolid, 0, MaxLiquid);
    public float GasMeter => Mathf.Clamp(currentTotalMeter - (MaxSolid + MaxLiquid), 0, MaxGas);
    public float MaxTotalMeter => MaxSolid + MaxLiquid + MaxGas;
    public float PlasmaMeter = 0f;

    [SerializeField] private float meterDepletionRate = 1f;
    public float MeterDepletionRate {get => meterDepletionRate; set => meterDepletionRate = value;}

    [SerializeField] private float timeBeforeCooldown = 1f;
    public float TimeBeforeCooldown {get => timeBeforeCooldown; set => timeBeforeCooldown = value;}

    float timeSinceLastShot = 0f;
//----------------------------------------------------------------------------------------------------
    [Header("UI Reference")]
    //[SerializeField] private ProceduralPhaseMeter uiMeter;
    [SerializeField] private PhaseClockMeter uiMeter;
    [Header("VFX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    Transform bulletSpawnObject;

    string currentState = "s";
    //bool inPlasmaMode = false;
    private PlayerStats stats;

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
        stats = transform.parent.parent.GetComponent<PlayerStats>();
        weaponAim = transform.parent.GetComponent<WeaponAim>();
        bulletSpawnObject = transform.Find("Bullet Spawn").GetComponent<Transform>();
        gun = gameObject;
        gunRB = gun.GetComponent<Rigidbody2D>();

        if (muzzleFlash != null)
        {
            var main = muzzleFlash.main;
            main.playOnAwake = false;
            main.loop = false;
        }
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
        // --- UPDATE UI METER MESH ---
        if (uiMeter != null)
        {
            uiMeter.UpdateClock(
                currentTotalMeter,
                maxSolidMeter,       
                maxLiquidMeter,      
                maxLiquidMeter,         
                PlasmaMeter,
                maxPlasmaMeter,      
                currentState
            );
        }
        Debug.Log(currentTotalMeter + "My State is: " + currentState);
    }

    private void Shoot(GameObject prefab, float spread, float speed, float recoil, float damageMult, float knockbackMult, float rangeMult)
    {
        bulletSpawnPosition = bulletSpawnObject.position;
        bulletSpawnRotation = bulletSpawnObject.rotation;

        float zSpread = Random.Range(-spread, spread);
        Quaternion randomZRotation = Quaternion.Euler(0f, 0f, zSpread);
        bulletSpawnRotation = bulletSpawnRotation * randomZRotation;

        GameObject spawnedBullet = Instantiate(prefab, bulletSpawnPosition, bulletSpawnRotation);

        spawnedBullet.GetComponent<BulletBehaviour>().InitializeBullet(damageMult, knockbackMult, rangeMult);

        Rigidbody2D bulletRB = spawnedBullet.GetComponent<Rigidbody2D>();
        bulletRB.linearVelocity = spawnedBullet.transform.up * speed;
        weaponAim.ApplyRecoil(recoil);

        // --- TRIGGER MUZZLE FLASH ---
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }
        timeSinceLastShot = 0f;
    }

    public void ShootSolid()
    {
        // Velocity scales up with Fire Rate!
        float finalSpeed = solidBulletSpeed * stats.solidFireRateMult; 
        Shoot(solidBullet, solidBulletSpread * stats.solidSpreadMult, finalSpeed, solidBulletRecoil, stats.solidDamageMult, stats.solidKnockbackMult, stats.solidRangeMult);
    }

    public void ShootLiquid()
    {
        float finalSpeed = liquidBulletSpeed * stats.liquidFireRateMult;
        Shoot(liquidBullet, liquidBulletSpread * stats.liquidSpreadMult, finalSpeed, liquidBulletRecoil, stats.liquidDamageMult, stats.liquidKnockbackMult, stats.liquidRangeMult);
    }

    public void ShootGas()
    {
        float finalSpeed = gasBulletSpeed * stats.gasFireRateMult;
        Shoot(gasBullet, gasBulletSpread * stats.gasSpreadMult, finalSpeed, gasBulletRecoil, stats.gasDamageMult, stats.gasKnockbackMult, stats.gasRangeMult);
    }

    public void ShootPlasma()
    {
        float finalSpeed = plasmaBulletSpeed * stats.plasmaFireRateMult;
        Shoot(plasmaBullet, plasmaBulletSpread, finalSpeed, plasmaBulletRecoil, stats.plasmaDamageMult, stats.plasmaKnockbackMult, stats.plasmaRangeMult);
        Debug.Log("Spread is :" + plasmaBulletSpread);
    }

    IEnumerator ShootMethod()
    {
        player.IsOnShootCooldown = true;
        switch (currentState)
        {
            case "s":
                ShootSolid();
                yield return new WaitForSeconds(timeBetweenSolidBullets / stats.solidFireRateMult);
                break;
            case "l":
                ShootLiquid();
                yield return new WaitForSeconds(timeBetweenLiquidBullets / stats.liquidFireRateMult);
                break;
            case "g":
                ShootGas();
                yield return new WaitForSeconds(timeBetweenGasBullets / stats.gasFireRateMult);
                break;
            case "p":
                ShootPlasma();
                yield return new WaitForSeconds(timeBetweenPlasmaBullets / stats.plasmaFireRateMult);
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
        if (timeSinceLastShot > timeBeforeCooldown && currentTotalMeter > 0)
        {
            currentTotalMeter -= Time.deltaTime;
            if (currentTotalMeter < 0)
                currentTotalMeter = 0;
        }
    }
}
