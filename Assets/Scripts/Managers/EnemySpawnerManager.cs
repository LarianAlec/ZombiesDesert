using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemySpawnerManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int enemiesCount;
        public GameObject[] enemyPrefabs;
        public float spawnDelay = 1f;
        public float waveDelay = 5f;
    }

    [Header("Wave Settings")] 
    public Wave[] waves;
    public Transform[] spawnPoints;
    public Vector3 spawnArea = new Vector3(10, 0, 10);

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            //_victoryPresenter.ShowPopup();
            Debug.Log("All waves completed!");
            return;
        }

        Wave currentWave = waves[currentWaveIndex];
        Debug.Log($"Starting Wave: {currentWave.waveName}");

        StartCoroutine(SpawnWave(currentWave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemiesCount; i++)
        {
            SpawnEnemy(wave.enemyPrefabs);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        isSpawning = false;
        currentWaveIndex++;

        yield return new WaitUntil(() => activeEnemies.Count == 0);

        if (currentWaveIndex < waves.Length)
        {
            yield return new WaitForSeconds(wave.waveDelay);
            StartNextWave();
        }
        else
        {
            //_victoryPresenter.ShowPopup();
            GameManager gm = FindObjectOfType<GameManager>();
            gm.OnPreparePhaseStarted?.Invoke();
        }
    }

    private void SpawnEnemy(GameObject[] possibleEnemies)
    {
        if (possibleEnemies.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No enemies or spawn points set!");
            return;
        }

        GameObject enemyPrefab = possibleEnemies[Random.Range(0, possibleEnemies.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = spawnPoint.position + new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.OnDeath += () => RemoveEnemy(enemy);
        }
        else
        {
            Debug.LogWarning("Enemy prefab has no Enemy component!");
        }
    }
    
    private void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in spawnPoints)
        {
            Gizmos.DrawWireCube(spawnPoint.position, spawnArea);
        }
    }
}
