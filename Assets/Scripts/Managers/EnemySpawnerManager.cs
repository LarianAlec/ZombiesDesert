using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 5f; 
    public int enemiesPerSpawn = 1; 
    public Transform[] spawnPoints; 

    [Header("Spawn Area")]
    public Vector3 spawnArea = new Vector3(10, 0, 10); 

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemy();
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Enemy prefabs or spawn points are not set!");
            return;
        }

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        Vector3 randomPosition = spawnPoint.position + new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );
        
        Instantiate(enemyPrefab, randomPosition, spawnPoint.rotation);
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
