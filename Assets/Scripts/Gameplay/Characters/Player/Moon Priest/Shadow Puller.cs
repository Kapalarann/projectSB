using System.Collections;
using UnityEngine;

public class ShadowPuller : Ability
{
    [Header("Attack")]
    public float range = 5f;
    public float coneAngle = 45f;
    public float baseDamage;
    public float detonateDamage;
    private float baseDamageBase;
    private float detonateDamageBase;
    private Coroutine damageBuffCoroutine;

    [Header("Effect")]
    public float pullStrength = 2f;
    public float pullDuration = 0.5f;

    [Header("Reference")]
    [SerializeField] public Debuff debuffToDetonate;
    public Transform pullTarget;

    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        baseDamageBase = baseDamage;
        detonateDamageBase = detonateDamage;
    }

    public override void Update()
    {
        base.Update();
    }

    public void OnPrimary()
    {
        TriggerShadowPull();
    }

    public void TriggerShadowPull()
    {
        if (pullTarget == null || _animator.GetBool("isStunned") || cooldownTimer > 0f) return;

        Vector3 moveDir = movement.movementValue;
        if (moveDir == Vector3.zero) moveDir = Vector3.right * movement.flipScale;

        foreach (var enemy in EnemyStateManager.Enemies)
        {
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;

            // Check range
            if (Vector3.Distance(enemy.transform.position, transform.position) > range) continue;

            // Check cone angle
            float angle = Vector3.Angle(moveDir, toEnemy);
            if (angle > coneAngle) continue;

            float damage = baseDamage;
            float pullMult = 1f;

            DebuffManager deb = enemy.GetComponent<DebuffManager>();
            if (deb != null)
            {
                if (deb.GetStacks("Moon") > 0)
                {
                    damage += detonateDamage;
                    pullMult = 3f;
                    deb.ApplyDebuff(debuffToDetonate, -1, gameObject);
                }
            }

            Health hp = enemy.GetComponent<Health>();
            hp.TakeDamage(damage, transform.position);
            GenerateEnergy();

            ShadowPull pullEffect = enemy.GetComponentInChildren<ShadowPull>();
            if (pullEffect != null)
            {
                pullEffect.PullTowards(pullTarget.position, pullStrength * pullMult, pullDuration);
            }
        }

        ResetCooldown();
    }

    public void IncreaseDamage(float baseBonus, float baseMultiplier, float detonateBonus, float detonateMultiplier, float duration)
    {
        // If there's already a buff active, cancel it first
        if (damageBuffCoroutine != null)
        {
            StopCoroutine(damageBuffCoroutine);
            ResetDamage();
        }

        baseDamage = baseDamageBase * baseMultiplier + baseBonus;
        detonateDamage = detonateDamageBase * detonateMultiplier + detonateBonus;

        damageBuffCoroutine = StartCoroutine(DamageBuffTimer(duration));
    }

    private IEnumerator DamageBuffTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetDamage();
        damageBuffCoroutine = null;
    }

    private void ResetDamage()
    {
        baseDamage = baseDamageBase;
        detonateDamage = detonateDamageBase;
    }
}
