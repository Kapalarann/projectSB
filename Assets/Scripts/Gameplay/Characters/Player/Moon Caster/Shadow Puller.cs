using UnityEngine;

public class ShadowPuller : MonoBehaviour
{
    public Transform pullTarget;
    public Debuff detonateDebuff;

    [Header("Attack")]
    public float range = 5f;
    public float coneAngle = 45f;
    public float baseDamage;
    public float detonateDamage;

    [Header("Effect")]
    public float pullStrength = 2f;
    public float pullDuration = 0.5f;

    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    public void OnAttack()
    {
        TriggerShadowPull();
    }

    public void TriggerShadowPull()
    {
        if (pullTarget == null) return;

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

            DebuffManager deb = enemy.GetComponent<DebuffManager>();
            if (deb != null)
            {
                if (deb.GetStacks("Moon") > 0)
                {
                    damage += detonateDamage;
                    deb.RemoveDebuff("Moon");
                }
            }

            Health hp = enemy.GetComponent<Health>();
            hp.TakeDamage(damage, transform.position);

            ShadowPull pullEffect = enemy.GetComponentInChildren<ShadowPull>();
            if (pullEffect != null)
            {
                pullEffect.PullTowards(pullTarget.position, pullStrength, pullDuration);
            }
        }
    }
}
