using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1f, 3f)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee
{
    Close,
    Charge
}

public class EnemyMelee : Enemy
{
    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;
    [SerializeField] private LayerMask playerLayer;

    public IdleState_Melee idleState {  get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, Constants.idleBoolName);
        moveState = new MoveState_Melee(this, stateMachine, Constants.moveBoolName);
        recoveryState = new RecoveryState_Melee(this, stateMachine, Constants.recoveryBoolName);
        chaseState = new ChaseState_Melee(this, stateMachine, Constants.chaseBoolName);
        attackState = new AttackState_Melee(this, stateMachine, Constants.attackBoolName);
        deadState = new DeadState_Melee(this, stateMachine, Constants.idleBoolName); // In dead state used ragdoll, then idle anim like placeholder
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

    public bool IsPlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);

        // DEBUG
        Gizmos.DrawLine(transform.position + new Vector3(0, 1.0f, 0), transform.position + new Vector3(0, 1.0f, 0) + transform.forward * 2.0f);
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public override void EnterBattleMode()
    {
        base.EnterBattleMode();

        if (stateMachine.currentState == deadState)
            return;

        stateMachine.ChangeState(chaseState);
    }

    public void AttackCast()
    {
        Vector3 attackDirection = transform.position + transform.forward *attackData.attackRange;
        
        if (Physics.SphereCast(transform.position + new Vector3(0, 1.0f, 0), 0.4f, transform.forward, out var hitInfo, 2.0f, playerLayer))
        {
            Debug.Log("Attack " + hitInfo.collider.gameObject.name);

            IDamagable damagable = hitInfo.collider.gameObject.GetComponent<IDamagable>();
            damagable?.TakeDamage();
        }
    }

    
}
