using UnityEngine;

public class PizzaEaterAI : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 3f;

    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player);
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
    }
}
