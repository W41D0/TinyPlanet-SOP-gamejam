using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Master Data")]
    public List<PowerUpSO> allAvailablePowerups;

    [Header("UI References")]
    public PowerupCardUI[] upgradeCards;
    public GameObject upgradeUIPanel;

    private Dictionary<PowerUpSO, int> playerInventory = new Dictionary<PowerUpSO, int>();

    private void Awake() 
    {
        //RollRandomUpgrades();    
    }

    public void RollRandomUpgrades()
    {
        Time.timeScale = 0f;
        upgradeUIPanel.SetActive(true);

        // 1. BUILD A FILTERED POOL
        List<PowerUpSO> pool = new List<PowerUpSO>();
        foreach (var powerup in allAvailablePowerups)
        {
            int currentLevel = 0;
            if (playerInventory.ContainsKey(powerup))
            {
                currentLevel = playerInventory[powerup];
            }

            // ONLY add to the pool if it's infinite (0) OR we haven't hit the max level yet
            if (powerup.maxLevel == 0 || currentLevel < powerup.maxLevel)
            {
                pool.Add(powerup);
            }
        }

        // 2. ASSIGN TO UI CARDS
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // Safety Check: Do we still have cards left in the pool?
            if (pool.Count > 0)
            {
                // Make sure the UI card is visible
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
                // The pool ran dry! Hide any leftover UI card slots so they aren't blank/broken.
                upgradeCards[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectUpgrade(PowerUpSO chosenPowerup)
    {
        if (playerInventory.ContainsKey(chosenPowerup))
        {
            playerInventory[chosenPowerup]++;
        }
        else
        {
            playerInventory.Add(chosenPowerup, 1);
        }

        int newLevel = playerInventory[chosenPowerup];
        Debug.Log("Acquired: " + chosenPowerup.powerupName + " | Now Level: " + newLevel);

        // ---> DO YOUR PLAYER BUFF LOGIC HERE <---
        FindAnyObjectByType<PlayerStats>().RecalculateStats(playerInventory);
        
        upgradeUIPanel.SetActive(false);
        Time.timeScale = 1f;

        FindAnyObjectByType<WaveManager>().StartRound();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Heal(playerHealth.getTotalHealth());
    }
}