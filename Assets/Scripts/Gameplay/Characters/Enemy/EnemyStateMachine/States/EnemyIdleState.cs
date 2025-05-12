using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float timer = 0f;
    public override void EnterState(EnemyStateManager enemy)
    {
        timer = 0f;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        timer += Time.deltaTime;
        if (timer >= enemy._idleDuration)
        {
            enemy.SwitchState(enemy.approachState);
        }
    }
}
