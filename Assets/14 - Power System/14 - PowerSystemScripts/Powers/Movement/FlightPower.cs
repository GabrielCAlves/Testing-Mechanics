using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[CreateAssetMenu(fileName = "NewFlightPower", menuName = "Powers/Movement/Flight Power")]
public class FlightPower : Power
{
    [Header("Configurações do Vôo")]
    public float flightSpeed = 15f;
    public float flightAcceleration = 5f;
    public float maxVerticalSpeed = 10f;
    public float hoverHeight = 2f;
    public GameObject flightEffectPrefab;
    public bool enableHover = true;

    private GameObject flightEffect;
    private CharacterController characterController;
    private Rigidbody rb;
    private float originalGravity;
    private Vector3 originalVelocity;
    private bool isFlying = false;
    private float verticalVelocity = 0f;

    public override void Activate(GameObject user)
    {
        base.Activate(user);

        // Obtém referências
        characterController = user.GetComponent<CharacterController>();
        rb = user.GetComponent<Rigidbody>();

        // Se tiver Rigidbody, guarda estado original
        if (rb != null)
        {
            originalGravity = rb.useGravity ? 1f : 0f;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Se tiver CharacterController, desabilita a gravidade manual
        if (characterController != null)
        {
            // O CharacterController não tem gravidade nativa, então só precisamos controlar o movimento
            //characterController.enabled = false;
            Debug.Log("FlightPower ativado com CharacterController");
        }

        user.GetComponent<PlayerMovement>().gravity = 0f; // Desativa gravidade do PlayerMovement
        user.GetComponent<PlayerMovement>().velocity.y = 0f;

        // Cria efeito visual
        if (flightEffectPrefab != null && flightEffect == null)
        {
            flightEffect = Instantiate(flightEffectPrefab, user.transform);
            flightEffect.transform.localPosition = Vector3.zero;
        }

        isFlying = true;

        // Hover effect
        if (enableHover && characterController != null)
        {
            // Levanta o personagem ligeiramente
            Vector3 hoverPosition = user.transform.position;
            hoverPosition.y = Mathf.Max(hoverPosition.y, hoverHeight);
            user.transform.position = hoverPosition;
        }

        // Reseta velocidade vertical
        verticalVelocity = 0f;

        Debug.Log($"Vôo ativado - Speed: {flightSpeed}, Hover: {enableHover}");
    }

    public override void UpdatePower(GameObject user)
    {
        if (user == null) return;

        // --- MOVIMENTO COM CHARACTER CONTROLLER ---
        if (characterController != null && characterController.enabled)
        {
            UpdateFlightWithCharacterController(user);
        }
        // --- MOVIMENTO COM RIGIDBODY ---
        else if (rb != null)
        {
            UpdateFlightWithRigidbody(user);
        }
        // --- MOVIMENTO SEM COMPONENTES (fallback) ---
        else
        {
            UpdateFlightWithTransform(user);
        }

        // Atualiza efeito visual
        if (flightEffect != null)
        {
            flightEffect.transform.position = user.transform.position;
        }
    }

    private void UpdateFlightWithCharacterController(GameObject user)
    {
        // Entrada de movimento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Controle de altitude
        float upInput = 0f;
        if (Input.GetButton("Jump")) upInput = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) upInput = -1f;

        // Direção do movimento
        Vector3 moveDirection = user.transform.forward * vertical + user.transform.right * horizontal;
        moveDirection.Normalize();

        // Movimento horizontal
        Vector3 horizontalMovement = moveDirection * flightSpeed * Time.deltaTime;

        // Movimento vertical (controlado manualmente)
        verticalVelocity += upInput * flightAcceleration * Time.deltaTime;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxVerticalSpeed, maxVerticalSpeed);

        // Aplica movimento vertical
        Vector3 verticalMovement = Vector3.up * verticalVelocity * Time.deltaTime;

        // Combina movimentos
        Vector3 totalMovement = horizontalMovement + verticalMovement;

        // Move o CharacterController
        characterController.Move(totalMovement);

        // Se estiver no chão e não estiver subindo, mantém hover
        if (characterController.isGrounded && verticalVelocity <= 0 && enableHover)
        {
            verticalVelocity = 0f;
            // Pequeno impulso para manter hover
            if (user.transform.position.y < hoverHeight)
            {
                Vector3 hoverMove = Vector3.up * (hoverHeight - user.transform.position.y) * Time.deltaTime * 2f;
                characterController.Move(hoverMove);
            }
        }

        // Mantém altura mínima se hover estiver ativo
        if (enableHover && user.transform.position.y < hoverHeight)
        {
            Vector3 hoverMove = Vector3.up * (hoverHeight - user.transform.position.y) * Time.deltaTime * 2f;
            characterController.Move(hoverMove);
        }

        // Debug
        // Debug.Log($"Flight - Pos: {user.transform.position.y}, VelY: {verticalVelocity}, Hover: {enableHover}");
    }

    private void UpdateFlightWithRigidbody(GameObject user)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float up = Input.GetButton("Jump") ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f);

        Vector3 moveDirection = user.transform.forward * vertical + user.transform.right * horizontal;
        moveDirection += Vector3.up * up;
        moveDirection.Normalize();

        Vector3 targetVelocity = moveDirection * flightSpeed;

        // Aplica velocidade gradualmente
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, flightAcceleration * Time.deltaTime);

        // Limita velocidade vertical
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed),
            rb.linearVelocity.z
        );

        // Mantém hover se ativado
        if (enableHover && user.transform.position.y < hoverHeight)
        {
            Vector3 hoverForce = Vector3.up * 10f;
            rb.AddForce(hoverForce, ForceMode.Force);
        }
    }

    private void UpdateFlightWithTransform(GameObject user)
    {
        // Fallback: move o transform diretamente (sem física)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float up = Input.GetButton("Jump") ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f);

        Vector3 moveDirection = user.transform.forward * vertical + user.transform.right * horizontal;
        moveDirection += Vector3.up * up;
        moveDirection.Normalize();

        user.transform.position += moveDirection * flightSpeed * Time.deltaTime;

        // Mantém hover
        if (enableHover && user.transform.position.y < hoverHeight)
        {
            Vector3 hoverPos = user.transform.position;
            hoverPos.y = hoverHeight;
            user.transform.position = hoverPos;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isFlying = false;

        // Restaura Rigidbody
        if (rb != null)
        {
            rb.useGravity = true;
            //rb.constraints = RigidbodyConstraints.None;
        }

        // Restaura CharacterController
        if (characterController != null)
        {
            // Deixa o CharacterController voltar a controlar a gravidade
            // A gravidade é aplicada pelo PlayerMovement
            //characterController.enabled = true;
        }

        user.GetComponent<PlayerMovement>().gravity = -9.81f;

        // Remove efeito visual
        if (flightEffect != null)
        {
            Destroy(flightEffect);
            flightEffect = null;
        }

        // Reseta velocidade vertical
        verticalVelocity = 0f;

        Debug.Log("Vôo desativado");
    }
}