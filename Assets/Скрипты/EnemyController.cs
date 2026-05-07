using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Chasing, Searching, Attacking, Dead }

    [Header("Current Status")]
    public EnemyState CurrentState = EnemyState.Idle;

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Vision")]
    [SerializeField] private float viewRadius = 15f;
    [Range(0, 360)]
    [SerializeField] private float viewAngle = 360f;
    [SerializeField] private float eyeHeight = 1.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Movement")]
    [SerializeField] private float chaseDistance = 20f;
    [SerializeField] private float attackDistance = 2.2f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float faceTargetSpeed = 10f;

    [Header("Memory")]
    [SerializeField] private float searchTime = 3f;

    [Header("Combat Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private EnemyWeaponTrigger weaponTrigger;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 3f;

    private NavMeshAgent agent;
    private Animator anim;

    private bool isDead;
    private bool isAttacking;
    private bool isSearching;
    private float nextPathUpdateTime;
    private float nextAttackTime;
    private float searchTimer;
    private Vector3 lastKnownPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (weaponTrigger == null)
            weaponTrigger = GetComponentInChildren<EnemyWeaponTrigger>(true);
    }

    private void Start()
    {
        if (agent != null)
            agent.speed = patrolSpeed;

        FindTarget();
    }

    private void Update()
    {
        if (isDead) return;

        if (target == null)
            FindTarget();

        if (target == null)
        {
            StopMovement();
            CurrentState = EnemyState.Idle;
            UpdateAnimator();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            lastKnownPosition = target.position;
            searchTimer = searchTime;
            isSearching = false;

            if (distance <= attackDistance)
            {
                StopMovement();
                FaceTarget();
                CurrentState = EnemyState.Attacking;

                if (!isAttacking && Time.time >= nextAttackTime)
                    StartAttack();
            }
            else if (!isAttacking && distance <= chaseDistance)
            {
                CurrentState = EnemyState.Chasing;
                ResumeMovement();

                if (Time.time >= nextPathUpdateTime && agent.enabled && agent.isOnNavMesh)
                {
                    agent.speed = chaseSpeed;
                    agent.SetDestination(target.position);
                    nextPathUpdateTime = Time.time + pathUpdateInterval;
                }
            }
            else
            {
                StopMovement();
                CurrentState = EnemyState.Idle;
            }
        }
        else
        {
            if (!isSearching && lastKnownPosition != Vector3.zero)
                isSearching = true;

            if (isSearching)
            {
                CurrentState = EnemyState.Searching;
                ResumeMovement();
                agent.speed = chaseSpeed;

                if (agent.enabled && agent.isOnNavMesh)
                    agent.SetDestination(lastKnownPosition);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
                {
                    searchTimer -= Time.deltaTime;
                    if (searchTimer <= 0f)
                    {
                        isSearching = false;
                        StopMovement();
                        CurrentState = EnemyState.Idle;
                        agent.speed = patrolSpeed;
                    }
                }
            }
            else
            {
                StopMovement();
                CurrentState = EnemyState.Idle;
            }
        }

        UpdateAnimator();
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
            target = player.transform;
    }

    private void StartAttack()
    {
        isAttacking = true;
        CurrentState = EnemyState.Attacking;
        nextAttackTime = Time.time + attackCooldown;

        StopMovement();

        if (HasParameter("Attack", AnimatorControllerParameterType.Trigger))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }
    }

    public void Animation_BeginAttackWindow()
    {
        if (isDead || weaponTrigger == null) return;
        weaponTrigger.BeginAttack(damage);
    }

    public void Animation_EndAttackWindow()
    {
        if (weaponTrigger != null)
            weaponTrigger.EndAttack();
    }

    public void Animation_AttackFinished()
    {
        isAttacking = false;
        if (weaponTrigger != null)
            weaponTrigger.EndAttack();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        isSearching = false;
        CurrentState = EnemyState.Dead;

        weaponTrigger?.EndAttack();

        if (agent != null)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        if (HasParameter("Death", AnimatorControllerParameterType.Trigger))
            anim.SetTrigger("Death");

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    private void StopMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void ResumeMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private void FaceTarget()
    {
        if (target == null) return;

        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        float speed = 0f;
        if (agent != null && agent.enabled && agent.speed > 0.01f)
            speed = agent.velocity.magnitude / agent.speed;

        if (HasParameter("Speed", AnimatorControllerParameterType.Float))
            anim.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    private bool HasParameter(string paramName, AnimatorControllerParameterType type)
    {
        if (anim == null) return false;

        foreach (var param in anim.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }
        return false;
    }

    private bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 dirToPlayer = target.position - transform.position;
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer > viewRadius) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleToPlayer > viewAngle / 2f) return false;

        Vector3 eyeOrigin = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = target.position + Vector3.up * eyeHeight;
        Vector3 dir = (targetPos - eyeOrigin).normalized;

        if (Physics.Raycast(eyeOrigin, dir, distanceToPlayer, obstacleMask))
            return false;

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        bool currentlyVisible = target != null && CanSeePlayer();
        Gizmos.color = currentlyVisible ? Color.green : Color.red;

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        if (currentlyVisible)
            Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, target.position + Vector3.up * eyeHeight);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}