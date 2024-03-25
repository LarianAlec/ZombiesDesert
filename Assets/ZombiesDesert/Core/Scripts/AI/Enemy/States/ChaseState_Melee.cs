using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private float lastUpdatedDestinationTime;

    public ChaseState_Melee(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName) : base(baseEnemy, stateMachine, animBoolName)
    {
        enemy = baseEnemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        enemy.transform.rotation = enemy.SetFocus(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastUpdatedDestinationTime + 0.25f)
        {
            lastUpdatedDestinationTime = Time.time;
            return true;
        }

        return false;
    }
}
