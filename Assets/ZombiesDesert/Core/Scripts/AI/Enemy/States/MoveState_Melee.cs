using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 destination;

    public MoveState_Melee(Enemy cachedBaseCharacter, EnemyStateMachine stateMachine, string animBoolName) : base(cachedBaseCharacter, stateMachine, animBoolName)
    {
        enemy = cachedBaseCharacter as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.agent.remainingDistance <= 0.5)
            stateMachine.ChangeState(enemy.idleState);
    }
}
