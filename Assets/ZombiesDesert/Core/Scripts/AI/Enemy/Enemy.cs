using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].position;
        
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }
}
