using UnityEngine;

public class ClimbableObject : MonoBehaviour
{
    [Header("Настройки climb")]
    public float climbSpeed = 3f;
    public float climbHeight = 5f;

    private bool isClimbing = false;
    private Transform player;

    void Start()
    {
        gameObject.tag = "Climbable";

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        col.isTrigger = true;
    }

    void Update()
    {
        if (isClimbing && player != null)
        {
            float vertical = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(0, vertical * climbSpeed * Time.deltaTime, 0);
            player.Translate(move);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = false;
            player = null;
        }
    }
}