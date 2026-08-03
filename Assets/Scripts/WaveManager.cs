using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int enemyCount;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public EnemyGroup[] enemyGroups;
        public float spawnRate;
    }

    public Wave[] waves;
    
    [Header("Spawn Area")]
    public BoxCollider2D spawnArea;

    [Header("Warning Settings")]
    public GameObject warningCirclePrefab;
    public float warningDuration = 1f;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        if (spawnArea == null)
        {
            Debug.LogError("Please assign a BoxCollider2D for the spawn area!");
            return;
        }

        StartCoroutine(StartWave());
    }

    void Update()
    {
        if (!isSpawning && currentWaveIndex < waves.Length)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                currentWaveIndex++;
                
                if (currentWaveIndex < waves.Length)
                {
                    StartCoroutine(StartWave());
                }
                else
                {
                    Debug.Log("Congratulations! You have cleared all waves.");
                }
            }
        }
    }

    IEnumerator StartWave()
    {
        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex];
        Debug.Log("Started: " + currentWave.waveName);

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        
        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.enemyCount; i++)
            {
                enemiesToSpawn.Add(group.enemyPrefab);
            }
        }

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject temp = enemiesToSpawn[i];
            int randomIndex = Random.Range(i, enemiesToSpawn.Count);
            enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
            enemiesToSpawn[randomIndex] = temp;
        }

        foreach (GameObject enemyPrefab in enemiesToSpawn)
        {
            Vector2 randomSpawnPos = GetRandomPointInBox(spawnArea);
            StartCoroutine(SpawnWarningAndEnemy(enemyPrefab, randomSpawnPos));
            yield return new WaitForSeconds(currentWave.spawnRate);
        }

        isSpawning = false;
    }

    Vector2 GetRandomPointInBox(BoxCollider2D box)
    {
        Bounds bounds = box.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(randomX, randomY);
    }

    IEnumerator SpawnWarningAndEnemy(GameObject enemyPrefab, Vector2 position)
    {
        GameObject warningObj = null;

        if (warningCirclePrefab != null)
        {
            warningObj = Instantiate(warningCirclePrefab, position, Quaternion.identity);
        }

        yield return new WaitForSeconds(warningDuration);

        if (warningObj != null)
        {
            Destroy(warningObj);
        }

        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}