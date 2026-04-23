using UnityEngine;
using UnityEngine.UI; // Обязательно нужно подключить это пространство имен для работы со Slider

public class HealthBar : MonoBehaviour
{
    [Header("Настройки")]
    public Slider healthSlider; // Перетащи сюда свой UI Slider в инспекторе
    public float maxHealth = 100f;

    private float currentHealth;

    void Start()
    {
        // Инициализация здоровья при старте
        currentHealth = maxHealth;

        // Настраиваем слайдер
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    // Метод для получения урона (вызывай его из других скриптов)
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Mathf.Clamp не даст здоровью опуститься ниже 0 или подняться выше maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Обновляем значение слайдера
        healthSlider.value = currentHealth;
    }

    // Метод для лечения (если нужно)
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
    }
}
