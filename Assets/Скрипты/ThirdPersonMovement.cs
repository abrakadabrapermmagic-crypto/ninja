using UnityEngine;
using FirstPersonMobileTools; // Подключаем пространство имен твоего джойстика

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Setup")]
    public Joystick joystick;         // Ссылка на твой UI джойстик
    public Transform cam;             // Ссылка на Main Camera

    [Header("Settings")]
    public float speed = 6f;          // Скорость бега
    public float turnSmoothTime = 0.1f; // Плавность поворота
    public float gravity = -9.81f;    // Сила гравитации

    private CharacterController controller;
    private float turnSmoothVelocity;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Если камера не назначена, пытаемся найти главную
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Получаем ввод от твоего джойстика
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Создаем вектор направления (X и Z, Y пропускаем)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Если джойстик отклонен (есть ввод)
        if (direction.magnitude >= 0.1f)
        {
            // 2. Вычисляем угол поворота
            // Atan2 дает угол ввода, прибавляем угол камеры, чтобы движение было "от камеры"
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // Плавное вращение персонажа
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 3. Двигаем персонажа в направлении, куда он смотрит
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // 4. Применяем гравитацию
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Прижимаем к земле
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}