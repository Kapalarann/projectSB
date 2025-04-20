using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] bool isPlayer;

    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    private bool isDead = false;

    [Header("Damage Effects")]
    [SerializeField] float damageNumberYOffset = 1.5f;

    [Header("Stamina")]
    [SerializeField] public float maxSP;
    [SerializeField] public float SP;
    [SerializeField] public float staminaRegen;
    [SerializeField] public float stamineDelay;

    [Header("Stun")]
    [SerializeField] public float stunDuration = 1f;
    [SerializeField] public float stunResMultiplier;
    [SerializeField] public float stunResPerPlayer;
    private float lastStaminaUseTime;
    private bool isRegeneratingStamina = false;
    private bool stunEnded = false;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;
    [SerializeField] BloodEffect bloodEffects;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;
    private StaminaBarManager staminaBar;
    private GlobalDamageNumberPool damageNumberPool;
    private Knockback knockback;
    private Audio _audio;
    private Block block;
    [System.NonSerialized] public bool isStunned = false;
    private void Awake()
    {
        HP = maxHP;
        SP = maxSP;
        lastStaminaUseTime = Time.time;

        healthBar = FindFirstObjectByType<HealthBarManager>();
        staminaBar = FindFirstObjectByType<StaminaBarManager>();
        damageNumberPool = FindFirstObjectByType<GlobalDamageNumberPool>();
        knockback = GetComponent<Knockback>();
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

                if(knockback != null) knockback.ApplyKnockback(attackerPos, damage * 3f);
                HitAndShake(intensity);
                ApplyStaminaDamage(staminaDamage);
                return;
            }
        }

        if (_audio != null) _audio.PlayHurtSound(); //hurt sfx

        if(spriteEffects != null) spriteEffects.FlashWhite(0.1f);
        if (bloodEffects != null) bloodEffects.PlayEffect(attackerPos); //blood particle fx
        if (isPlayer) HitAndShake(1f);

        ShowDamageNumber(gameObject.transform.position, damage);

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
        healthBar.UpdateHealth(transform, HP, maxHP);

        if (HP <= 0)
        {
            isDead = true;
        }

        if (knockback != null) 
        {
            float angle = isStunned ? GlobalValues.instance.knockbackAngle : 0f;
            knockback.ApplyKnockback(attackerPos, damage * 3f, angle); 
        }
        ApplyStaminaDamage(damage);

        if (isStunned && !isPlayer) ComboCounterUI.instance.IncreaseCombo();
        if (isPlayer) ComboCounterUI.instance.ResetCombo();
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
        if (isStunned)
        {
            AirCollider();
            return;
        }

        SP -= amount;
        SP = Mathf.Clamp(SP, 0, maxSP);

        if (SP <= 0) Stun();

        staminaBar.UpdateStamina(transform, SP, maxSP);

        lastStaminaUseTime = Time.time;
        isRegeneratingStamina = false;
    }

    private void RegenerateStamina()
    {
        if (!isStunned && Time.time - lastStaminaUseTime >= stamineDelay)
        {
            if (SP < maxSP)
            {
                SP += staminaRegen * Time.fixedDeltaTime;
                SP = Mathf.Clamp(SP, 0, maxSP);
                staminaBar.UpdateStamina(transform, SP, maxSP);
                isRegeneratingStamina = true;
            }
        }
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
        stunEnded = true;
    }

    private void EndStun()
    {
        GroundCollider();
        stunEnded = false;
        isStunned = false;
        _animator.SetBool("isStunned", false);
        SP = maxSP;
        staminaBar.UpdateStamina(transform, SP, maxSP);
    }

    private void FixedUpdate()
    {
        if (GroundChecker.IsGrounded(gameObject))
        {
            if (stunEnded) EndStun();

            if (isDead) Die();
        }

        RegenerateStamina();
    }

    private void AirCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void GroundCollider()
    {
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
    }

    void Die()
    {
        healthBar.RemoveHealthBar(transform);
        staminaBar.RemoveStaminaBar(transform);
        Destroy(gameObject);
    }
}
