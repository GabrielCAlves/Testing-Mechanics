// TeleportPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeleportPower", menuName = "Powers/Movement/Teleport Power")]
public class TeleportPower : Power
{
    [Header("Configurações do Teletransporte")]
    public float maxDistance = 30f;
    public float teleportDelay = 0.1f;
    public GameObject teleportStartEffect;
    public GameObject teleportEndEffect;
    public AudioClip teleportSound;
    public bool canTeleportThroughWalls = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        var mono = user.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(PerformTeleport(user));
    }

    System.Collections.IEnumerator PerformTeleport(GameObject user)
    {
        // Efeito de saída
        if (teleportStartEffect != null)
        {
            Instantiate(teleportStartEffect, user.transform.position, Quaternion.identity);
        }

        // Esconde o jogador
        user.GetComponent<MeshRenderer>().enabled = false;
        //user.SetActive(false);

        yield return new WaitForSeconds(teleportDelay);

        Vector3 targetPosition = GetTeleportPosition(user);

        // Move o jogador
        user.transform.position = targetPosition;

        // Mostra o jogador
        user.GetComponent<MeshRenderer>().enabled = true;
        //user.SetActive(true);

        // Efeito de chegada
        if (teleportEndEffect != null)
        {
            Instantiate(teleportEndEffect, targetPosition, Quaternion.identity);
        }

        if (teleportSound != null)
        {
            AudioSource.PlayClipAtPoint(teleportSound, targetPosition);
        }
    }

    Vector3 GetTeleportPosition(GameObject user)
    {
        Ray ray = new Ray(user.transform.position, user.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (!canTeleportThroughWalls)
            {
                return hit.point - user.transform.forward * 1f;
            }
            else
            {
                // Verifica se o caminho está livre
                if (!Physics.Raycast(ray, maxDistance))
                {
                    return ray.GetPoint(maxDistance);
                }
                else
                {
                    return user.transform.position + user.transform.forward * maxDistance;
                }
            }
        }
        else
        {
            return user.transform.position + user.transform.forward * maxDistance;
        }
    }
}