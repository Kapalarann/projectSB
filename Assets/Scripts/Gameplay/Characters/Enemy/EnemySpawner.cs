using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData currentWave;
    [SerializeField] private Transform[] _spawnPoints;

    private List<EnemyData> _enemyList;
    private int _budget;
    private float _spawnInterval;

    private float _spawnTimer = 0f;

    private void Start()
    {
        InitializeWave(currentWave);
    }
    private void Update()
    {
        if (_spawnTimer > 0) _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && _budget > 0)
        {
            SpawnEnemy();
            _spawnTimer += _spawnInterval;
        }
    }
    private void InitializeWave(WaveData waveData)
    {
        _enemyList = waveData.enemyList;
        _budget = waveData.budget;
        _spawnInterval = waveData.spawnInterval;
    }
    private void SpawnEnemy()
    {
        while(true)
        {
            EnemyData selectedEnemy = _enemyList[Random.Range(0, _enemyList.Count)];

            if (_budget >= selectedEnemy.enemyCost)
            {
                Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

                Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);

                _budget -= selectedEnemy.enemyCost;
                return;
            }
        }
    }
}
