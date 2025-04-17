using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private Transform[] _spawnPoints;

    public int _currentWaveIndex = 0;
    private WaveData _currentWave;
    private EnemyData[] _enemyList;
    private int _budget;
    private float _spawnInterval;
    private float _spawnTimer = 0f;

    private int _enemiesSpawnedThisWave = 0;

    private void Start()
    {
        if (waves.Length > 0)
            InitializeWave(waves[_currentWaveIndex]);
    }

    private void Update()
    {
        if (_currentWave == null) return;

        // Handle enemy spawning for the current wave
        if (_spawnTimer > 0) _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && _budget > 0)
        {
            SpawnEnemy();
            _spawnTimer += _spawnInterval;
        }

        CheckForNextWave();
    }

    private void InitializeWave(WaveData waveData)
    {
        _currentWave = waveData;
        _enemyList = waveData.enemyList;
        _budget = waveData.budget;
        _spawnInterval = waveData.spawnInterval;
        _enemiesSpawnedThisWave = 0;
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < 10; i++) // avoid infinite loop
        {
            EnemyData selectedEnemy = _enemyList[Random.Range(0, _enemyList.Length)];

            if (_budget >= selectedEnemy.enemyCost)
            {
                Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

                Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);

                _budget -= selectedEnemy.enemyCost;
                _enemiesSpawnedThisWave++;
                return;
            }
        }
    }

    private void CheckForNextWave()
    {
        if (_currentWaveIndex + 1 >= waves.Length) return; // no more waves
        if (_budget > 0) return; // wait until all enemies are spawned

        int remainingEnemies = EnemyStateManager.Enemies.Count;

        if (_enemiesSpawnedThisWave == 0) return; // avoid divide by zero

        float remainingPercent = (float)remainingEnemies / _enemiesSpawnedThisWave;
        float threshold = waves[_currentWaveIndex+1].percentOfEnter;

        if (remainingPercent <= threshold)
        {
            _currentWaveIndex++;
            InitializeWave(waves[_currentWaveIndex]);
        }
    }
}
