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

        // Procura o alvo automaticamente
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

        UpdateStatusEffects();

        if (isFrozen || isStunned)
        {
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        if (agent != null)
            agent.isStopped = false;

        // Aplica slow
        if (agent != null && slowTimer > 0)
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

        if(IsTargetDetectable())
        {
            // Distância ao alvo
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                // Ataca
                Attack();
            }
            else if (distance <= detectionRange)
            {
                // Persegue
                if (agent != null)
                {
                    agent.SetDestination(target.position);
                }
                else
                {
                    // Movimento simples sem NavMesh
                    Vector3 direction = (target.position - transform.position).normalized;
                    transform.position += direction * moveSpeed * slowFactor * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
                }
            }
        }
        

        // Animações
        if (animator != null)
        {
            if (agent != null)
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            else
            {
                animator.SetFloat("Speed", moveSpeed * slowFactor);
            }
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

            // Efeito de ataque
            Debug.Log($"Inimigo atacou {target.name} causando {attackDamage} de dano");

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    void UpdateStatusEffects()
    {
        // Freeze
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                isFrozen = false;
                if (agent != null)
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
                if (agent != null)
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
        return target.tag == playerTag;
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
        if (agent != null)
            agent.isStopped = true;
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        if (agent != null)
            agent.isStopped = true;
    }

    public void Die()
    {
        isDead = true;
        if (agent != null)
            agent.isStopped = true;

        // Animação de morte
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