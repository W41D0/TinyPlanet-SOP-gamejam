using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Master Data")]
    public List<PowerUpSO> allAvailablePowerups;

    [Header("UI References")]
    public PowerupCardUI[] upgradeCards;
    public GameObject upgradeUIPanel;
    public TextMeshProUGUI GoldText;

    [Header("Animation Settings")]
    public CanvasGroup shopCanvasGroup;
    public float animationDuration = 0.25f;

    private Dictionary<PowerUpSO, int> playerInventory = new Dictionary<PowerUpSO, int>();

    public void RollRandomUpgrades()
    {
        upgradeUIPanel.SetActive(true);
        StartCoroutine(AnimateShopOpen());

        List<PowerUpSO> pool = new List<PowerUpSO>();
        foreach (var powerup in allAvailablePowerups)
        {
            int currentLevel = 0;
            if (playerInventory.ContainsKey(powerup))
            {
                currentLevel = playerInventory[powerup];
            }

            if (powerup.maxLevel == 0 || currentLevel < powerup.maxLevel)
            {
                pool.Add(powerup);
            }
        }

        for (int i = 0; i < upgradeCards.Length; i++)
        {
            if (pool.Count > 0)
            {
                upgradeCards[i].gameObject.SetActive(true);

                int randomIndex = Random.Range(0, pool.Count);
                PowerUpSO chosenPowerup = pool[randomIndex];

                pool.RemoveAt(randomIndex);

                int currentLevel = 0;
                if (playerInventory.ContainsKey(chosenPowerup))
                {
                    currentLevel = playerInventory[chosenPowerup];
                }
                int nextLevel = currentLevel + 1;

                upgradeCards[i].SetupCard(chosenPowerup, nextLevel);
            }
            else
            {
                upgradeCards[i].gameObject.SetActive(false);
            }
        }

        GoldText.text = "REMAINING GOLD: " + CoinBag.Instance.UpdateGoldText();
        RefreshAllCardsAffordability(); // Triggers the color check when shop opens!
    }

    public bool TryPurchaseUpgrade(PowerUpSO chosenPowerup)
    {
        int currentLevel = 0;
        if (playerInventory.ContainsKey(chosenPowerup))
        {
            currentLevel = playerInventory[chosenPowerup];
        }

        int cost = chosenPowerup.GetCostAtLevel(currentLevel);

        if (CoinBag.Instance != null && CoinBag.Instance.totalCoins >= cost)
        {
            CoinBag.Instance.totalCoins -= cost;
            GoldText.text = "REMAINING GOLD: " + CoinBag.Instance.UpdateGoldText();

            if (playerInventory.ContainsKey(chosenPowerup))
            {
                playerInventory[chosenPowerup]++;
            }
            else
            {
                playerInventory.Add(chosenPowerup, 1);
            }

            FindAnyObjectByType<PlayerStats>().RecalculateStats(playerInventory);
            
            RefreshAllCardsAffordability(); // Re-checks the other cards instantly!

            return true; 
        }
        else
        {
            return false; 
        }
    }

    private void RefreshAllCardsAffordability()
    {
        if (CoinBag.Instance == null) return;

        foreach (var card in upgradeCards)
        {
            if (card.gameObject.activeSelf)
            {
                card.CheckAffordability(CoinBag.Instance.totalCoins);
            }
        }
    }

    public void CloseShopAndStartRound()
    {
        upgradeUIPanel.SetActive(false);
        Time.timeScale = 1f;

        FindAnyObjectByType<WaveManager>().StartRound();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(playerHealth.getTotalHealth());
            }
        }
    }

    private System.Collections.IEnumerator AnimateShopOpen()
    {
        shopCanvasGroup.alpha = 0f;
        upgradeUIPanel.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        
        Time.timeScale = 0.1f; 

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; 
            float percentage = elapsedTime / animationDuration;

            float ease = Mathf.Sin(percentage * Mathf.PI * 0.5f);

            shopCanvasGroup.alpha = ease;
            upgradeUIPanel.transform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 1f), Vector3.one, ease);

            yield return null;
        }

        shopCanvasGroup.alpha = 1f;
        upgradeUIPanel.transform.localScale = Vector3.one;
        Time.timeScale = 0f;
    }
}