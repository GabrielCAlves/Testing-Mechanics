using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemMovement_New : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isJumping;

    [SerializeField] private Vector2 direction;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Movimento horizontal
        direction = moveAction.ReadValue<Vector2>().normalized;
        //Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        //transform.position += new Vector3(move.x, 0, move.y) * speed * Time.deltaTime;
        //transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime; // Doesn't follow the turned direction
        transform.Translate(new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime); // Does follow the turned direction

        // Verifica se está no chão e reseta a velocidade vertical
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeno valor para manter no chão
            isJumping = false;
        }

        if (isGrounded && jumpAction.triggered && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            Debug.Log("Pulo executado! Velocidade Y: " + velocity.y);
        }

        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimento vertical SEPARADO do horizontal
        if(isJumping) transform.position += new Vector3(0, velocity.y, 0) * Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            
            // Resetar pulo ao tocar no chão
            if (velocity.y <= 0)
            {
                isJumping = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }
}