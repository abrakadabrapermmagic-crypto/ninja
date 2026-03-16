using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Canvas gameOverCanvas;
    public Text finalDistanceText;
    public GameObject gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DistanceCounter distanceCounter = gameManager.GetComponent<DistanceCounter>();

            if (distanceCounter != null)
            {
                // Исправленная строка формирования текста
                finalDistanceText.text = "Вы продержались: " + Mathf.Round(distanceCounter.totalDistance) + " метров";
                gameOverCanvas.gameObject.SetActive(true);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
