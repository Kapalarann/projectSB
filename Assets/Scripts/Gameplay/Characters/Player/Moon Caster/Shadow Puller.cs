using UnityEngine;

public class ShadowPuller : MonoBehaviour
{
    public Transform pullTarget;

    public float pullStrength = 2f;
    public float pullDuration = 0.5f;

    public void OnAttack()
    {
        TriggerShadowPull();
    }

    public void TriggerShadowPull()
    {
        if (pullTarget == null) return;

        foreach (var enemy in EnemyStateManager.Enemies)
        {
            ShadowPull pullEffect = enemy.GetComponentInChildren<ShadowPull>();
            if (pullEffect != null)
            {
                pullEffect.PullTowards(pullTarget.position, pullStrength, pullDuration);
            }
        }
    }
}
