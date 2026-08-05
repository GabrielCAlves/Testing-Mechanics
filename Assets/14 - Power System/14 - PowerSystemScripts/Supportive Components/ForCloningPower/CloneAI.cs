using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(Health))]
public class CloneAI : MonoBehaviour
{
    public enum MovementMode
    {
        NavMeshAgent,
        DirectMovement
    }

    [Header("Modo de Movimento")]
    public MovementMode movementMode = MovementMode.DirectMovement;

    [Header("Referências")]
    private GameObject owner;
    private CloningPower power;
    private Transform target;
    private Health health;
    private Animator animator;
    private NavMeshAgent agent;

    [Header("Configurações")]
    private float damageMultiplier = 0.5f;
    private float speedMultiplier = 0.8f;
    private float attackRange = 3f;
    private float attackCooldown = 1.5f;
    private float detectionRange = 10f;
    private float followDistance = 3f;
    private float stopDistance = 2.5f;
    private LayerMask enemyLayers;
    private bool canAttack = true;

    [Header("Configurações de Movimento Direto")]
    public float directMoveSpeed = 4f;
    public float directRotationSpeed = 8f;
    public float directStoppingDistance = 0.5f;

    [Header("Configurações de Evasão")]
    public float separationRadius = 1.5f;      // Distância para evitar outro clone
    public float separationForce = 2f;         // Força de repulsão
    public LayerMask cloneLayer;               // Layer dos clones

    [Header("Status")]
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isPositioned = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private bool isStopped = false;
    private Vector3 separationVelocity = Vector3.zero;

    public void Initialize(
        GameObject owner,
        CloningPower power,
        float damageMultiplier,
        float speedMultiplier,
        float attackRange,
        float attackCooldown,
        float detectionRange,
        float followDistance,
        LayerMask enemyLayers,
        bool canAttack)
    {
        this.owner = owner;
        this.power = power;
        this.damageMultiplier = damageMultiplier;
        this.speedMultiplier = speedMultiplier;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
        this.detectionRange = detectionRange;
        this.followDistance = followDistance;
        this.enemyLayers = enemyLayers;
        this.canAttack = canAttack;
        this.stopDistance = attackRange * 0.5f;

        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Configura NavMeshAgent se estiver usando
        if (movementMode == MovementMode.NavMeshAgent && agent != null)
        {
            agent.speed *= speedMultiplier;
            agent.stoppingDistance = stopDistance;
            agent.autoBraking = true;
            agent.radius = 0.3f;
            agent.angularSpeed = 360f;
            agent.acceleration = 8f;
        }
        else if (movementMode == MovementMode.NavMeshAgent && agent == null)
        {
            Debug.LogWarning("NavMeshAgent não encontrado! Mudando para DirectMovement.");
            movementMode = MovementMode.DirectMovement;
        }

        // Se for movimento direto, desativa o NavMeshAgent
        if (movementMode == MovementMode.DirectMovement && agent != null)
        {
            agent.enabled = false;
        }

        // Configura a tag, mas poderia ser a layer, do clone para identificação
        gameObject.tag = "Clone";

        // Define a layer se não estiver definida
        if (cloneLayer.value == 0)
        {
            cloneLayer = LayerMask.GetMask("Clone");
        }

        SphereCollider detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRange;

        FindTarget();
        lastPosition = transform.position;

        Debug.Log($"Clone AI inicializada! Modo: {movementMode}, StopDistance: {stopDistance}");
    }

    void Update()
    {
        if (health == null || health.currentHealth <= 0)
        {
            DestroyClone();
            return;
        }

        attackTimer -= Time.deltaTime;

        // Procura alvo periodicamente
        if (target == null || Vector3.Distance(transform.position, target.position) > detectionRange)
        {
            FindTarget();
        }

        // --- COMPORTAMENTO PRINCIPAL ---
        if (target == null)
        {
            FollowOwner();
            UpdateAnimations();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // --- SE ESTÁ NO ALCANCE DE ATAQUE ---
        if (distanceToTarget <= attackRange)
        {
            StopMovement();
            isPositioned = true;
            Attack();
            UpdateAnimations();
            return;
        }

        // --- SE ESTÁ NA DISTÂNCIA DE PARADA ---
        if (distanceToTarget <= stopDistance)
        {
            StopMovement();
            isPositioned = true;
            UpdateAnimations();
            return;
        }

        // --- SE ESTÁ LONGE, PERSEGUE ---
        if (distanceToTarget <= detectionRange)
        {
            ChaseTarget();
        }
        else
        {
            FollowOwner();
        }

        UpdateAnimations();
    }

    void FindTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayers);

        if (enemies.Length > 0)
        {
            float closestDistance = float.MaxValue;
            GameObject closestEnemy = null;

            foreach (var enemy in enemies)
            {
                if (enemy.gameObject == owner || enemy.gameObject == gameObject) continue;
                if (enemy.CompareTag("Clone")) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.gameObject;
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy.transform;
            }
        }
        else
        {
            target = null;
        }
    }

    void FollowOwner()
    {
        if (owner == null) return;

        float distanceToOwner = Vector3.Distance(transform.position, owner.transform.position);

        // Teleporta se estiver muito longe
        if (distanceToOwner > detectionRange * 2)
        {
            Vector3 spawnPos = owner.transform.position + (owner.transform.forward * -2f);
            transform.position = spawnPos;
            if (movementMode == MovementMode.NavMeshAgent && agent != null && agent.enabled)
            {
                agent.Warp(spawnPos);
            }
            return;
        }

        // Se está perto do dono, PARA
        if (distanceToOwner <= followDistance)
        {
            StopMovement();
            isPositioned = true;
            return;
        }

        // Se está longe, segue o dono
        MoveToTarget(owner.transform.position);
    }

    void ChaseTarget()
    {
        if (target == null) return;

        // Verifica se está preso (apenas para NavMeshAgent)
        if (movementMode == MovementMode.NavMeshAgent && agent != null && agent.enabled)
        {
            if (agent.velocity.magnitude < 0.1f && agent.remainingDistance > stopDistance * 2)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > 2f)
                {
                    agent.ResetPath();
                    agent.SetDestination(target.position);
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }

        MoveToTarget(target.position);
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        isStopped = false;

        // --- CALCULA A DIREÇÃO PRINCIPAL ---
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Se estiver muito perto, não se move
        if (distance < directStoppingDistance)
        {
            StopMovement();
            return;
        }

        // --- APLICA EVASÃO ENTRE CLONES ---
        Vector3 separationForceVector = GetSeparationForce();

        // Combina a direção principal com a força de evasão
        Vector3 finalDirection = moveDirection + separationForceVector * separationForce;
        finalDirection.Normalize();

        if (movementMode == MovementMode.NavMeshAgent && agent != null && agent.enabled)
        {
            // NavMeshAgent não suporta evasão diretamente, mas pode-se ajustar o destino
            Vector3 adjustedTarget = targetPosition + separationForceVector * separationForce * 0.5f;
            agent.isStopped = false;
            agent.SetDestination(adjustedTarget);
        }
        else
        {
            // --- MOVIMENTO DIRETO COM EVASÃO ---
            float speed = directMoveSpeed * speedMultiplier;
            transform.position += finalDirection * speed * Time.deltaTime;

            // Rotaciona
            if (finalDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(finalDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, directRotationSpeed * Time.deltaTime);
            }
        }
    }

    Vector3 GetSeparationForce()
    {
        Vector3 separation = Vector3.zero;
        int cloneCount = 0;

        // Procura outros clones próximos
        Collider[] nearbyClones = Physics.OverlapSphere(transform.position, separationRadius, cloneLayer);

        foreach (var clone in nearbyClones)
        {
            if (clone.gameObject == gameObject) continue;
            if (!clone.CompareTag("Clone")) continue;

            Vector3 direction = transform.position - clone.transform.position;
            float distance = direction.magnitude;

            if (distance < separationRadius && distance > 0.01f)
            {
                // Quanto mais próximo, maior a força de repulsão
                float strength = 1f - (distance / separationRadius);
                separation += direction.normalized * strength;
                cloneCount++;
            }
        }

        if (cloneCount > 0)
        {
            separation /= cloneCount;
        }

        return separation.normalized;
    }

    void StopMovement()
    {
        isStopped = true;

        if (movementMode == MovementMode.NavMeshAgent && agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }

    void Attack()
    {
        if (attackTimer > 0) return;
        if (target == null) return;

        attackTimer = attackCooldown;
        isAttacking = true;

        // Garante que está parado
        StopMovement();

        // Aplica dano
        var damageDealer = GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            damageDealer.DealDamage(target.gameObject);
        }
        else
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                float damage = 10f * damageMultiplier;
                targetHealth.TakeDamage(damage);
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Rotaciona para encarar o alvo
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        Invoke(nameof(ResetAttack), 0.3f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        float speed = 0f;

        if (movementMode == MovementMode.NavMeshAgent && agent != null && agent.enabled && !agent.isStopped)
        {
            speed = agent.velocity.magnitude;
        }
        else if (movementMode == MovementMode.DirectMovement && !isStopped)
        {
            speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
            lastPosition = transform.position;
        }

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsAttacking", isAttacking);
    }

    void DestroyClone()
    {
        if (power != null)
        {
            power.RemoveClone(gameObject);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (enemyLayers == (enemyLayers | (1 << other.gameObject.layer)))
        {
            if (other.gameObject != owner && !other.CompareTag("Clone"))
            {
                target = other.transform;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}