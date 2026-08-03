using System.Collections.Generic;
using UnityEngine;
using TMPro; // For the Clock UI

public class WaveManager : MonoBehaviour
{
    [Header("Round Settings")]
    public float roundDuration = 60f;
    public int currentRound = 1;
    private float roundTimer;
    private bool isRoundActive = false;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;

    private SpawnPoint[] spawnPoints;
    
    [Tooltip("Seconds between spawns at Round 1")]
    public float baseSpawnRate = 3f; 
    
    [Tooltip("The absolute fastest enemies can spawn")]
    public float maxSpawnRate = 0.5f; 
    
    [Tooltip("The round where the spawn rate stops getting faster")]
    public int roundToMaxSpawnRate = 10; 
    
    private float currentSpawnRate;
    private float spawnTimer;

    [Header("UI & Systems")]
    public TextMeshProUGUI clockText; 
    public UpgradeManager upgradeManager;

    void Start()
    {
        spawnPoints = FindObjectsByType<SpawnPoint>();
        
        StartRound();
    }

    void Update()
    {
        if (!isRoundActive) return;

        roundTimer -= Time.deltaTime;
        UpdateClockUI();

        if (roundTimer <= 0)
        {
            EndRound();
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = currentSpawnRate;
        }
    }

    public void StartRound()
    {
        roundTimer = roundDuration;
        
        float progressToMax = (float)(currentRound - 1) / (Mathf.Max(1, roundToMaxSpawnRate - 1));
        currentSpawnRate = Mathf.Lerp(baseSpawnRate, maxSpawnRate, progressToMax);
        spawnTimer = currentSpawnRate;

        UpdateEnemyStats(currentRound);

        isRoundActive = true;
    }

    private void EndRound()
    {
        isRoundActive = false;
        currentRound++;
        
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }
        
        upgradeManager.RollRandomUpgrades();
    }

    private void SpawnEnemy()
    {
        List<SpawnPoint> validSpawners = new List<SpawnPoint>();
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.isSafeToSpawn)
            {
                validSpawners.Add(sp);
            }
        }

        if (validSpawners.Count == 0) return;

        SpawnPoint chosenSpawner = validSpawners[Random.Range(0, validSpawners.Count)];
        
        GameObject newEnemy = Instantiate(enemyPrefab, chosenSpawner.transform.position, Quaternion.identity);
        
        // -> PASS STATS TO ENEMY SCRIPT HERE <-
        // Example: newEnemy.GetComponent<EnemyScript>().ApplyStats(currentEnemyHealth, currentEnemyDamage);
    }

    private void UpdateEnemyStats(int round)
    {
        EnemyDifficultyManager.Instance.CalculateMultipliers(round);
        Debug.Log("Round " + round + " started. Health Multiplier: " + EnemyDifficultyManager.Instance.healthMultiplier);
    }

    private void UpdateClockUI()
    {
        if (clockText != null)
        {
            int minutes = Mathf.FloorToInt(roundTimer / 60F);
            int seconds = Mathf.FloorToInt(roundTimer - minutes * 60);
            clockText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}