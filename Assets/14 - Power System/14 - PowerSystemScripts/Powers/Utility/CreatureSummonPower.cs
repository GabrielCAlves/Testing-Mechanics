using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewCreatureSummonPower", menuName = "Powers/Utility/Creature Summon Power")]
public class CreatureSummonPower : Power
{
    [Header("Configurações de Invocação")]
    public GameObject[] creaturesToSummon;
    public int maxCreatures = 3;
    public float summonRange = 5f;
    public float creatureLifetime = 60f;
    public float creatureDamageMultiplier = 0.7f;
    public float creatureHealthMultiplier = 0.7f;
    public float creatureSpeedMultiplier = 0.8f;
    public GameObject summonEffect;
    public AudioClip summonSound;

    [Header("Configuração do Prefab")]
    public bool useFirstChildAsCharacter = true; // Se TRUE, usa o primeiro filho como character

    [Header("Comportamento das Criaturas")]
    public float detectionRange = 10f;
    public float followDistance = 3f;
    public float attackRange = 3f;
    public float attackCooldown = 1.5f;
    public LayerMask enemyLayers;
    public bool creaturesCanAttack = true;

    [Header("Modo de Movimento das Criaturas")]
    public CreatureSummonedAI.MovementMode creatureMovementMode = CreatureSummonedAI.MovementMode.DirectMovement;

    [Header("Configurações de Evasão entre Criaturas")]
    public float separationRadius = 1.5f;
    public float separationForce = 2f;
    public LayerMask creatureLayer;

    private List<GameObject> summonedCreatures = new List<GameObject>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        SummonCreature(user);
    }

    void SummonCreature(GameObject user)
    {
        // Remove criaturas mortas
        summonedCreatures.RemoveAll(c => c == null);

        if (summonedCreatures.Count >= maxCreatures)
        {
            GameObject oldest = summonedCreatures[0];
            summonedCreatures.RemoveAt(0);
            Destroy(oldest);
        }

        if (creaturesToSummon == null || creaturesToSummon.Length == 0)
        {
            Debug.LogWarning("Nenhuma criatura para invocar!");
            return;
        }

        // Escolhe criatura aleatória
        GameObject creaturePrefab = creaturesToSummon[Random.Range(0, creaturesToSummon.Length)];

        // Posição de invocação
        Vector3 spawnPosition = user.transform.position +
            user.transform.forward * summonRange +
            Random.insideUnitSphere * 2f;
        spawnPosition.y = user.transform.position.y;

        // Instancia o prefab inteiro
        GameObject creatureContainer = Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);

        // --- DETERMINA QUAL GAMEOBJECT SERÁ A CRIATURA ---
        GameObject creatureCharacter;

        if (useFirstChildAsCharacter && creatureContainer.transform.childCount > 0)
        {
            // Usa o primeiro filho como o character principal
            creatureCharacter = creatureContainer.transform.GetChild(0).gameObject;
            Debug.Log($"Usando primeiro filho '{creatureCharacter.name}' como character da criatura");
        }
        else
        {
            // Usa o container inteiro como character
            creatureCharacter = creatureContainer;
            Debug.Log($"Usando container '{creatureContainer.name}' como character da criatura");
        }

        // --- CONFIGURA O CONTAINER (objeto pai) ---
        // O container vai ser o objeto que será destruído no final
        creatureContainer.tag = "CreatureContainer";

        // --- CONFIGURA O CHARACTER (objeto filho ou o próprio container) ---
        creatureCharacter.tag = "Creature";

        // Desativa NavMeshAgent se não for usar (no character)
        var navMeshAgent = creatureCharacter.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null && creatureMovementMode == CreatureSummonedAI.MovementMode.DirectMovement)
        {
            navMeshAgent.enabled = false;
        }

        // Configura Health no character
        var health = creatureCharacter.GetComponent<Health>();
        if (health == null)
        {
            health = creatureCharacter.AddComponent<Health>();
        }
        health.maxHealth *= creatureHealthMultiplier;
        health.currentHealth = health.maxHealth;

        // Configura DamageDealer no character
        var damageDealer = creatureCharacter.GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            damageDealer.damageMultiplier = creatureDamageMultiplier;
        }

        // Adiciona o script de IA no character
        CreatureSummonedAI creatureAI = creatureCharacter.GetComponent<CreatureSummonedAI>();
        if (creatureAI == null)
        {
            creatureAI = creatureCharacter.AddComponent<CreatureSummonedAI>();
        }

        // Configura o modo de movimento
        creatureAI.movementMode = creatureMovementMode;

        // Configura evasão
        creatureAI.separationRadius = separationRadius;
        creatureAI.separationForce = separationForce;
        creatureAI.creatureLayer = creatureLayer;

        // Inicializa a IA
        creatureAI.Initialize(
            user,
            this,
            creatureDamageMultiplier,
            creatureSpeedMultiplier,
            attackRange,
            attackCooldown,
            detectionRange,
            followDistance,
            enemyLayers,
            creaturesCanAttack
        );

        // --- AUTO-DESTRUIÇÃO NO CONTAINER ---
        CreatureController controller = creatureContainer.GetComponent<CreatureController>();
        if (controller == null)
        {
            controller = creatureContainer.AddComponent<CreatureController>();
        }
        controller.Initialize(creatureLifetime, this, creatureCharacter);

        // Adiciona o container à lista (não o character)
        summonedCreatures.Add(creatureContainer);

        // --- APLICA O EFEITO DE INVOCAÇÃO NA POSIÇÃO DO CHARACTER ---
        if (summonEffect != null)
        {
            Instantiate(summonEffect, creatureCharacter.transform.position, Quaternion.identity);
        }

        if (summonSound != null)
        {
            AudioSource.PlayClipAtPoint(summonSound, creatureCharacter.transform.position);
        }

        Debug.Log($"Criatura invocada! Character: {creatureCharacter.name}, Container: {creatureContainer.name}, Total: {summonedCreatures.Count}/{maxCreatures}");
    }

    public void RemoveCreature(GameObject creatureContainer)
    {
        if (summonedCreatures.Contains(creatureContainer))
        {
            summonedCreatures.Remove(creatureContainer);
            Debug.Log($"Criatura removida. Restam: {summonedCreatures.Count}");
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        //foreach (var creature in summonedCreatures)
        //{
        //    if (creature != null)
        //    {
        //        Destroy(creature);
        //    }
        //}
        //summonedCreatures.Clear();
        Debug.Log("Todas as criaturas foram destruídas");
    }
}