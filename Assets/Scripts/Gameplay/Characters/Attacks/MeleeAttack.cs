using System;
using UnityEngine;

[System.Serializable]
public class MeleeAttack
{
    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    public float attackRange = 3f;
    public float damage = 1f;

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
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            float normalizedTime = timer / duration;
            float dashSpeedFactor = dashSpeedCurve.Evaluate(normalizedTime);
            float frameSpeed = (dist / duration) * dashSpeedFactor;

            Vector3 dashVelocity = dir * frameSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        onDashComplete?.Invoke();
    }
}
