using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    public IdleState_Melee idleState {  get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, Constants.idleBoolName);
        moveState = new MoveState_Melee(this, stateMachine, Constants.moveBoolName);
        recoveryState = new RecoveryState_Melee(this, stateMachine, Constants.recoveryBoolName);
        chaseState = new ChaseState_Melee(this, stateMachine, Constants.chaseBoolName);
        attackState = new AttackState_Melee(this, stateMachine, Constants.attackBoolName);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

    }
}
