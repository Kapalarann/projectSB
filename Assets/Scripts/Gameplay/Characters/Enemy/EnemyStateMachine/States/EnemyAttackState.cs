public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy._animator.SetTrigger("onAttack");
        enemy._attackTime -= enemy.rangedAttack._attackCooldown;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        
    }
}
