using TMPro;
using UnityEngine;

public class CoinBag : MonoBehaviour
{
    public static CoinBag Instance;

    [Header("Wallet")]
    public int totalCoins = 0;

    [Header("Conversion Settings")]
    [Tooltip("How many coins you get per 1 damage. (e.g., 2 means 1 dmg = 2 coins. 0.5 means 2 dmg = 1 coin)")]
    [SerializeField] private float coinMultiplier = 1f;

    public TextMeshProUGUI GoldText; // Note: If using UI Text (Canvas), change to TextMeshProUGUI

    private float partialCoins = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize the UI text when the game starts
        UpdateGoldText();
    }

    public void AddCoins(float damageAmount)
    {
        float calculatedCoins = damageAmount * coinMultiplier;
        
        partialCoins += calculatedCoins;

        if (partialCoins >= 1f)
        {
            int wholeCoinsEarned = Mathf.FloorToInt(partialCoins);
            totalCoins += wholeCoinsEarned;
            
            partialCoins -= wholeCoinsEarned; 

            // Update the UI only when actual coins are added
            UpdateGoldText();
        }
    }

    public string UpdateGoldText()
    {
        if (GoldText == null) return "0";

        string formattedGold;

        // Note: Standard int max value is ~2.14 Billion. 
        // If you plan to go past 2 Billion, change totalCoins to a 'long' or 'float'.
        if (totalCoins >= 1_000_000_000)
        {
            formattedGold = (totalCoins / 1_000_000_000f).ToString("0.#") + "B";
        }
        else if (totalCoins >= 1_000_000)
        {
            formattedGold = (totalCoins / 1_000_000f).ToString("0.#") + "M";
        }
        else if (totalCoins >= 1_000)
        {
            formattedGold = (totalCoins / 1_000f).ToString("0.#") + "K";
        }
        else
        {
            formattedGold = totalCoins.ToString();
        }

        GoldText.text = $"{formattedGold} G";
        return $"{formattedGold} G";
    }
}