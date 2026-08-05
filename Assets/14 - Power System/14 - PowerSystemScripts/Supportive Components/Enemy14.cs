using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy14 : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 2f;

    [Header("Detecção")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public LayerMask playerLayer;
    public string playerTag = "Player";
    public Transform target;

    [Header("Ataque")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float attackTimer = 0f;

    [Header("Status")]
    public bool isFrozen = false;
    public float freezeTimer = 0f;
    public float slowFactor = 1f;
    public float slowTimer = 0f;
    public bool isStunned = false;
    public float stunTimer = 0f;

    private Health health;
    private NavMeshAgent agent;
    private Animator animator;
    private float originalSpeed;
    private bool isDead = false;
    private bool isGravityControlled = false;

    void Start()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            originalSpeed = agent.speed;
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Verifica se o agente está ativo
        bool isAgentActive = agent != null && agent.isActiveAndEnabled;

        UpdateStatusEffects();

        if (isFrozen || isStunned)
        {
            // Só tenta parar o agente se ele estiver ativo
            if (isAgentActive)
                agent.isStopped = true;
            return;
        }

        // Se o agente não estiver ativo (controlado pela gravidade), não faz nada
        if (!isAgentActive)
        {
            // Atualiza animação para parado
            if (animator != null)
                animator.SetFloat("Speed", 0f);
            return;
        }

        // A partir daqui, o agente está ativo
        agent.isStopped = false;

        // Aplica slow
        if (slowTimer > 0)
        {
            agent.speed = originalSpeed * slowFactor;
        }
        else
        {
            agent.speed = originalSpeed;
        }

        // Busca alvo
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (IsTargetDetectable())
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                // Ataca (NÃO desativa o agente, apenas para)
                agent.isStopped = true;
                Attack();
            }
            else if (distance <= detectionRange)
            {
                // Persegue
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                // Fora do alcance de detecção, para
                agent.isStopped = true;
            }
        }
        else
        {
            // Alvo não detectável, para
            agent.isStopped = true;
        }

        // Animações
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void Attack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && target != null)
        {
            attackTimer = attackCooldown;

            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }

            Debug.Log($"Inimigo atacou {target.name} causando {attackDamage} de dano");

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    void UpdateStatusEffects()
    {
        bool isAgentActive = agent != null && agent.isActiveAndEnabled;

        // Freeze
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                isFrozen = false;
                if (isAgentActive)
                    agent.isStopped = false;
            }
        }

        // Slow
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
                slowFactor = 1f;
        }

        // Stun
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                if (isAgentActive)
                    agent.isStopped = false;
            }
        }
    }

    void FindTarget()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (players.Length > 0)
        {
            target = players[0].transform;
        }
    }

    bool IsTargetDetectable()
    {
        return target != null && target.CompareTag(playerTag);
    }

    public void ApplySlow(float factor, float duration)
    {
        slowFactor = Mathf.Clamp01(factor);
        slowTimer = duration;
    }

    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
    }

    public void Die()
    {
        isDead = true;
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}