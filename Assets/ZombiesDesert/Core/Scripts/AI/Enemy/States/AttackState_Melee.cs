using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 2.0f;

    public AttackState_Melee(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName) : base(baseEnemy, stateMachine, animBoolName)
    {
        enemy = baseEnemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.animator.SetFloat(Constants.attackAnimationSpeedName, enemy.attackData.animationSpeed);
        enemy.animator.SetFloat(Constants.attackAnimationIndexName, enemy.attackData.attackIndex);
        enemy.animator.SetFloat(Constants.slashAttackAnimationIndexName, Random.Range(0,2)); // HardCode! Needs to refactoring!

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack();
    }

    private void SetupNextAttack()
    {
        if (IsPlayerClose())
        {
            enemy.animator.SetFloat(Constants.recoveryAnimationIndexName, Constants.quickRecoveryValue);
        }
        else
        {
            enemy.animator.SetFloat(Constants.recoveryAnimationIndexName, Constants.slowRecoveryValue);
        }

        enemy.attackData = GetRandomAttackData();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsManualRotationEnabled())
        {
            enemy.transform.rotation = enemy.SetFocus(enemy.player.position);
            attackDirection = enemy.transform.position + enemy.transform.forward * MAX_ATTACK_DISTANCE;
        }

        if (enemy.IsManualMovementEnabled())
        {
            enemy.transform.position = 
                Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
        }

        if (shouldChangeState)
        {
            stateMachine.ChangeState(enemy.recoveryState);
        }
    }

    private bool IsPlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.attackData.attackRange + 1.0f;

    private AttackData GetRandomAttackData()
    {
        List<AttackData> validAttacks = new List<AttackData>(enemy.attackList);

        if (IsPlayerClose())
        {
            // If enemy close to player validAttacks should contain attacks only witn AttackType_Melee.Close 
            validAttacks.RemoveAll(predicate => predicate.attackType == AttackType_Melee.Charge);
        }
        else
        {
            validAttacks.RemoveAll(predicate => predicate.attackType == AttackType_Melee.Close);
        }

        int randomIndex = Random.Range(0, validAttacks.Count);
        return validAttacks[randomIndex];
    }
}
