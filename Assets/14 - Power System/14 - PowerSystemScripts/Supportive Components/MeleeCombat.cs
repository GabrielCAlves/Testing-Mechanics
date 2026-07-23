using UnityEngine;

[RequireComponent(typeof(Health))]
public class MeleeCombat : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public float damage = 15f;
    public float damageMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask targetLayers;
    public GameObject hitEffect;
    public AudioClip hitSound;

    private Health health;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public void PerformMeleeAttack()
    {
        if (cooldownTimer > 0 || isAttacking)
            return;

        isAttacking = true;
        cooldownTimer = attackCooldown;

        // Detecta alvos na frente
        Collider[] targets = Physics.OverlapSphere(
            transform.position + transform.forward * attackRange * 0.5f,
            attackRange,
            targetLayers
        );

        foreach (var target in targets)
        {
            if (target.gameObject == gameObject) continue;

            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                float finalDamage = damage * damageMultiplier;
                targetHealth.TakeDamage(finalDamage);
            }

            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackMultiplier, ForceMode.Impulse);
            }

            // Efeitos
            if (hitEffect != null)
            {
                Instantiate(hitEffect, target.transform.position, Quaternion.identity);
            }
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, target.transform.position);
            }
        }

        // Animações (opcional)
        Debug.Log($"Ataque corpo a corpo realizado com {damage * damageMultiplier} de dano");

        // Desativa o estado de ataque após um curto período
        Invoke(nameof(ResetAttack), 0.2f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange * 0.5f, attackRange);
    }
}