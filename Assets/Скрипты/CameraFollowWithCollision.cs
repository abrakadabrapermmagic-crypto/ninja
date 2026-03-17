using UnityEngine;

public class CameraFollowWithCollision: MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float distance = 5f;
    public Transform target;                 // Игрок
    public Vector3 offset = new Vector3(0, 2, -5);
    public float smoothSpeed = 10f;
    public float minDistance = 1f;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;

    private Vector3 currentVelocity;
    private float currentYaw;                // Текущий поворот вокруг игрока

    void LateUpdate()
    {



        if (target == null) return;

        // Вращение камеры при зажатой ПКМ
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentYaw += mouseX * mouseSensitivity;
        }

        // Поворот offset вокруг оси Y игрока
        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 rotatedOffset = rotation * offset;

        // Желаемая позиция камеры
        Vector3 desiredPosition = target.position + rotatedOffset;

        // Проверка столкновений
        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(
            target.position,
            direction.normalized,
            out hit,
            distance, hitMask))
        {
            desiredPosition = hit.point - direction.normalized * 0.3f;
        }

        // Плавное движение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );

        // Камера всегда смотрит на игрока
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
