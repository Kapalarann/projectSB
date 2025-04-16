using UnityEngine;
public class EnemyApproachState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        if (!enemy.SearchTarget()) enemy.SwitchState(enemy.idleState);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        if (enemy._target == null)
        {
            enemy.SwitchState(enemy.idleState);
            return;
        }

        float d = Vector3.Distance(enemy.transform.position, enemy._target.transform.position);

        // Attempt attack if in range and cooldown is ready
        if (d <= enemy.rangedAttack._maxRange &&
            d >= enemy.rangedAttack._minRange &&
            enemy._attackTime >= enemy.rangedAttack._attackCooldown)
        {
            enemy.SwitchState(enemy.attackState);
            return;
        }

        // Always apply movement, even if in range and attack is cooling down
        Vector3 boidDirection = enemy.GetBoidDirection();
        enemy.Move(boidDirection);
    }
}
