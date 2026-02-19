using UnityEngine;
using System.Collections;

public class SlowDownTrigger : MonoBehaviour
{
    [Header("Параметры замедления")]
    public float slowMultiplier = 0.5f;  // Множитель скорости (0.5 = половина скорости)
    public float slowDuration = 3f;      // Длительность в секундах
    public GameObject targetObject;      // Целевой объект (оставь пустым для автоопределения по тегу)

    private Rigidbody targetRb;
    private Vector3 originalVelocity;
    private bool isSlowed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег или конкретный объект
        if (targetObject != null && other.gameObject == targetObject ||
            (targetObject == null && other.CompareTag("Player")))  // Измени тег на нужный
        {
            targetRb = other.GetComponent<Rigidbody>();
            if (targetRb != null && !isSlowed)
            {
                originalVelocity = targetRb.velocity;
                StartCoroutine(SlowDownCoroutine());
            }
        }
    }

    private IEnumerator SlowDownCoroutine()
    {
        isSlowed = true;
        targetRb.velocity *= slowMultiplier;  // Мгновенное замедление[web:1]

        yield return new WaitForSeconds(slowDuration);

        // Восстановление оригинальной скорости (или можно применить силу)
        targetRb.velocity = originalVelocity;
        isSlowed = false;
    }
}


