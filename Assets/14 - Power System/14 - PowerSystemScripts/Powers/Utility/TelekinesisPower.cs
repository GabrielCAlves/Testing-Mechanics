// TelekinesisPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewTelekinesisPower", menuName = "Powers/Utility/Telekinesis Power")]
public class TelekinesisPower : Power
{
    [Header("Configurações de Telecinese")]
    public float grabRange = 10f;
    public float throwForce = 30f;
    public float holdDistance = 3f;
    public float maxLiftMass = 50f;
    public LayerMask grabbableLayers;
    public GameObject telekinesisEffect;
    public Material telekinesisMaterial;

    private GameObject grabbedObject;
    private Rigidbody grabbedRB;
    private float originalDrag;
    private GameObject effectObject;
    private bool isActive = false;

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

    public void UpdateTelekinesis(GameObject user)
    {
        if (!isActive || grabbedObject == null) return;

        // Move objeto com o mouse/cursor
        Vector3 targetPosition = user.transform.position + user.transform.forward * holdDistance;
        targetPosition += user.transform.right * Input.GetAxis("Mouse X") * 2f;
        targetPosition += user.transform.up * Input.GetAxis("Mouse Y") * 2f;

        grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, targetPosition, Time.deltaTime * 10f));

        // Rotaciona objeto
        if (Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * 5f;
            float rotY = Input.GetAxis("Mouse Y") * 5f;
            grabbedRB.MoveRotation(grabbedRB.rotation * Quaternion.Euler(rotY, rotX, 0));
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
            grabbedRB.AddForce(user.transform.forward * throwForce, ForceMode.Impulse);

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