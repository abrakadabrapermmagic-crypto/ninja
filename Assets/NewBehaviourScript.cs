using UnityEngine;

public class ClimbableObject : MonoBehaviour
{
    [Header("Настройки climb")]
    public float climbHeight = 5f; // Высота зоны climb (опционально)
    public bool requireKeyPress = false; // Требовать ли клавишу для входа

    // Автоматически добавляем тег и коллайдер
    void Start()
    {
        // Устанавливаем тег
        gameObject.tag = "Climbable";

        // Добавляем триггер-коллайдер если его нет
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;

        Debug.Log($"Climbable объект готов: {gameObject.name}");
    }

    // Визуальный эффект при входе (опционально)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Можно добавить эффект, звук, подсветку
            Debug.Log("Игрок вошел в зону climb: " + other.name);
        }
    }
}
