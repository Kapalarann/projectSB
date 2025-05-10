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

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;
    [SerializeField] BloodEffect bloodEffects;

    [System.NonSerialized] public bool isInvulnerable = false;
    private HealthBarManager healthBar;
    private GlobalDamageNumberPool damageNumberPool;
    private Knockback knockback;
    private Audio _audio;
    private Stamina stamina;
    private Energy energy;
    private Block block;

    private void Awake()
    {
        HP = maxHP;

        healthBar = FindFirstObjectByType<HealthBarManager>();
        damageNumberPool = FindFirstObjectByType<GlobalDamageNumberPool>();
        knockback = GetComponent<Knockback>();
        _audio = GetComponent<Audio>();
        stamina = GetComponent<Stamina>();
        energy = GetComponent<Energy>();
        block = GetComponent<Block>();

        if (healthBar != null)
        {
            healthBar.AddHealthBar(transform);
            healthBar.UpdateHealth(transform, HP, maxHP);
        }
    }

    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        if (_audio != null) _audio.PlayHurtSound();

        if (spriteEffects != null) spriteEffects.FlashWhite(0.1f);
        if (bloodEffects != null) bloodEffects.PlayEffect(attackerPos);
        if (isPlayer) HitAndShake(1f);

        ShowDamageNumber(gameObject.transform.position, damage);

        if (stamina != null)
        {
            stamina.ApplyStaminaDamage(damage);
        }

        if (knockback != null)
        {
            float angle = 0f;
            if (stamina != null) angle = stamina.isStunned ? GlobalValues.instance.knockbackAngle : 0f;
            knockback.ApplyKnockback(attackerPos, damage * 3f, angle);
        }

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
        if (healthBar != null) healthBar.UpdateHealth(transform, HP, maxHP);

        if (HP <= 0)
        {
            isDead = true;
        }

        if (!isPlayer) ComboCounterUI.instance.IncreaseCombo();
        if (isPlayer) ComboCounterUI.instance.ResetCombo();
    }

    public void TakeDamage(float damage, bool isHP)
    {
        if (isHP)
        {
            HP -= damage;
            HP = Mathf.Clamp(HP, 0, maxHP);
            if (healthBar != null) healthBar.UpdateHealth(transform, HP, maxHP);

            if (HP <= 0)
            {
                isDead = true;
            }
        }
    }

    public void TryBlock(float damage, Vector3 attackerPos, out bool isReflected)
    {
        isReflected = false;
        if (block == null) return;

        bool isPerfect;
        float blockVal;
        if (block.TryBlock(damage, attackerPos, out isPerfect, out blockVal))
        {
            float staminaDamage = isPerfect ? 0 : damage * (1 - blockVal);
            float intensity = isPerfect ? 1f : 0.5f;

            if (isPerfect)
            {
                spriteEffects.FlashWhite(0.1f);
                isReflected = true;
            }

            if (knockback != null) knockback.ApplyKnockback(attackerPos, damage * 3f);
            HitAndShake(intensity);
            if (stamina != null) stamina.ApplyStaminaDamage(staminaDamage);

            bool enoughEP = true;
            if (energy != null)
            {
                if (energy.EP >= staminaDamage) energy.ChangeEnergy(-staminaDamage);
                else enoughEP = false;
            }
            if (enoughEP) return;
        }
    }

    public void ShowDamageNumber(Vector3 worldPosition, float damage)
    {
        if (damageNumberPool == null) return;
        GameObject damageNumber = damageNumberPool.GetDamageNumber();

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
        ScreenShake.Instance.Shake(0.1f, 0.1f * intensity);
    }

    private void FixedUpdate()
    {
        if (GroundChecker.IsGrounded(gameObject))
        {
            if (isDead) Die();
        }
    }

    public void RecoverHealth(float amount)
    {
        HP += amount;
        HP = Mathf.Clamp(HP, 0, maxHP);
        if (healthBar != null) healthBar.UpdateHealth(transform, HP, maxHP);
    }

    void Die()
    {
        healthBar.RemoveHealthBar(transform);
        Destroy(gameObject);
    }
}