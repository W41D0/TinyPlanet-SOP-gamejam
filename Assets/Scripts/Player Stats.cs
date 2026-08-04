using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Live Global Additions")]
    public float healthMultiplier = 1f;
    public float thornsDamage = 0f;

    [HideInInspector] public float movementSpeedMultiplier = 1f;
    [HideInInspector] public float dashCooldownMultiplier = 1f; // <-- NEW
    [HideInInspector] public float dashSpeedMultiplier = 1f;    // <-- NEW

    // --------------------------------------------------------
    // LIVE MATTER MULTIPLIERS
    // --------------------------------------------------------
    [HideInInspector] public float solidDamageMult = 1f;
    [HideInInspector] public float solidKnockbackMult = 1f;
    [HideInInspector] public float solidFireRateMult = 1f;
    [HideInInspector] public float solidSpreadMult = 1f;
    [HideInInspector] public float solidMatterTimeMult = 1f;
    [HideInInspector] public float solidRangeMult = 1f; 

    [HideInInspector] public float liquidDamageMult = 1f;
    [HideInInspector] public float liquidKnockbackMult = 1f;
    [HideInInspector] public float liquidFireRateMult = 1f;
    [HideInInspector] public float liquidSpreadMult = 1f;
    [HideInInspector] public float liquidMatterTimeMult = 1f;
    [HideInInspector] public float liquidRangeMult = 1f; 

    [HideInInspector] public float gasDamageMult = 1f;
    [HideInInspector] public float gasKnockbackMult = 1f;
    [HideInInspector] public float gasFireRateMult = 1f;
    [HideInInspector] public float gasSpreadMult = 1f;
    [HideInInspector] public float gasMatterTimeMult = 1f;
    [HideInInspector] public float gasRangeMult = 1f; 

    // --------------------------------------------------------
    // PLASMA BLITZ MULTIPLIERS
    // --------------------------------------------------------
    [HideInInspector] public float plasmaDamageMult = 1f;
    [HideInInspector] public float plasmaKnockbackMult = 1f;
    [HideInInspector] public float plasmaFireRateMult = 1f;
    [HideInInspector] public float plasmaSpreadMult = 1f;
    [HideInInspector] public float plasmaMatterTimeMult = 1f;
    [HideInInspector] public float plasmaRangeMult = 1f; 

    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private float initialBaseHealth; 

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();
        initialBaseHealth = playerHealth.getTotalHealth(); 
    }

    public void RecalculateStats(Dictionary<PowerUpSO, int> inventory)
    {
        // 1. WIPE THE SLATE CLEAN
        healthMultiplier = 1f;
        thornsDamage = 0f;
        movementSpeedMultiplier = 1f;
        dashCooldownMultiplier = 1f; // <-- NEW
        dashSpeedMultiplier = 1f;    // <-- NEW

        solidDamageMult = 1f; liquidDamageMult = 1f; gasDamageMult = 1f;
        solidKnockbackMult = 1f; liquidKnockbackMult = 1f; gasKnockbackMult = 1f;
        solidFireRateMult = 1f; liquidFireRateMult = 1f; gasFireRateMult = 1f;
        solidSpreadMult = 1f; liquidSpreadMult = 1f; gasSpreadMult = 1f;
        solidMatterTimeMult = 1f; liquidMatterTimeMult = 1f; gasMatterTimeMult = 1f;

        plasmaDamageMult = 1f; plasmaKnockbackMult = 1f; plasmaFireRateMult = 1f; 
        plasmaSpreadMult = 1f; plasmaMatterTimeMult = 1f; plasmaRangeMult = 1f;

        solidRangeMult = 1f; liquidRangeMult = 1f; gasRangeMult = 1f; 

        // 2. APPLY ALL INVENTORY ITEMS
        foreach (var kvp in inventory)
        {
            PowerUpSO powerup = kvp.Key;
            int levelReached = kvp.Value; 
            
            // --- NEW: CUMULATIVE MATH ---
            float totalValForThisCard = 0f;
            for (int i = 1; i <= levelReached; i++)
            {
                totalValForThisCard += powerup.GetValueAtLevel(i);
            }

            // Now apply the TRUE total to the player!
            switch (powerup.statToModify)
            {
                case StatModifierType.MaxHealth: healthMultiplier += totalValForThisCard; break;
                case StatModifierType.Thorns: thornsDamage += totalValForThisCard; break;
                case StatModifierType.Speed: movementSpeedMultiplier += totalValForThisCard; break;
                case StatModifierType.DashCooldown: dashCooldownMultiplier += totalValForThisCard; break; // <-- NEW
                case StatModifierType.DashDistance: dashSpeedMultiplier += totalValForThisCard; break;    // <-- NEW
                
                case StatModifierType.Range: 
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidRangeMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidRangeMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasRangeMult += totalValForThisCard;
                    break;
                
                case StatModifierType.Damage:
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidDamageMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidDamageMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasDamageMult += totalValForThisCard;
                    break;

                case StatModifierType.Knockback:
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidKnockbackMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidKnockbackMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasKnockbackMult += totalValForThisCard;
                    break;

                case StatModifierType.FireRate:
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidFireRateMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidFireRateMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasFireRateMult += totalValForThisCard;
                    break;

                case StatModifierType.Spread:
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidSpreadMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidSpreadMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasSpreadMult += totalValForThisCard;
                    break;

                case StatModifierType.MatterTime:
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Solid) solidMatterTimeMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Liquid) liquidMatterTimeMult += totalValForThisCard;
                    if (powerup.matterToModify == MatterType.All || powerup.matterToModify == MatterType.Gas) gasMatterTimeMult += totalValForThisCard;
                    break;
            }
        }

        // 3. APPLY PLASMA MERGE LOGIC
        plasmaDamageMult = solidDamageMult + liquidDamageMult + gasDamageMult * 0.5f;

        // 4. PUSH FLAT UPDATES
        playerHealth.ThornsDamage = thornsDamage;
        playerHealth.setTotalHealth(initialBaseHealth * healthMultiplier);
        
        if (playerController != null) 
        {
            playerController.SpeedMultiplier = movementSpeedMultiplier;
            playerController.DashCooldownMultiplier = dashCooldownMultiplier; // <-- NEW
            playerController.DashSpeedMultiplier = dashSpeedMultiplier;       // <-- NEW
        }
    }
}