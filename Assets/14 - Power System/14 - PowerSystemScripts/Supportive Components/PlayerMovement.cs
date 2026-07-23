using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 10f;
    public float acceleration = 10f;
    public float jumpForce = 8f;
    public float gravity = -9.81f;

    [Header("Status")]
    public float speedMultiplier = 1f;
    public float jumpMultiplier = 1f;

    private CharacterController controller;
    private Vector3 velocity;
    private float slowFactor = 1f;
    private float slowTimer = 0f;
    private bool isMovementEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verifica se o controller está ativo
        if (controller == null || !controller.enabled)
        {
            // Se o controller estiver desativado, não tenta mover
            return;
        }

        // Slow
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0) slowFactor = 1f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        move.Normalize();

        float currentSpeed = moveSpeed * speedMultiplier * slowFactor;

        // Verifica se o movimento está habilitado
        if (isMovementEnabled)
        {
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        // Pulo
        if (Input.GetButtonDown("Jump") && controller.isGrounded && isMovementEnabled)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity) * jumpMultiplier;
        }

        // Gravidade
        velocity.y += gravity * Time.deltaTime;

        // Só aplica gravidade se o controller estiver ativo
        if (controller.enabled && isMovementEnabled)
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void ApplySlow(float factor, float duration)
    {
        slowFactor = factor;
        slowTimer = duration;
    }

    public void EnableMovement(bool enable)
    {
        isMovementEnabled = enable;
    }

    public bool IsMovementEnabled()
    {
        return isMovementEnabled && controller != null && controller.enabled;
    }
}