using UnityEngine;
using UnityEngine.UI;

public class DistanceCounter : MonoBehaviour
{
    internal readonly float stance;
    public PlayerController playerController;
    public Text distanceText;
    public float TotalDistance = 0f;

    void Update()
    {
        if (playerController.IsMoving())
        {
            TotalDistance += 1f * Time.deltaTime; // +1 метр в секунду
        }
        distanceText.text = "Пройдено: " + Mathf.Round(TotalDistance) + " м";
    }
}
