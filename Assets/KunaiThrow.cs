using UnityEngine;

public class KunaiThrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform throwPoint;

    [Header("Settings")]
    [SerializeField] private float throwForce = 25f;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;

    private void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            ThrowKunai();
        }
    }

    private void ThrowKunai()
    {
        GameObject kunai = Instantiate(
            kunaiPrefab,
            throwPoint.position,
            playerCamera.rotation
        );

        Rigidbody rb = kunai.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = playerCamera.forward * throwForce;
        }
    }
}