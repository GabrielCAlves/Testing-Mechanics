using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemMovement_New : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    public GameObject character;
    public bool isShooting = false;
    public float rotLerp = .2f;
    public bool useTwoArrowsRot = true;
    private Vector2 previousDirection;
    private bool hasRotated = false;
    private bool directionChanged;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isJumping;

    [SerializeField] private Vector2 direction;

    [SerializeField] private SimpleFSM simpleFSM;
    private float originalSpeed;
    private float sideWalkSpeed;
    private float backwardWalkSpeed;
    private float runSpeed;
    private float timerToRun = 0f;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        simpleFSM = GetComponent<SimpleFSM>();
        originalSpeed = speed;
        sideWalkSpeed = speed / 3f;
        backwardWalkSpeed = speed / 3;
        runSpeed = speed*3/* + (speed / 2)*/;

        previousDirection = Vector2.zero;
    }

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Movimento horizontal
        direction = moveAction.ReadValue<Vector2>().normalized; 
        
        // Verifica se a direção mudou
        directionChanged = direction != previousDirection;

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
            if (simpleFSM != null)
            {
                simpleFSM.SetJump();
            }
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            Debug.Log("Pulo executado! Velocidade Y: " + velocity.y);
        }

        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimento vertical SEPARADO do horizontal
        if(isJumping) transform.position += new Vector3(0, velocity.y, 0) * Time.deltaTime;

        if (simpleFSM != null)
        {
            if (direction.x == 0 && direction.y == 0 && !isShooting)
            {
                simpleFSM.SetIdle();
                timerToRun = 0f;
                return;
            }

            if (direction.y > 0 && (direction.x > 0 || direction.x < 0))
            {
                if (timerToRun >= 2f)
                {
                    speed = runSpeed;
                    simpleFSM.SetRun();
                }
                else
                {
                    speed = originalSpeed;
                    simpleFSM.SetWalkForward();
                    timerToRun += Time.deltaTime;
                }

                if (useTwoArrowsRot && directionChanged)
                {
                    character.transform.Rotate(0, 45 * direction.x, 0);
                    hasRotated = true;
                }

                previousDirection = direction;
                return;
            }

            if (direction.y < 0 && (direction.x > 0 || direction.x < 0))
            {
                speed = backwardWalkSpeed;
                simpleFSM.SetWalkBackward();

                if (useTwoArrowsRot && directionChanged)
                {
                    character.transform.Rotate(0, -45 * direction.x, 0);
                    hasRotated = true;
                }

                previousDirection = direction;
                return;
            }

            VerifyRotation();

            if (direction.x > 0)
            {
                speed = sideWalkSpeed;
                simpleFSM.SetWalkToRight();
                //character.transform.rotation = Quaternion.identity;
            }
            else if (direction.x < 0)
            {
                speed = sideWalkSpeed;
                simpleFSM.SetWalkToLeft();
                //character.transform.rotation = Quaternion.identity;
            }

            if (direction.y > 0)
            {
                if (timerToRun >= 2f)
                {
                    speed = runSpeed;
                    simpleFSM.SetRun();
                }
                else
                {
                    speed = originalSpeed;
                    simpleFSM.SetWalkForward();
                    timerToRun += Time.deltaTime;
                }
                //character.transform.rotation = Quaternion.identity;
            }
            else if (direction.y < 0)
            {
                speed = backwardWalkSpeed;
                simpleFSM.SetWalkBackward();
                //character.transform.rotation = Quaternion.identity;
            }

            
        }
    }

    private void VerifyRotation()
    {
        //if(character != null && transform.parent != null && character.transform.rotation != transform.parent.rotation)
        //    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, transform.parent.rotation, rotLerp);
        //if (character != null && character.transform.rotation != new Quaternion(0, 0, 0, 1))
        //    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, new Quaternion(0,0,0,1), rotLerp);

        if (character != null && character.transform.rotation != transform.rotation)
            character.transform.rotation = transform.rotation;
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

            if (simpleFSM != null)
            {
                simpleFSM.SetIdle();
            }
        }
    }
}