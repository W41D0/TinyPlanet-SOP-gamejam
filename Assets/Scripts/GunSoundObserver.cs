using UnityEngine;

public class GunSoundObserver : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource oneShotSource;
    public AudioSource loopSource;

    [Header("Bullet Sounds")]
    public AudioClip solidSound;
    public AudioClip liquidSound;
    public AudioClip gasSound;
    public AudioClip plasmaSound;

    [Header("Settings")]
    public bool isPlasmaContinuous = false;

    private GunScript gunScript;
    private PlayerController player;
    private bool wasOnCooldown = false;

    void Start()
    {
        gunScript = GetComponent<GunScript>();
        player = transform.parent.parent.GetComponent<PlayerController>();

        if (oneShotSource == null)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
        }
        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
        }
    }

    // التغيير السحري هنا: استخدام LateUpdate بدلاً من Update
    void LateUpdate()
    {
        if (player != null && gunScript != null)
        {
            bool isShooting = player.IsShooting;
            
            bool isSolid = gunScript.SolidMeter < gunScript.MaxSolid;
            bool isLiquid = !isSolid && gunScript.LiquidMeter < gunScript.MaxLiquid;
            bool isGas = !isSolid && !isLiquid && gunScript.GasMeter < gunScript.MaxGas;
            bool isPlasma = !isSolid && !isLiquid && !isGas;

            bool useLoop = false;
            AudioClip currentLoopClip = null;

            if (isLiquid) 
            {
                useLoop = true;
                currentLoopClip = liquidSound;
            }
            else if (isPlasma && isPlasmaContinuous)
            {
                useLoop = true;
                currentLoopClip = plasmaSound;
            }

            if (isShooting && useLoop)
            {
                if (loopSource.clip != currentLoopClip) loopSource.clip = currentLoopClip;
                if (!loopSource.isPlaying && currentLoopClip != null) loopSource.Play();
            }
            else
            {
                if (loopSource.isPlaying) loopSource.Stop();
            }

            bool isOnCooldown = player.IsOnShootCooldown;

            if (isOnCooldown && !wasOnCooldown && !useLoop)
            {
                if (isSolid && solidSound != null)
                {
                    oneShotSource.PlayOneShot(solidSound);
                }
                else if (isGas && gasSound != null)
                {
                    oneShotSource.PlayOneShot(gasSound);
                }
                else if (isPlasma && !isPlasmaContinuous && plasmaSound != null)
                {
                    oneShotSource.PlayOneShot(plasmaSound);
                }
            }

            wasOnCooldown = isOnCooldown;
        }
    }
}