using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Wave/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    public GameObject enemyPrefab;
    public int enemyCost;
}

