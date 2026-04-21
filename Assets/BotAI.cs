using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BotAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Vision")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 360f;
    public float eyeHeight = 1.5f;
    public LayerMask obstacleMask;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Memory")]
    public float searchTime = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isChasing;
    private bool isSearching;
    private Vector3 lastKnownPosition;
    private float searchTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = patrolSpeed;

        // Проверка наличия параметра Speed (из старого скрипта)
        bool hasSpeedParam = false;
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == "Speed") hasSpeedParam = true;
        }

        if (!hasSpeedParam)
        {
            Debug.LogError("Ошибка: В Аниматоре нет параметра 'Speed'!");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (CanSeePlayer())
        {
            isChasing = true;
            isSearching = false;
            lastKnownPosition = player.position;
            searchTimer = searchTime;

            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                isSearching = true;
            }

            if (isSearching)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(lastKnownPosition);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
                {
                    searchTimer -= Time.deltaTime;
                    if (searchTimer <= 0)
                    {
                        isSearching = false;
                        agent.ResetPath();
                        agent.speed = patrolSpeed;
                    }
                }
            }
            else
            {
                agent.ResetPath();
                agent.speed = patrolSpeed;
            }
        }

        // Обновление анимации Speed (0...1)
        float currentSpeed = 0f;
        if (agent.speed > 0)
        {
            currentSpeed = agent.velocity.magnitude / agent.speed;
        }

        anim.SetFloat("Speed", currentSpeed);
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer > viewRadius) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleToPlayer > viewAngle / 2f) return false;

        Vector3 eyeOrigin = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = player.position + Vector3.up * eyeHeight;
        Vector3 dir = (targetPos - eyeOrigin).normalized;

        if (Physics.Raycast(eyeOrigin, dir, distanceToPlayer, obstacleMask)) return false;

        return true;
    }

    // --- ОТРИСОВКА ВИЗУАЛЬНЫХ ГРАНИЦ (GIZMOS) ---
    private void OnDrawGizmos()
    {
        // Рисуем радиус обзора
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Определяем цвет линий (зеленый если видит, красный если нет)
        // Используем CanSeePlayer() для проверки в реальном времени в редакторе
        bool currentlyVisible = player != null && CanSeePlayer();
        Gizmos.color = currentlyVisible ? Color.green : Color.red;

        // Рисуем границы угла обзора
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        // Дополнительно: рисуем линию до игрока, если он в зоне видимости
        if (currentlyVisible)
        {
            Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, player.position + Vector3.up * eyeHeight);
        }
    }
}