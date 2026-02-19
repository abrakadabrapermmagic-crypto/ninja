using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;    // Твой игрок
    public Vector3 offset = new Vector3(0, 5, -10); // Дистанция от игрока
    public float smoothSpeed = 0.125f; // Плавность

    void LateUpdate() // LateUpdate лучше подходит для камер
    {
        if (target == null) return;

        // Желаемая позиция
        Vector3 desiredPosition = target.position + offset;
        // Плавный переход
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Камера всегда смотрит на игрока
        transform.LookAt(target);
    }
}