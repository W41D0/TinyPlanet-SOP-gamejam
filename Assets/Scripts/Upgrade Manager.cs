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
        RollRandomUpgrades();    
    }

    public void RollRandomUpgrades()
    {
        Time.timeScale = 0f;
        upgradeUIPanel.SetActive(true);


        List<PowerUpSO> pool = new List<PowerUpSO>(allAvailablePowerups);

        for (int i = 0; i < upgradeCards.Length; i++)
        {
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
    }
}