using UnityEngine;

public class FistDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(10f);
            }
        }
    }
}