using UnityEngine;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;

    protected string animBoolName;
    protected float stateTimer;

    public EnemyState(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = baseEnemy;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() 
    {
       enemyBase.animator.SetBool(animBoolName, true);
    }

    public virtual void Update() 
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit() 
    {
        enemyBase.animator.SetBool(animBoolName, false);
    }
}
