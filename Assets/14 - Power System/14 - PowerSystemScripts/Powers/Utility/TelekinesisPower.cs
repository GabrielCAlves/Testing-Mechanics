// TelekinesisPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewTelekinesisPower", menuName = "Powers/Utility/Telekinesis Power")]
public class TelekinesisPower : Power
{
    [Header("Configurações de Telecinese")]
    public float grabRange = 10f;
    public float moveSensitivity = 2f;
    public float rotationSensitivity = 5f;
    public float throwForce = 30f;
    public float upForce = 10f;
    public float holdDistance = 3f;
    public float maxLiftMass = 50f;
    public LayerMask grabbableLayers;
    public GameObject telekinesisEffect;
    public Material telekinesisMaterial;
    public bool keepObjectInFront = true;
    public bool lookAtObjectWhileHolding = true;

    private GameObject grabbedObject;
    private Rigidbody grabbedRB;
    private float originalDrag;
    private GameObject effectObject;
    private bool isActive = false;
    private Vector3 targetPosition = Vector3.zero;
    private bool isRotating = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        if (grabbedObject == null)
        {
            TryGrabObject(user);
        }
        else
        {
            ThrowObject(user);
        }
    }

    void TryGrabObject(GameObject user)
    {
        RaycastHit hit;
        if (Physics.Raycast(user.transform.position, user.transform.forward, out hit, grabRange, grabbableLayers))
        {
            if (hit.rigidbody != null && hit.rigidbody.mass <= maxLiftMass)
            {
                grabbedObject = hit.collider.gameObject;
                grabbedRB = hit.rigidbody;
                originalDrag = grabbedRB.linearDamping;
                grabbedRB.linearDamping = 10f;
                grabbedRB.useGravity = false;

                // Efeito visual
                if (telekinesisEffect != null)
                {
                    effectObject = Instantiate(telekinesisEffect, grabbedObject.transform);
                    effectObject.transform.localPosition = Vector3.zero;

                    var renderer = effectObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = telekinesisMaterial;
                    }
                }

                isActive = true;
            }
        }
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || grabbedObject == null) return;

        // Move objeto com o mouse/cursor
        if (!isRotating)
        {
            if (keepObjectInFront)
            {
                targetPosition = user.transform.position + user.transform.forward * holdDistance; // Mantém o objeto na frente do usuário
            }
            targetPosition += user.transform.right * Input.GetAxis("Mouse X") * moveSensitivity;
            targetPosition += user.transform.up * Input.GetAxis("Mouse Y") * moveSensitivity;
            targetPosition += user.transform.forward * Input.GetAxis("Mouse ScrollWheel") * moveSensitivity; // Permite mover o objeto para frente e para trás com a roda do mouse
            targetPosition = Vector3.ClampMagnitude(targetPosition, grabRange); // Limita a distância do objeto em relação ao usuário

            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, targetPosition, Time.deltaTime * 10f));
        }

        if(lookAtObjectWhileHolding)
        {
            user.transform.LookAt(new Vector3(grabbedRB.position.x, user.transform.position.y, grabbedRB.position.z)); // Mantém o usuário olhando para o objeto, mas sem inclinar para cima ou para baixo
        }

        // Rotaciona objeto
        if (Input.GetMouseButton(1))
        {
            isRotating = true;
            float rotX = Input.GetAxis("Mouse X") * rotationSensitivity;
            float rotY = Input.GetAxis("Mouse Y") * rotationSensitivity;
            float rotZ = Input.GetAxis("Mouse ScrollWheel") * rotationSensitivity;
            grabbedRB.MoveRotation(grabbedRB.rotation * Quaternion.Euler(rotY, rotX, rotZ));
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        // Solta ou arremessa
        if (Input.GetMouseButtonUp(0))
        {
            ThrowObject(user);
        }
    }

    void ThrowObject(GameObject user)
    {
        if (grabbedObject != null)
        {
            grabbedRB.linearDamping = originalDrag;
            grabbedRB.useGravity = true;
            grabbedRB.AddForce(user.transform.forward * throwForce + user.transform.up * upForce, ForceMode.Impulse);

            if (effectObject != null)
            {
                Destroy(effectObject);
            }
        }

        grabbedObject = null;
        grabbedRB = null;
        isActive = false;
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        if (grabbedObject != null)
        {
            grabbedRB.linearDamping = originalDrag;
            grabbedRB.useGravity = true;
            grabbedObject = null;
            grabbedRB = null;
        }

        if (effectObject != null)
        {
            Destroy(effectObject);
        }

        isActive = false;
    }
}