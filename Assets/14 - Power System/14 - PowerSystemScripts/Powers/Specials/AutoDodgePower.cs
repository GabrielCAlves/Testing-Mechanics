// AutoDodgePower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewAutoDodgePower", menuName = "Powers/Special/Auto Dodge Power")]
public class AutoDodgePower : Power
{
    [Header("Configurações de Auto-Dodge")]
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;
    public float detectionRadius = 5f;
    public float minimumApproachSpeed = 1f;
    public LayerMask threatLayers;
    public GameObject dodgeEffect;
    public AudioClip dodgeSound;
    public Health health;

    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;

        if(health == null)
        {
            health = user.GetComponent<Health>();
            if (health == null)
            {
                Debug.LogWarning("Health component not found on user.");
            }
        }
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || user == null) return;

        cooldownTimer -= Time.deltaTime;

        if (!isDodging && cooldownTimer <= 0) // cooldownTimer -> responsável por, às vezes, o inimigo acertar o jogador mesmo quando ele está se esquivando, então o cooldownTimer é usado para evitar que o jogador se esquive novamente imediatamente após o dodge.
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
            Debug.Log($"Ameaça detectada: {threat.name} a uma distância de {Vector3.Distance(user.transform.position, threat.transform.position)}");

            // Verifica se está se aproximando
            Vector3 direction = threat.transform.position - user.transform.position;
            float approachSpeed = Vector3.Dot(threat.attachedRigidbody.linearVelocity, direction.normalized);

            if (approachSpeed >= minimumApproachSpeed)
            {
                Debug.Log($"Ameaça se aproximando: {threat.name} com velocidade de aproximação {approachSpeed}");

                PerformDodge(user, direction);
                break;
            }
        }
    }

    void PerformDodge(GameObject user, Vector3 threatDirection)
    {
        if(health != null)
            health.isImmortal = true; // Torna o usuário imortal durante o dodge

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

        if(health != null)
            health.isImmortal = false; // Remove a imortalidade após o dodge
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;
        isDodging = false;
    }
}