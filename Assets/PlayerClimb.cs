using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    public float climbSpeed = 3f;
    private Rigidbody rb;
    private bool isClimbing = false;
    private float defaultGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultGravity = rb.useGravity ? 1f : 0f; // Сохраняем исходную гравитацию
    }

    void Update()
    {
        if (isClimbing)
        {
            Climb();
        }
    }

    private void Climb()
    {
        float verticalInput = Input.GetAxis("Vertical"); // W/S или стрелки
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, verticalInput * climbSpeed, rb.velocity.z);

        // Выход при горизонтальном движении (E для выхода или автоматом)
        if (Input.GetKeyDown(KeyCode.E) || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }
}

