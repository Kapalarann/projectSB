using System.Collections;
using TMPro;
using UnityEngine;

public class Health : Interactable
{
    [SerializeField] bool isPlayer;

    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    [HideInInspector] public bool isDown = false;
    [HideInInspector] public bool isDead = false;

    [Header("Revive")]
    [SerializeField] public float maxRP;
    [SerializeField] public float RP = 0;
    [SerializeField] public float RPperSec;

    [Header("Damage Effects")]
    [SerializeField] float damageNumberYOffset = 1.5f;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;
    [SerializeField] BloodEffect bloodEffects;
    [SerializeField] SparkEffect sparkEffects;

    [System.NonSerialized] public bool isInvulnerable = false;
    private PlayerMovement playerMovement;
    private HealthBarManager healthBar;
    private GlobalDamageNumberPool damageNumberPool;
    private Knockback knockback;
    private Audio _audio;
    private Stamina stamina;
    private Energy energy;
    private Block block;

    private int defaultLayer;
    private int downedLayer;

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

        if(isPlayer) playerMovement = GetComponent<PlayerMovement>();

        defaultLayer = gameObject.layer;
        downedLayer = LayerMask.NameToLayer("Downed");

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

    public void TryBlock(float damage, Vector3 attackerPos, out bool isReflected, out float damageLeft)
    {
        isReflected = false;
        damageLeft = damage;
        if (block == null) return;

        bool isPerfect;
        float blockVal;
        if (block.TryBlock(damage, attackerPos, out isPerfect, out blockVal))
        {
            float staminaDamage = isPerfect ? 0 : damage * (1 - blockVal);
            float intensity = isPerfect ? 2f : 0.5f;

            if (isPerfect)
            {
                spriteEffects.FlashWhite(0.1f);
                if (sparkEffects != null) sparkEffects.PlayEffect(attackerPos); 
                isReflected = true;
                damageLeft = 0f;
            }

            if (knockback != null) knockback.ApplyKnockback(attackerPos, damage * 3f);
            HitAndShake(intensity);
            if (stamina != null) stamina.ApplyStaminaDamage(staminaDamage);

            bool enoughEP = true;
            if (energy != null)
            {
                if (energy.EP >= staminaDamage)
                {
                    energy.ChangeEnergy(-staminaDamage);
                    damageLeft = (1 - blockVal) * damage;
                }
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
            if (isDead)
            {
                if (isPlayer)
                {
                    if(!isDown) Downed();
                }
                else
                {
                    Die();
                }
            } 
        }

        if (isPlayer && isDown)
        {
            if(RP + Time.fixedDeltaTime * RPperSec < maxRP)
            {
                RP += Time.fixedDeltaTime * RPperSec;
                healthBar.UpdateHealth(transform, RP, maxRP);
            }

            if(RP >= maxRP) Revived();
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

    public void Downed()
    {
        isDown = true;
        RP = 0;

        playerMovement.ApplySlow(0.75f, -1);
        _animator.SetBool("isDowned", true);
        gameObject.layer = downedLayer;
        PlayerStatManager.instance.downedPlayers++;

        if (PlayerStatManager.instance.downedPlayers >= PlayerMovement.Players.Count)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    public void Revived()
    {
        isDown = false;
        isDead = false;
        HP = maxHP / 2;
        if (healthBar != null) healthBar.UpdateHealth(transform, HP, maxHP);

        playerMovement.RemovePermanentSlow();
        _animator.SetBool("isDowned", false);
        gameObject.layer = defaultLayer;
        PlayerStatManager.instance.downedPlayers--;
    }

    public override void Interact()
    {
        if (isDown)
        {
            RP += RPperSec;
        }
    }
}