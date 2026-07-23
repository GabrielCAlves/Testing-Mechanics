// AutoDodgePower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewAutoDodgePower", menuName = "Powers/Special/Auto Dodge Power")]
public class AutoDodgePower : Power
{
    [Header("Configurações de Auto-Dodge")]
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;
    public float detectionRadius = 3f;
    public LayerMask threatLayers;
    public GameObject dodgeEffect;
    public AudioClip dodgeSound;

    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;
    }

    public void UpdateAutoDodge(GameObject user)
    {
        if (!isActive) return;

        cooldownTimer -= Time.deltaTime;

        if (!isDodging && cooldownTimer <= 0)
        {
            CheckForThreats(user);
        }

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                StopDodge(user);
            }
        }
    }

    void CheckForThreats(GameObject user)
    {
        Collider[] threats = Physics.OverlapSphere(user.transform.position, detectionRadius, threatLayers);

        foreach (var threat in threats)
        {
            // Verifica se está se aproximando
            Vector3 direction = threat.transform.position - user.transform.position;
            float approachSpeed = Vector3.Dot(threat.attachedRigidbody.linearVelocity, direction.normalized);

            if (approachSpeed > 2f)
            {
                PerformDodge(user, direction);
                break;
            }
        }
    }

    void PerformDodge(GameObject user, Vector3 threatDirection)
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        cooldownTimer = dodgeCooldown;

        // Direção do dodge (perpendicular ao perigo)
        Vector3 dodgeDirection = Vector3.Cross(threatDirection, Vector3.up).normalized;
        if (Random.value > 0.5f) dodgeDirection = -dodgeDirection;

        // Aplica movimento
        user.transform.position += dodgeDirection * dodgeDistance;

        // Efeitos
        if (dodgeEffect != null)
        {
            Instantiate(dodgeEffect, user.transform.position, Quaternion.identity);
        }

        if (dodgeSound != null)
        {
            AudioSource.PlayClipAtPoint(dodgeSound, user.transform.position);
        }
    }

    void StopDodge(GameObject user)
    {
        isDodging = false;
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;
        isDodging = false;
    }
}