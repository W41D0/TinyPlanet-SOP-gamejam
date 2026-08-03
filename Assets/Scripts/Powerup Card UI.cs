using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupCardUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Image statusIconImage;

    [HideInInspector] public PowerUpSO currentPowerup;

    public void SetupCard(PowerUpSO powerupData, int nextLevel)
    {
        currentPowerup = powerupData;

        titleText.text = powerupData.powerupName;
        levelText.text = "LVL " + nextLevel;
        iconImage.sprite = powerupData.icon;
        statusIconImage.sprite = powerupData.statusEffectIcon;

        float rawValue = powerupData.GetValueAtLevel(nextLevel);
        
        float displayValue = (rawValue - 1f); 
        
        descriptionText.text = string.Format(powerupData.descriptionTemplate, displayValue);
    }
}