using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] List<EnemyData> _enemyList;
    [SerializeField] int _budget;
    [SerializeField] float _spawnInterval = 3.0f;
    [SerializeField] Transform[] _spawnPoints;

    float _spawnTimer = 0f;
    private void Update()
    {
        if (_spawnTimer > 0) _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && _budget > 0)
        {
            SpawnEnemy();
            _spawnTimer += _spawnInterval;
        }
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

    [System.Serializable]
    public class EnemyData
    {
        public GameObject enemyPrefab;
        public int enemyCost;
    }
}
