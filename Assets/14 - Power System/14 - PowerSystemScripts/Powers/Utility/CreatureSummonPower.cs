// CreatureSummonPower.cs
using FreeflowCombatSpace;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject summonEffect;
    public AudioClip summonSound;

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
            // Remove a criatura mais antiga
            GameObject oldest = summonedCreatures[0];
            summonedCreatures.RemoveAt(0);
            Destroy(oldest);
        }

        // Escolhe criatura aleatória
        GameObject creaturePrefab = creaturesToSummon[Random.Range(0, creaturesToSummon.Length)];

        // Posição de invocação
        Vector3 spawnPosition = user.transform.position +
            user.transform.forward * summonRange +
            Random.insideUnitSphere * 2f;
        spawnPosition.y = user.transform.position.y;

        // Instancia criatura
        GameObject creature = Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);

        // Configura criatura
        var health = creature.GetComponent<Health>();
        if (health != null)
        {
            health.maxHealth *= creatureHealthMultiplier;
            health.currentHealth = health.maxHealth;
        }

        var damage = creature.GetComponent<DamageDealer>();
        if (damage != null)
        {
            damage.damageMultiplier = creatureDamageMultiplier;
        }

        // Adiciona auto-destruição
        creature.AddComponent<CreatureController>().Initialize(creatureLifetime, this);

        // Adiciona à lista
        summonedCreatures.Add(creature);

        // Efeitos
        if (summonEffect != null)
        {
            Instantiate(summonEffect, spawnPosition, Quaternion.identity);
        }

        if (summonSound != null)
        {
            AudioSource.PlayClipAtPoint(summonSound, spawnPosition);
        }
    }

    public void RemoveCreature(GameObject creature)
    {
        if (summonedCreatures.Contains(creature))
        {
            summonedCreatures.Remove(creature);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        foreach (var creature in summonedCreatures)
        {
            if (creature != null)
            {
                Destroy(creature);
            }
        }
        summonedCreatures.Clear();
    }
}

public class CreatureController : MonoBehaviour
{
    private float lifetime;
    private CreatureSummonPower power;
    private float timer;
    private bool isInitialized = false;

    public void Initialize(float lifetime, CreatureSummonPower power)
    {
        this.lifetime = lifetime;
        this.power = power;
        timer = 0f;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            if (power != null)
            {
                power.RemoveCreature(gameObject);
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (power != null)
        {
            power.RemoveCreature(gameObject);
        }
    }
}