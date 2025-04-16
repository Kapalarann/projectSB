using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Wave/Wave Data", order = 2)]
public class WaveData : ScriptableObject
{
    [Header("Wave Settings")]
    public EnemyData[] enemyList; // List of enemies available in this wave
    public int budget;                // Total budget for this wave
    public float spawnInterval = 3.0f; // Time interval between spawns
    [Range(0f, 1f)]
    public float percentOfEnter = 0f;
}

