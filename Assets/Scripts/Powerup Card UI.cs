using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PowerupCardUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Image iconImage;
    public Image statusIconImage;

    [Header("Purchase Visuals")]
    public Color purchasedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color tooExpensiveTint = new Color(0.8f, 0.2f, 0.2f, 1f); 

    [HideInInspector] public PowerUpSO currentPowerup;

    private UpgradeManager manager;
    private Button cardButton;

    private int nextPriceLevel = 1;
    private int currentCost = 0; 
    private bool isPurchased = false; 

    private Graphic[] allCardGraphics;
    private Dictionary<Graphic, Color> originalColors = new Dictionary<Graphic, Color>();

    private void Awake()
    {
        cardButton = GetComponent<Button>();
        
        allCardGraphics = GetComponentsInChildren<Graphic>(true);
        foreach (Graphic g in allCardGraphics)
        {
            originalColors[g] = g.color;
        }
    }

    private void Start()
    {
        manager = Object.FindAnyObjectByType<UpgradeManager>();
    }

    public void OnCardClicked()
    {
        bool purchaseSuccessful = manager.TryPurchaseUpgrade(currentPowerup);
        
        if (purchaseSuccessful)
        {
            isPurchased = true; 
            
            // 0.4f makes the BOUGHT card 60% see-through!
            ApplyTint(purchasedTint, 0.6f); 
            
            if (cardButton != null) cardButton.interactable = false; 
            UpdateGoldText(nextPriceLevel);
        }
    }

    public void SetupCard(PowerUpSO powerupData, int nextLevel)
    {
        currentPowerup = powerupData;
        isPurchased = false; 

        if (cardButton != null) cardButton.interactable = true;
        
        // 1f keeps the card 100% solid when rolled!
        ApplyTint(Color.white, 1f); 

        titleText.text = powerupData.powerupName;
        levelText.text = "LVL " + nextLevel;
        iconImage.sprite = powerupData.icon;
        statusIconImage.sprite = powerupData.statusEffectIcon;

        nextPriceLevel = nextLevel + 1;

        UpdateGoldText(nextLevel);

        float rawValue = powerupData.GetValueAtLevel(nextLevel);
        string displayValue = rawValue.ToString("0.##"); 
        
        descriptionText.text = string.Format(powerupData.descriptionTemplate, displayValue);
    }

    public void CheckAffordability(int playerCoins)
    {
        if (isPurchased) return; 

        if (playerCoins >= currentCost)
        {
            // 1f keeps the card completely solid when you can afford it
            ApplyTint(Color.white, 1f); 
            if (cardButton != null) cardButton.interactable = true;
        }
        else
        {
            // 1f keeps the RED TINT completely solid when you are broke!
            ApplyTint(tooExpensiveTint, 1f); 
            if (cardButton != null) cardButton.interactable = false;
        }
    }

    private void ApplyTint(Color tintColor, float alphaMultiplier)
    {
        if (allCardGraphics != null)
        {
            foreach (Graphic g in allCardGraphics)
            {
                if (originalColors.ContainsKey(g))
                {
                    Color origColor = originalColors[g];
                    Color finalColor = origColor * tintColor;
                    
                    // This forces the transparency to completely ignore the inspector tint's alpha
                    // and strictly use the multiplier we gave it above!
                    finalColor.a = origColor.a * alphaMultiplier; 

                    g.color = finalColor;
                }
            }
        }
    }

    void UpdateGoldText(int nextLevel)
    {
        if (costText == null) return;
            
        int cost = currentPowerup.GetCostAtLevel(nextLevel - 1);
        currentCost = cost; 

        string formattedGold;

        if (cost >= 1_000_000_000)
        {
            formattedGold = (cost / 1_000_000_000f).ToString("0.#") + "B";
        }
        else if (cost >= 1_000_000)
        {
            formattedGold = (cost / 1_000_000f).ToString("0.#") + "M";
        }
        else if (cost >= 1_000)
        {
            formattedGold = (cost / 1_000f).ToString("0.#") + "K";
        }
        else
        {
            formattedGold = cost.ToString();
        }

        costText.text = $"{formattedGold}";
    }
}