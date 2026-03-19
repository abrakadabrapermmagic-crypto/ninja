using UnityEngine;
using FirstPersonMobileTools; // ваш джойстик

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Setup")]
    public Joystick joystick;         // назначьте в инспекторе
    public Transform cam;
    public Animator animator;

    [Header("Animation")]
    public string blendParameter = "Speed";

    [Header("Settings")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;

    [Header("Jump")]
    public float jumpHeight = 2f;

    private CharacterController controller;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool jumpRequest;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (joystick == null)
            Debug.LogWarning("[TPM] Joystick not assigned in inspector. Assign it to use mobile input.");

        if (animator == null)
            Debug.LogWarning("[TPM] Animator not found/assigned. Assign animator or place Animator on child.");
    }

    void Update()
    {
        // Получаем вход: либо с джойстика, либо с клавиатуры (фоллбек для теста в редакторе)
        float horizontal = 0f;
        float vertical = 0f;

        if (joystick != null)
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        Vector3 rawInput = new Vector3(horizontal, 0f, vertical);

        // Сила отклонения джойстика (0..1)
        float magnitude = Mathf.Clamp01(new Vector2(horizontal, vertical).magnitude);

        Debug.Log($"[TPM] Input H:{horizontal:F2} V:{vertical:F2} -> magnitude:{magnitude:F2}");

        if (animator != null)
        {
            bool hasParam = false;
            foreach (var p in animator.parameters)
            {
                if (p.name == blendParameter && p.type == AnimatorControllerParameterType.Float)
                {
                    hasParam = true;
                    break;
                }
            }

            if (!hasParam)
            {
                Debug.LogWarning($"[TPM] Animator doesn't contain float parameter '{blendParameter}'. Check name and type (case-sensitive).");
            }
            else
            {
                animator.SetFloat(blendParameter, magnitude);
                Debug.Log($"[TPM] Animator parameter '{blendParameter}' set to {magnitude:F2}");
            }
        }

        Vector3 direction = rawInput.normalized;

        if (magnitude > 0.1f)
        {
            if (cam == null)
            {
                Debug.LogWarning("[TPM] Camera not assigned and Camera.main is null. Rotation may be incorrect.");
            }

            float camY = cam != null ? cam.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camY;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * magnitude * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (jumpRequest)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpRequest = false;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void JumpButtonPressed()
    {
        if (controller != null && controller.isGrounded)
        {
            jumpRequest = true;
        }
    }
}