using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private EnemyMelee enemy;
    public RecoveryState_Melee(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName) : base(baseEnemy, stateMachine, animBoolName)
    {
        enemy = baseEnemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.SetFocus(enemy.player.position);

        if (shouldChangeState)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
