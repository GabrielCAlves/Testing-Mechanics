// FlightPower.cs
using UnityEngine;

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

    private bool isFlying = false;
    private GameObject flightEffect;
    private Rigidbody rb;
    private CharacterController controller;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        EnableFlight(user);
    }

    void EnableFlight(GameObject user)
    {
        isFlying = true;

        rb = user.GetComponent<Rigidbody>();
        controller = user.GetComponent<CharacterController>();

        // Desabilita gravidade
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Cria efeito visual
        if (flightEffectPrefab != null && flightEffect == null)
        {
            flightEffect = Instantiate(flightEffectPrefab, user.transform);
            flightEffect.transform.localPosition = Vector3.zero;
        }

        // Hover effect
        if (enableHover)
        {
            user.transform.position = new Vector3(
                user.transform.position.x,
                hoverHeight,
                user.transform.position.z
            );
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isFlying = false;

        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        if (flightEffect != null)
        {
            Destroy(flightEffect);
        }
    }

    public void UpdateFlight(GameObject user)
    {
        if (!isFlying) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float up = 0f;
        if (Input.GetButton("Jump"))
            up = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            up = -1f;

        Vector3 moveDirection = user.transform.forward * vertical + user.transform.right * horizontal;
        moveDirection += Vector3.up * up;
        moveDirection.Normalize();

        if (rb != null)
        {
            Vector3 targetVelocity = moveDirection * flightSpeed;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, flightAcceleration * Time.deltaTime);

            // Limita velocidade vertical
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed),
                rb.linearVelocity.z);
        }
        else if (controller != null)
        {
            Vector3 move = moveDirection * flightSpeed * Time.deltaTime;
            controller.Move(move);
        }
    }
}