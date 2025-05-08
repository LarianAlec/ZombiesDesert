using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemySpawnerManager : MonoBehaviour
{
    // WaveType must be matching with WavesTimerWidget.WaveGoal for correct UI view
    public enum WaveType
    {
        Combat,
        Magazine
    };

    [System.Serializable]
    public class Wave
    {
        public WaveType waveType;
        public string waveName;
        public int enemiesCount;
        public GameObject[] enemyPrefabs;
        public float spawnDelay = 1f;
        public float waveDelay = 5f;
        [Tooltip("Additional delay for Magazine waves")]
        public float magazineWaveExtraDelay = 10f;
    }

    [Header("Wave Settings")] 
    public Wave[] waves;
    public Transform[] spawnPoints;
    public Vector3 spawnArea = new Vector3(10, 0, 10);

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine waveTimerCoroutine;

    public event Action<int> OnWaveStarted; // currentWaveIndex
    public event Action<float> OnWaveTimerChanged; // remaingTime
    public event Action<string> OnWaveNameChanged; // waveName
    public event Action<int> OnWaveTypeChanged; // waveType
    public event Action OnAllWavesCompleted;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed!");
            OnAllWavesCompleted?.Invoke();

            GameManager gm = GameManager.instance;
            gm.OnPreparePhaseStarted?.Invoke();
            return;
        }

        Wave currentWave = waves[currentWaveIndex];
        Debug.Log($"Starting {currentWave.waveType} Wave: {currentWave.waveName}");
        OnWaveStarted?.Invoke(currentWaveIndex);
        OnWaveNameChanged?.Invoke(currentWave.waveName);
        OnWaveTypeChanged?.Invoke((int)currentWave.waveType);

        StartCoroutine(SpawnWave(currentWave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        // Spawn enemies
        for (int i = 0; i < wave.enemiesCount; i++)
        {
            SpawnEnemy(wave.enemyPrefabs);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        isSpawning = false;
        currentWaveIndex++;
        
        // Wait until all enemies will be died
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        // Calling the appropriate event depending on the type of wave
        if (wave.waveType == WaveType.Magazine)
        {
            GameManager.instance.OnMagazinePhaseStarted?.Invoke();
        }
        else
        {
            GameManager.instance.OnPreparePhaseStarted?.Invoke();
        }

        // Calculating totalDelay for start next wave
        float totalDelay = wave.waveDelay;

        if (wave.waveType == WaveType.Magazine)
        {
            totalDelay += wave.magazineWaveExtraDelay;
            Debug.Log($"Magazine wave completed. Additional delay: {wave.magazineWaveExtraDelay}");
        }

        // Taimer for next wave
        if (currentWaveIndex < waves.Length)
        {
            // UI show wait next wave goal
            if (wave.waveType != WaveType.Magazine)
            {  
                UI_Manager.instance.ShowWaitNextWaveGoal();
            }
            
            waveTimerCoroutine = StartCoroutine(WaveTimerCountdown(totalDelay));
            yield return waveTimerCoroutine;
            StartNextWave();
        }
        else
        {
            OnAllWavesCompleted?.Invoke();
            GameManager.instance.OnPreparePhaseStarted?.Invoke();
        }
    }

    private IEnumerator WaveTimerCountdown(float delay)
    {
        float remainingTime = delay;

        while (remainingTime > 0)
        {
            OnWaveTimerChanged?.Invoke(remainingTime);
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        OnWaveTimerChanged?.Invoke(0f);
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

    public Wave GetCurrentWave()
    {
        return waves[currentWaveIndex];
    }
}
