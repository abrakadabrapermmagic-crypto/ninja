using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BotAI : MonoBehaviour
{
    [Header("Настройки преследования")]
    public Transform target;
    public float stoppingDistance = 2f;
    public bool followOnlyWhenVisible = true; // Если false, будет идти всегда

    [Header("Настройки Поля Зрения (FOV)")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask targetMask;   // Установите здесь слой Player
    public LayerMask obstacleMask; // Установите здесь слой Ground/Walls

    private NavMeshAgent agent;
    private Animator animator;
    private bool canSeeTarget;
    private int speedHash;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.stoppingDistance = stoppingDistance;

        // Кэшируем ID параметра для оптимизации и проверки
        speedHash = Animator.StringToHash("Speed");

        // Проверка: есть ли такой параметр в Аниматоре вообще?
        bool hasSpeedParam = false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == "Speed") hasSpeedParam = true;
        }
        if (!hasSpeedParam) Debug.LogError("Ошибка: В Аниматоре нет параметра 'Speed'! Создайте Float параметр с таким именем.");
    }

    void Update()
    {
        FieldOfViewCheck();

        // Условие движения
        bool shouldFollow = !followOnlyWhenVisible || (followOnlyWhenVisible && canSeeTarget);

        if (target != null && shouldFollow)
        {
            agent.SetDestination(target.position);
        }

        // Обновляем анимацию (проверяем скорость агента)
        float currentSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(speedHash, currentSpeed);
    }

    private void FieldOfViewCheck()
    {
        // Ищем объекты в радиусе на слое targetMask
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform targetInRadius = rangeChecks[0].transform;
            Vector3 directionToTarget = (targetInRadius.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetInRadius.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    canSeeTarget = true;
                    return;
                }
            }
        }
        canSeeTarget = false;
    }

    private void OnDrawGizmos()
    {
        // Рисуем радиус в редакторе
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Рисуем границы угла обзора
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = canSeeTarget ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }
}