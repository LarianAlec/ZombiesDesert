using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private EnemyMelee enemy;

    public IdleState_Melee(Enemy cachedBaseCharacter, EnemyStateMachine stateMachine, string animBoolName) : base(cachedBaseCharacter, stateMachine, animBoolName)
    {
        enemy = cachedBaseCharacter as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}


