using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantMove : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;

    public void Init(Vector3 direction, float speed)
    {
        rb = GetComponent<Rigidbody>();
        velocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
