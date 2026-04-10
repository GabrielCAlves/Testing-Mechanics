using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Input Debug")]
    [SerializeField] private float x;
    [SerializeField] private float y;

    private Vector3 velocity;
    private bool isJumping;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verificar se está no chão
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeno valor para manter no chão
            isJumping = false;
        }

        // Input de pulo
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            Debug.Log("Pulou! Velocidade Y: " + velocity.y);
        }

        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;

        // Movimento horizontal
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * y;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // Aplicar movimento vertical
        controller.Move(velocity * Time.deltaTime);

        // Debug para verificar estado
        Debug.Log($"Is Grounded: {controller.isGrounded}, Velocity Y: {velocity.y}, Is Jumping: {isJumping}");
    }
}