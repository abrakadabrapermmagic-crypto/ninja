using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    public Slider healthSlider;
    public GameObject gameOverPanel;

    void Start()
    {
        currentHP = maxHP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;

        gameOverPanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        healthSlider.value = currentHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // останавливает игру
    }
}