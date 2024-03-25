using UnityEngine;
using UnityEngine.AI;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;

    protected string animBoolName;
    protected float stateTimer;

    protected bool triggerCalled;

    public EnemyState(Enemy baseEnemy, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = baseEnemy;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() 
    {
        enemyBase.animator.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update() 
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit() 
    {
        enemyBase.animator.SetBool(animBoolName, false);
    }

    public void AnimationTrigger() => triggerCalled = true;

    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
            return agent.destination;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
                return path.corners[i + 1];
        }

        return agent.destination;
    }
}
