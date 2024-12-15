using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDamageNumberPool : MonoBehaviour
{
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private int initialPoolSize = 30;
    [SerializeField] public float globalDamageDisplayMult = 1f;
    [SerializeField] public float fontSizeChangeThreshold = 5f; //increment of damage to increase damage number size
    [SerializeField] public float fontSize = 0.5f;
    [SerializeField] public float fontSizeMult = 1.1f; //percentage increase of size per tens value

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform);
            damageNumber.SetActive(false);
            pool.Enqueue(damageNumber);
        }
    }

    // Get a damage number from the pool
    public GameObject GetDamageNumber()
    {
        if (pool.Count > 0)
        {
            GameObject damageNumber = pool.Dequeue();
            damageNumber.SetActive(true);
            return damageNumber;
        }
        else
        {
            // Expand the pool if needed
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform);
            return damageNumber;
        }
    }

    // Return a damage number to the pool
    public void ReturnDamageNumber(GameObject damageNumber)
    {
        damageNumber.SetActive(false);
        pool.Enqueue(damageNumber);
    }
}
