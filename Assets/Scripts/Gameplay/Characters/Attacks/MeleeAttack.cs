using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeleeAttack
{
    [Header("Attack Settings")]
    [SerializeField] public float attackCooldown = 1f;
    [SerializeField] public float idealRange = 2f;
    [SerializeField] public float attackRange = 3f;
    [SerializeField] public float attackRadius = 1f;
    [SerializeField] public float damage = 1f;
    [SerializeField] private Transform attackPoint;
    HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    [Header("Dash Settings")]
    public float dashDistance = 10f;
    public float dashDuration = 0.2f;
    [SerializeField] private AnimationCurve dashSpeedCurve;

    public void PerformMeleeAttack(MonoBehaviour owner, Rigidbody rb, Transform attacker, GameObject target, Action onDashComplete)
    {
        if (target == null || rb == null) return;

        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;

        owner.StartCoroutine(DashAttack(rb, direction, dashDistance, dashDuration, onDashComplete));
    }

    private System.Collections.IEnumerator DashAttack(Rigidbody rb, Vector3 dir, float dist, float duration, Action onDashComplete)
    {
        alreadyHit.Clear();
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            float normalizedTime = timer / duration;
            float dashSpeedFactor = dashSpeedCurve.Evaluate(normalizedTime);
            float frameSpeed = (dist / duration) * dashSpeedFactor;

            Vector3 dashVelocity = dir * frameSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);

            Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRadius);
            foreach (Collider hit in hitColliders)
            {
                if (hit.gameObject == rb.gameObject || alreadyHit.Contains(hit.gameObject)) continue;
                alreadyHit.Add(hit.gameObject);

                Health health = hit.GetComponent<Health>();
                if (health == null) continue;

                bool isParried = false;
                health.TryBlock(damage, rb.position, out isParried);
                if (isParried)
                {
                    rb.GetComponent<Stamina>().ApplyStaminaDamage(damage);
                }
                else
                {
                    health.TakeDamage(damage, rb.position);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        onDashComplete?.Invoke();
    }
}
