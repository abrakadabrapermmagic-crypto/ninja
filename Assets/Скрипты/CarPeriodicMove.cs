using System.Collections;
using UnityEngine;

public class CarPeriodicMove : MonoBehaviour
{
    public float moveDistance = 5f; // Расстояние за шаг (единиц)

    void Start()
    {
        StartCoroutine(MoveEverySecond());
    }

    IEnumerator MoveEverySecond()
    {
        while (true)
        {
            // Двигаемся вперёд
            transform.Translate(Vector3.forward * moveDistance);

            // Ждём ровно 1 секунду
            yield return new WaitForSeconds(1f);
        }
    }
}
