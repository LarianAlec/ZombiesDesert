using JetBrains.Annotations;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
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
    public HealthController_Enemy health { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<HealthController_Enemy>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Update()
    {

    }

    public virtual void GetHit()
    {
        health.ReduceHealth();

        if (health.ShouldDie())
            Die();

        EnterBattleMode();
    }

    public virtual void GetHitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        if (health.ShouldDie())
            StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    public virtual void Die()
    {

    }

    public virtual void EnterBattleMode()
    {

    }

    public void SetManualMovement(bool manualMovement) => isManualMovementEnabled = manualMovement;
    public void SetManualRotation(bool manualRotation) => isManualRotationEnabled = manualRotation;

    public void ExitCurrentAIStateViaAnimEvent() => stateMachine.currentState.ExitCurrentAIStateThrouAnimEvent();

    public bool IsManualMovementEnabled() => isManualMovementEnabled;
    public bool IsManualRotationEnabled() => isManualRotationEnabled;
    public bool IsPlayerInAggroRange() => Vector3.Distance(transform.position, player.position) < aggroRange;

    private IEnumerator  HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force,hitPoint, ForceMode.Impulse);
    }

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

    private void InitializePatrolPoints()
    {
        if (patrolPoints.Length == 0)
            return;

        foreach (Transform point in patrolPoints)
        {
            point.parent = null;
        }
    }
}
