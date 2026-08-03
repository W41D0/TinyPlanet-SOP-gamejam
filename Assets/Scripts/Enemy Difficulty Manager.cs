using System;
using UnityEngine;

public class EnemyDifficultyManager : MonoBehaviour
{
    public static EnemyDifficultyManager Instance;

    [Header("All Enemy Growth Rates")]
    public float healthGrowthPerRound = 1.15f;
    public float damageGrowthPerRound = 1.15f;

    public float knockbackGrowthPerRound = 1.01f;
    [SerializeField] int knockbackMaxRounds = 20;

    public float speedGrowthPerRound = 1.015f;
    [SerializeField] int speedMaxRounds = 20;

    [Header("Shooters")]
    public float stoppingDistanceGrowthPerRound = 1.03f;
    public float retreatDistanceGrowthPerRound = 1.01f;
    public float shootingRangeGrowthPerRound = 1.03f;
    public float fireRateGrowthPerRound = 1.02f;
    public float spreadGrowthPerRound = 0.95f;
    [SerializeField] int shooterMaxRound = 20;

    [Header("Dashers")]
    public float PrepTimeGrowthPerRound = 0.975f;
    public float dashSpeedGrowthPerRound = 1.015f;
    [SerializeField] int dasherMaxRound = 20;

    [Header("Exploder")]
    public float explosionDamageGrowthPerRound = 1.15f; 
    public float radiusGrowthPerRound = 1.04f;
    public float knockbackForceGrowthPerRound = 1.02f;
    [SerializeField] int exploderMaxRound = 20;

    // --------------------------------------------------------
    // LIVE MULTIPLIERS
    // --------------------------------------------------------
    [HideInInspector] public float healthMultiplier = 1f;
    [HideInInspector] public float damageMultiplier = 1f;
    [HideInInspector] public float knockbackMultiplier = 1f;
    [HideInInspector] public float speedMultiplier = 1f;

    // Shooters
    [HideInInspector] public float stoppingDistanceMultiplier = 1f;
    [HideInInspector] public float retreatDistanceMultiplier = 1f;
    [HideInInspector] public float shootingRangeMultiplier = 1f;
    [HideInInspector] public float fireRateMultiplier = 1f;
    [HideInInspector] public float spreadMultiplier = 1f;

    // Dashers
    [HideInInspector] public float prepTimeMultiplier = 1f;
    [HideInInspector] public float dashSpeedMultiplier = 1f;

    // Exploders
    [HideInInspector] public float explosionDamageMultiplier = 1f;
    [HideInInspector] public float radiusMultiplier = 1f;
    [HideInInspector] public float knockbackForceMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CalculateMultipliers(int currentRound)
    {
        if (currentRound <= 0) return;

        // --- GLOBAL STATS ---
        healthMultiplier = Mathf.Pow(healthGrowthPerRound, currentRound - 1);
        damageMultiplier = Mathf.Pow(damageGrowthPerRound, currentRound - 1);
        
        // Capped Global Stats
        knockbackMultiplier = Mathf.Pow(knockbackGrowthPerRound, Mathf.Min(currentRound, knockbackMaxRounds) - 1);
        speedMultiplier = Mathf.Pow(speedGrowthPerRound, Mathf.Min(currentRound, speedMaxRounds) - 1);

        // --- SHOOTERS ---
        int shooterRound = Mathf.Min(currentRound, shooterMaxRound);
        stoppingDistanceMultiplier = Mathf.Pow(stoppingDistanceGrowthPerRound, shooterRound - 1);
        retreatDistanceMultiplier = Mathf.Pow(retreatDistanceGrowthPerRound, shooterRound - 1);
        shootingRangeMultiplier = Mathf.Pow(shootingRangeGrowthPerRound, shooterRound - 1);
        fireRateMultiplier = Mathf.Pow(fireRateGrowthPerRound, shooterRound - 1);
        spreadMultiplier = Mathf.Pow(spreadGrowthPerRound, shooterRound - 1);

        // --- DASHERS ---
        int dasherRound = Mathf.Min(currentRound, dasherMaxRound);
        prepTimeMultiplier = Mathf.Pow(PrepTimeGrowthPerRound, dasherRound - 1);
        dashSpeedMultiplier = Mathf.Pow(dashSpeedGrowthPerRound, dasherRound - 1);

        // --- EXPLODERS ---
        int exploderRound = Mathf.Min(currentRound, exploderMaxRound);
        radiusMultiplier = Mathf.Pow(radiusGrowthPerRound, exploderRound - 1);
        knockbackForceMultiplier = Mathf.Pow(knockbackForceGrowthPerRound, exploderRound - 1);
        explosionDamageMultiplier = Mathf.Pow(explosionDamageGrowthPerRound, currentRound - 1);// No round limit for explosion damage

    }
}