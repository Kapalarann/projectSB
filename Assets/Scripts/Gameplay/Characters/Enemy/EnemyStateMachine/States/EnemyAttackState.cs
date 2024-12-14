using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy._animator.SetTrigger("onAttack");
        enemy._attackTime -= enemy._attackCooldown;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        
    }
}
