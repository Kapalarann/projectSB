using UnityEngine;

[CreateAssetMenu(menuName = "Debuff System/Burn Debuff")]
public class BurnDebuffSO : Debuff
{
    public int damagePerTick = 1;

    public override void OnTick(GameObject target, int currentStacks)
    {
        if (target == null) return;
        var health = target.GetComponent<Health>();
        if (health == null) return;

        health.TakeDamage(damagePerTick * currentStacks * Time.deltaTime, true, false);
    }
}
