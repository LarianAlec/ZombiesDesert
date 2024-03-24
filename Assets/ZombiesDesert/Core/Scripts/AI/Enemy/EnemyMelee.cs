using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    public IdleState_Melee idleState {  get; private set; }
    public MoveState_Melee moveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, Constants.idleBoolName);
        moveState = new MoveState_Melee(this, stateMachine, Constants.moveBoolName);
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
