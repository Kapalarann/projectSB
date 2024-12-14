using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHP;
    [SerializeField] float HP;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;

    private void Awake()
    {
        HP = maxHP;

        healthBar = FindObjectOfType<Canvas>().GetComponent<HealthBarManager>();

        healthBar.AddHealthBar(transform);
        healthBar.UpdateHealth(transform, HP, maxHP);
    }
    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;
        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        healthBar.UpdateHealth(transform, HP, maxHP);
        if (HP <= 0) Die();
    }

    void Die()
    {
        healthBar.RemoveHealthBar(transform);
        Destroy(gameObject);
    }
}
