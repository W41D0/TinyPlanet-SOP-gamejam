using UnityEngine;

public enum StatModifierType
{
    MaxHealth,
    Defense,
    Damage,
    Speed,
    Range,
    Knockback,
    FireRate,
    Spread,
    Thorns,
    MatterTime,
    DashDistance,
    DashCooldown
}

public enum MatterType
{
    Solid,
    Liquid,
    Gas,
    Plasma,
    All
}

[CreateAssetMenu(fileName = "PowerUpSO", menuName = "Scriptable Objects/PowerUpSO")]
public class PowerUpSO : ScriptableObject
{
    [Header("Display Info")]
    public string powerupName;
    public Sprite icon;
    public Sprite statusEffectIcon;

    [TextArea] public string descriptionTemplate;

    [Header("Infinite Progression")]
    public int maxLevel = 0; // 0 means it can scale infinitely
    public float baseValue;
    
    public float multiplierPerLevel;

    [Header("Infinite Shop Settings")]
    public float baseCost = 10f;
    public float costMultiplierPerLevel = 1.5f;

    [Header("System Tags")]
    public StatModifierType statToModify;
    public MatterType matterToModify;

    public float GetValueAtLevel(int level)
    {
        if (level <= 0) return 0;

            if (statToModify == StatModifierType.Thorns)
        {
            return baseValue * Mathf.Pow(multiplierPerLevel, level - 1);
        }
        
        return (baseValue - 1f) * Mathf.Pow(multiplierPerLevel, level - 1);
    }

    public int GetCostAtLevel(int level)
    {
        if (level <= 0) return (int)baseCost;
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplierPerLevel, level));
    }
}