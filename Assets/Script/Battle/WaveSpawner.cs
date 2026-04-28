using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public static int enemyPerWave = 1;

    [System.Serializable]
    public class WaveSettings
    {
        public GameObject[] enemyTypes;
        public int initialEnemyCount = 2;
        public float spawnDelay = 0.5f;
    }

    [Header("Configuration")]
    public WaveSettings[] wavePatterns;
    public float delayBetweenWaves = 10f;

    [Header("Spawns & Bases")]
    public Transform[] enemySpawnPoints;
    public GameObject playerBase;
    public GameObject enemyBase;

    bool PlayerBaseAlive => playerBase != null;
    bool EnemyBaseAlive => enemyBase != null;

    void Start()
    {
        if (wavePatterns == null || wavePatterns.Length == 0 || enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogError("Invalid spawner configuration.");
            return;
        }

        // Set the enemy count for all waves to global static value (optional)
        foreach (var config in wavePatterns)
        {
            config.initialEnemyCount = enemyPerWave;
        }

        StartCoroutine(WaveSpawnerLoop());
    }

    IEnumerator WaveSpawnerLoop()
    {
        int waveIndex = 0;
        while (PlayerBaseAlive && EnemyBaseAlive)
        {
            yield return SpawnStaticWave(waveIndex++);
            yield return new WaitForSeconds(delayBetweenWaves);
        }
        Debug.Log("Spawning stopped: Base destroyed.");
    }

    IEnumerator SpawnStaticWave(int waveIndex)
    {
        var config = wavePatterns[waveIndex % wavePatterns.Length];

        if (config.enemyTypes == null || config.enemyTypes.Length == 0)
        {
            Debug.LogWarning("Wave skipped: No enemy types defined.");
            yield break;
        }

        int count = config.initialEnemyCount;
        float delay = config.spawnDelay;

        Debug.Log($"Wave {waveIndex + 1}: Spawning {count} enemies with {delay:F2}s delay");

        for (int i = 0; i < count; i++)
        {
            if (!PlayerBaseAlive || !EnemyBaseAlive)
                yield break;

            var enemyPrefab = config.enemyTypes[Random.Range(0, config.enemyTypes.Length)];
            var spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            yield return new WaitForSeconds(delay);
        }
    }
}
