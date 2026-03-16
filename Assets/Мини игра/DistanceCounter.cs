using UnityEngine;
using UnityEngine.UI;

public class DistanceCounter : MonoBehaviour
{
    public PlayerController playerController;
    public Text distanceText;
    private float totalDistance = 0f;

    void Update()
    {
        if (playerController.IsMoving())
        {
            totalDistance += 1f * Time.deltaTime; // +1 метр в секунду
        }
        distanceText.text = "Пройдено: " + Mathf.Round(totalDistance) + " м";
    }
}
