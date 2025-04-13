using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] bool isPlayer;

    [Header("Health")]
    [SerializeField] float maxHP;
    [SerializeField] float HP;

    [Header("Damage Effects")]
    [SerializeField] float damageNumberYOffset = 1.5f;

    [Header("Stamina")]
    [SerializeField] float maxSP;
    [SerializeField] float SP;
    [SerializeField] float stunDuration = 1f;
    [SerializeField] float stunResMultiplier;
    [SerializeField] float stunResPerPlayer;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;
    private StaminaBarManager staminaBar;
    private GlobalDamageNumberPool damageNumberPool;
    private Audio _audio;
    [System.NonSerialized] public bool isStunned = false;
    private void Awake()
    {
        HP = maxHP;
        SP = maxSP;

        healthBar = FindFirstObjectByType<HealthBarManager>();
        staminaBar = FindFirstObjectByType<StaminaBarManager>();
        damageNumberPool = FindFirstObjectByType<GlobalDamageNumberPool>();
        _audio = GetComponent<Audio>();

        healthBar.AddHealthBar(transform);
        healthBar.UpdateHealth(transform, HP, maxHP);

        staminaBar.AddStaminaBar(transform);
        staminaBar.UpdateStamina(transform, SP, maxSP);

        _receiver.StunEnd += StunEnd;
    }
    private void OnDestroy()
    {
        _receiver.StunEnd -= StunEnd;
    }
    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        if(_audio != null) _audio.PlayHurtSound(); //hurt sfx
        if(isPlayer)
        {
            HitStop.Instance.DoHitStop(0.2f);
            ScreenShake.Instance.Shake(0.1f, 0.1f);
        }
        if (gameObject.GetComponent<BloodEffect>() != null) gameObject.GetComponent<BloodEffect>().PlayEffect(attackerPos); //blood particle fx

        ShowDamageNumber(gameObject.transform.position, damage);
        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        if (HP <= 0) 
        { 
            Die();
            return;
        }
        healthBar.UpdateHealth(transform, HP, maxHP);

        if (isStunned) return;
        SP -= damage;
        SP = Mathf.Clamp(SP, 0, maxSP);

        if (SP <= 0) Stun();

        staminaBar.UpdateStamina(transform, SP, maxSP);
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

    void Stun()
    {
        if (_audio != null) _audio.PlayStunSound(); //stun sfx
        HitStop.Instance.DoHitStop(0.2f);
        ScreenShake.Instance.Shake(0.1f, 0.05f);
        spriteEffects.FlashWhite(stunDuration / 10);
        isStunned = true;
        _animator.SetBool("isStunned", true);
        _animator.SetFloat("stunDuration", 1/stunDuration);
    }

    public void StunEnd(AnimationEvent animationEvent)
    {
        isStunned = false;
        _animator.SetBool("isStunned", false);
        SP = maxSP;
        staminaBar.UpdateStamina(transform, SP, maxSP);
    }

    void Die()
    {
        healthBar.RemoveHealthBar(transform);
        staminaBar.RemoveStaminaBar(transform);
        Destroy(gameObject);
    }
}
