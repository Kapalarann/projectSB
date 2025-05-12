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

        float distance = Vector3.Distance(enemy.transform.position, enemy._target.transform.position);

        if (enemy.isRanged &&
            distance <= enemy.rangedAttack._maxRange &&
            distance >= enemy.rangedAttack._minRange &&
            enemy._attackTime >= enemy.rangedAttack._attackCooldown)
        {
            enemy.SwitchState(enemy.attackState);
            return;
        }

        if (enemy.isMelee &&
            distance <= enemy.meleeAttack.attackRange &&
            enemy._attackTime >= enemy.meleeAttack.attackCooldown)
        {
            enemy.SwitchState(enemy.attackState);
            return;
        }

        Vector3 boidDirection = enemy.GetBoidDirection();
        enemy.Move(boidDirection);
    }

}
