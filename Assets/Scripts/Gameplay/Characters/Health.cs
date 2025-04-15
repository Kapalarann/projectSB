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
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;

    [Header("Damage Effects")]
    [SerializeField] float damageNumberYOffset = 1.5f;

    [Header("Stamina")]
    [SerializeField] public float maxSP;
    [SerializeField] public float SP;
    [SerializeField] public float stunDuration = 1f;
    [SerializeField] public float stunResMultiplier;
    [SerializeField] public float stunResPerPlayer;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;
    [SerializeField] BloodEffect bloodEffects;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;
    private StaminaBarManager staminaBar;
    private GlobalDamageNumberPool damageNumberPool;
    private Audio _audio;
    private Block block;
    [System.NonSerialized] public bool isStunned = false;
    private void Awake()
    {
        HP = maxHP;
        SP = maxSP;

        healthBar = FindFirstObjectByType<HealthBarManager>();
        staminaBar = FindFirstObjectByType<StaminaBarManager>();
        damageNumberPool = FindFirstObjectByType<GlobalDamageNumberPool>();
        _audio = GetComponent<Audio>();
        block = GetComponent<Block>();

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
        if (block != null)
        {
            bool isPerfect;
            float blockVal;
            if (block.TryBlock(damage, attackerPos, out isPerfect, out blockVal))
            {
                float staminaDamage = isPerfect ? 0 : damage * (1 - blockVal);
                float intensity = isPerfect ? 1f : 0.5f;

                if(isPerfect) spriteEffects.FlashWhite(0.1f);

                HitAndShake(intensity);
                ApplyStaminaDamage(staminaDamage);
                return;
            }
        }

        if (_audio != null) _audio.PlayHurtSound(); //hurt sfx

        if(spriteEffects != null) spriteEffects.FlashWhite(0.1f);
        if (bloodEffects != null) bloodEffects.PlayEffect(attackerPos); //blood particle fx
        if (isPlayer) HitAndShake(1f);
        else HitAndShake(0.5f);

        ShowDamageNumber(gameObject.transform.position, damage);
        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        if (HP <= 0)
        {
            Die();
            return;
        }
        healthBar.UpdateHealth(transform, HP, maxHP);

        ApplyStaminaDamage(damage);
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

        damageNumber.GetComponent<TextMeshProUGUI>().fontSize = damageNumberPool.fontSize * (damageNumberPool.fontSizeMult * (1 + Mathf.Floor(damage * damageNumberPool.globalDamageDisplayMult / damageNumberPool.fontSizeChangeThreshold)));
        damageNumber.GetComponent<DamageNumber>().Initialize((damage * damageNumberPool.globalDamageDisplayMult).ToString(), Color.white);

        StartCoroutine(ReturnAfterDuration(damageNumber));
    }

    private IEnumerator ReturnAfterDuration(GameObject damageNumber)
    {
        yield return new WaitForSeconds(damageNumber.GetComponent<DamageNumber>().lifetime);
        damageNumberPool.ReturnDamageNumber(damageNumber);
    }

    private void HitAndShake(float intensity)
    {
        HitStop.Instance.DoHitStop(0.2f * intensity);
        ScreenShake.Instance.Shake(0.1f , 0.1f * intensity);
    }

    private void ApplyStaminaDamage(float amount)
    {
        if (isStunned) return;

        SP -= amount;
        SP = Mathf.Clamp(SP, 0, maxSP);

        if (SP <= 0) Stun();

        staminaBar.UpdateStamina(transform, SP, maxSP);
    }

    void Stun()
    {
        if (_audio != null) _audio.PlayStunSound(); //stun sfx
        HitStop.Instance.DoHitStop(0.2f);
        ScreenShake.Instance.Shake(0.1f, 0.05f);
        spriteEffects.FlashWhite(stunDuration / 10);
        isStunned = true;

        if (block != null) block.CancelBlock();

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
