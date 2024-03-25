using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 attackDirection;

    private const float MAX_ATTACK_DISTANCE = 2.0f;

    public AttackState_Melee(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName) : base(baseEnemy, stateMachine, animBoolName)
    {
        enemy = baseEnemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsManualMovementEnabled())
        {
            enemy.transform.position = 
                Vector3.MoveTowards(enemy.transform.position, attackDirection, enemy.attackMoveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
