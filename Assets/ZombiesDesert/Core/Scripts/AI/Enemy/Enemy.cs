using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [Header("Idle data")]
    public float idleTime;
    public float aggroRange = 5.0f;

    [Header("Move data")]
    public float walkSpeed;
    public float chaseSpeed;
    public float turnRate;
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;
    private bool isManualMovementEnabled;
    private bool isManualRotationEnabled;

    public Transform player { get; private set; }
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public void SetManualMovement(bool manualMovement) => isManualMovementEnabled = manualMovement;
    public void SetManualRotation(bool manualRotation) => isManualRotationEnabled = manualRotation;

    public void ExitCurrentAIStateViaAnimEvent() => stateMachine.currentState.ExitCurrentAIStateThrouAnimEvent();

    public bool IsManualMovementEnabled() => isManualMovementEnabled;
    public bool IsManualRotationEnabled() => isManualRotationEnabled;
    public bool IsPlayerInAggroRange() => Vector3.Distance(transform.position, player.position) < aggroRange;


    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].position;
        
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    public Quaternion SetFocus(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnRate * Time.deltaTime);
        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
