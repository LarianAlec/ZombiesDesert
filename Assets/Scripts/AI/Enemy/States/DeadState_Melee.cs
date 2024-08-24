using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private RagdollComponent ragdoll;

    public DeadState_Melee(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName) : base(baseEnemy, stateMachine, animBoolName)
    {
        enemy = baseEnemy as EnemyMelee;
        ragdoll = enemy.GetComponent<RagdollComponent>();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.animator.enabled = false;
        enemy.agent.isStopped = true;

        ragdoll.EnableRagdoll();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
    }
}
