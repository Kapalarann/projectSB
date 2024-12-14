using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyApproachState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        if(!enemy.SearchTarget()) enemy.SwitchState(enemy.idleState);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        if(enemy._target == null) enemy.SwitchState(enemy.idleState);
        float d = Vector3.Distance(enemy.transform.position, enemy._target.transform.position);

        if (d <= enemy._maxRange && d>= enemy._minRange && enemy._attackTime >= enemy._attackCooldown) { 
            enemy.SwitchState(enemy.attackState);
        }

        Vector3 targetPosition = enemy._target.transform.position; //target's position
        Vector3 targetLocation = targetPosition - (targetPosition - enemy.transform.position).normalized * enemy._minRange; //target location to go
        targetLocation.y = enemy.transform.position.y;
        Vector3 directionToTarget = targetLocation - enemy.transform.position;

        enemy.Move(directionToTarget.normalized);
    }
}
