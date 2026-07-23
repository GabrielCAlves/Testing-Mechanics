// PortalCreationPower.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPortalCreationPower", menuName = "Powers/Utility/Portal Creation Power")]
public class PortalCreationPower : Power
{
    [Header("Configurações de Portal")]
    public float portalRange = 30f;
    public float portalRadius = 2f;
    public GameObject portalPrefab;
    public Material portalMaterial;
    public Color portalColor = Color.cyan;
    public AudioClip portalSound;

    private GameObject portal1;
    private GameObject portal2;
    private Vector3 portal1Position;
    private Vector3 portal2Position;
    private bool hasPortal1 = false;
    private bool hasPortal2 = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        CreatePortal(user);
    }

    void CreatePortal(GameObject user)
    {
        Ray ray = new Ray(user.transform.position, user.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, portalRange))
        {
            Vector3 position = hit.point + hit.normal * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(hit.normal);

            if (!hasPortal1)
            {
                // Cria portal 1
                portal1 = Instantiate(portalPrefab, position, rotation);
                portal1.transform.localScale = Vector3.one * portalRadius;
                portal1Position = position;
                hasPortal1 = true;

                // Configura material
                var renderer = portal1.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = portalMaterial;
                    renderer.material.color = portalColor;
                }

                // Adiciona identificador
                portal1.AddComponent<PortalIdentifier>().portalNumber = 1;
            }
            else if (!hasPortal2)
            {
                // Cria portal 2
                portal2 = Instantiate(portalPrefab, position, rotation);
                portal2.transform.localScale = Vector3.one * portalRadius;
                portal2Position = position;
                hasPortal2 = true;

                var renderer = portal2.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = portalMaterial;
                    renderer.material.color = portalColor;
                }

                portal2.AddComponent<PortalIdentifier>().portalNumber = 2;

                // Conecta os portais
                ConnectPortals();
            }
            else
            {
                // Move portal 1 para nova posição
                portal1.transform.position = position;
                portal1.transform.rotation = rotation;
                portal1Position = position;
            }

            if (portalSound != null)
            {
                AudioSource.PlayClipAtPoint(portalSound, position);
            }
        }
    }

    void ConnectPortals()
    {
        if (portal1 != null && portal2 != null)
        {
            var teleporter1 = portal1.AddComponent<PortalTeleporter>();
            teleporter1.targetPortal = portal2;
            teleporter1.targetPosition = portal2Position;

            var teleporter2 = portal2.AddComponent<PortalTeleporter>();
            teleporter2.targetPortal = portal1;
            teleporter2.targetPosition = portal1Position;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        if (portal1 != null)
        {
            Destroy(portal1);
            portal1 = null;
        }
        if (portal2 != null)
        {
            Destroy(portal2);
            portal2 = null;
        }

        hasPortal1 = false;
        hasPortal2 = false;
    }
}

public class PortalIdentifier : MonoBehaviour
{
    public int portalNumber;
}

public class PortalTeleporter : MonoBehaviour
{
    public GameObject targetPortal;
    public Vector3 targetPosition;
    public float teleportDelay = 0.1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != gameObject && targetPortal != null)
        {
            StartCoroutine(TeleportObject(other.gameObject));
        }
    }

    System.Collections.IEnumerator TeleportObject(GameObject obj)
    {
        yield return new WaitForSeconds(teleportDelay);

        obj.transform.position = targetPosition + targetPortal.transform.forward * 1f;

        // Mantém momentum
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = targetPortal.transform.forward * rb.linearVelocity.magnitude;
        }
    }
}