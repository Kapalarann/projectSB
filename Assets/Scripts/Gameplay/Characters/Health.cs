using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHP;
    [SerializeField] float HP;

    [Header("Damage Number")]
    [SerializeField] float damageNumberYOffset = 1.5f;

    [Header("Stamina")]
    [SerializeField] float maxSP;
    [SerializeField] float SP;
    [SerializeField] float stunResMultiplier;
    [SerializeField] float stunResPerPlayer;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;
    private GlobalDamageNumberPool damageNumberPool;

    private void Awake()
    {
        HP = maxHP;

        healthBar = FindObjectOfType<HealthBarManager>();
        damageNumberPool = FindObjectOfType<GlobalDamageNumberPool>();

        healthBar.AddHealthBar(transform);
        healthBar.UpdateHealth(transform, HP, maxHP);
    }
    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        if(gameObject.GetComponent<Audio>() != null) gameObject.GetComponent<Audio>().PlayHurtSound(); //hurt sfx
        if (gameObject.GetComponent<BloodEffect>() != null) gameObject.GetComponent<BloodEffect>().PlayEffect(attackerPos); //blood particle fx

        ShowDamageNumber(gameObject.transform.position, damage);
        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        healthBar.UpdateHealth(transform, HP, maxHP);
        if (HP <= 0) Die();
    }

    public void ShowDamageNumber(Vector3 worldPosition, float damage)
    {
        GameObject damageNumber = damageNumberPool.GetDamageNumber();

        // Set its position and initialize it
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition + Vector3.up * damageNumberYOffset);

        Vector2 localPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(damageNumberPool.gameObject.GetComponent<RectTransform>(), screenPosition, Camera.main, out localPosition))
        {
            damageNumber.GetComponent<RectTransform>().localPosition = localPosition;
        }

        damageNumber.GetComponent<TextMeshProUGUI>().fontSize = damageNumberPool.fontSize * (damageNumberPool.fontSizeMult * (1 + Mathf.Floor(damage * damageNumberPool.globalDamageDisplayMult / damageNumberPool.fontSizeChangeThreshold) ) );
        damageNumber.GetComponent<DamageNumber>().Initialize((damage * damageNumberPool.globalDamageDisplayMult).ToString(), Color.white);

        StartCoroutine(ReturnAfterDuration(damageNumber));
    }

    private IEnumerator ReturnAfterDuration(GameObject damageNumber)
    {
        yield return new WaitForSeconds(damageNumber.GetComponent<DamageNumber>().lifetime);
        damageNumberPool.ReturnDamageNumber(damageNumber);
    }

    void Die()
    {
        healthBar.RemoveHealthBar(transform);
        Destroy(gameObject);
    }
}
