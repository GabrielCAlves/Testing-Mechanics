using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCloningPower", menuName = "Powers/Utility/Cloning Power")]
public class CloningPower : Power
{
    [Header("Configurações de Clonagem")]
    public int maxClones = 3;
    public float cloneLifetime = 30f;
    public float cloneDamageMultiplier = 0.5f;
    public float cloneHealthMultiplier = 0.5f;
    public float cloneSpeedMultiplier = 0.8f;
    public GameObject clonePrefab;
    public GameObject spawnEffect;
    public AudioClip spawnSound;
    public bool clonesCanAttack = true;
    public float cloneAttackRange = 3f;
    public float cloneAttackCooldown = 1.5f;

    [Header("Comportamento dos Clones")]
    public float detectionRange = 10f;
    public float followDistance = 3f;
    public LayerMask enemyLayers;

    [Header("Modo de Movimento dos Clones")]
    public CloneAI.MovementMode cloneMovementMode = CloneAI.MovementMode.DirectMovement;

    [Header("Configurações de Evasão entre Clones")]
    public float separationRadius = 1.5f;
    public float separationForce = 2f;
    public LayerMask cloneLayer;

    private List<GameObject> activeClones = new List<GameObject>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        CreateClone(user);
    }

    void CreateClone(GameObject user)
    {
        // Remove clones antigos se estiver no limite
        if (activeClones.Count >= maxClones)
        {
            GameObject oldest = activeClones[0];
            activeClones.RemoveAt(0);
            Destroy(oldest);
        }

        // Cria novo clone
        GameObject clone;
        if (clonePrefab != null)
        {
            clone = Instantiate(clonePrefab,
                user.transform.position + user.transform.forward * 2f + Random.insideUnitSphere * 1f,
                user.transform.rotation);
        }
        else
        {
            clone = Instantiate(user,
                user.transform.position + user.transform.forward * 2f + Random.insideUnitSphere * 1f,
                user.transform.rotation);
        }

        // Desativa o PlayerMovement no clone
        var playerMovement = clone.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Desativa o NavMeshAgent se não for usar
        var navMeshAgent = clone.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null && cloneMovementMode == CloneAI.MovementMode.DirectMovement)
        {
            navMeshAgent.enabled = false;
        }

        // Configura Health
        var health = clone.GetComponent<Health>();
        if (health == null)
        {
            health = clone.AddComponent<Health>();
        }
        health.maxHealth *= cloneHealthMultiplier;
        health.currentHealth = health.maxHealth;

        // Adiciona o script de IA do clone
        CloneAI cloneAI = clone.GetComponent<CloneAI>();
        if (cloneAI == null)
        {
            cloneAI = clone.AddComponent<CloneAI>();
        }

        // Configura o modo de movimento
        cloneAI.movementMode = cloneMovementMode;

        // Configura a IA
        cloneAI.Initialize(
            user,
            this,
            cloneDamageMultiplier,
            cloneSpeedMultiplier,
            cloneAttackRange,
            cloneAttackCooldown,
            detectionRange,
            followDistance,
            enemyLayers,
            clonesCanAttack
        );

        // --- CONFIGURA EVASÃO ---
        cloneAI.separationRadius = separationRadius;
        cloneAI.separationForce = separationForce;
        if (cloneLayer.value != 0)
        {
            cloneAI.cloneLayer = cloneLayer;
        }
        else
        {
            // Se não foi definido, usa a layer do clone
            cloneAI.cloneLayer = LayerMask.GetMask("Clone");
        }

        // Configura DamageDealer se existir
        var damage = clone.GetComponent<DamageDealer>();
        if (damage != null)
        {
            damage.damageMultiplier = cloneDamageMultiplier;
        }

        // Auto-destruição
        CloneController controller = clone.GetComponent<CloneController>();
        if (controller == null)
        {
            controller = clone.AddComponent<CloneController>();
        }
        controller.Initialize(cloneLifetime, this);

        // Adiciona à lista
        activeClones.Add(clone);

        // Efeitos
        if (spawnEffect != null)
        {
            Instantiate(spawnEffect, clone.transform.position, Quaternion.identity);
        }

        if (spawnSound != null)
        {
            AudioSource.PlayClipAtPoint(spawnSound, clone.transform.position);
        }

        Debug.Log($"Clone criado! Modo: {cloneMovementMode}, Total: {activeClones.Count}/{maxClones}");
    }

    public void RemoveClone(GameObject clone)
    {
        if (activeClones.Contains(clone))
        {
            activeClones.Remove(clone);
            Debug.Log($"Clone removido. Restam: {activeClones.Count}");
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        // Remove todos os clones
        //foreach (var clone in activeClones)
        //{
        //    if (clone != null)
        //    {
        //        Destroy(clone);
        //    }
        //}
        //activeClones.Clear();
        Debug.Log("Todos os clones foram destruídos");
    }
}