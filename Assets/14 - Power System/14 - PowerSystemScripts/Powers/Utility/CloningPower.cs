// CloningPower.cs
using FreeflowCombatSpace;
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
    public GameObject clonePrefab;
    public GameObject spawnEffect;
    public AudioClip spawnSound;
    public bool clonesCanAttack = true;

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
                user.transform.position + user.transform.forward * 2f,
                user.transform.rotation);
        }
        else
        {
            clone = Instantiate(user,
                user.transform.position + user.transform.forward * 2f,
                user.transform.rotation);
        }

        // Configura clone
        var health = clone.GetComponent<Health>();
        if (health != null)
        {
            health.maxHealth *= cloneHealthMultiplier;
            health.currentHealth = health.maxHealth;
        }

        var damage = clone.GetComponent<DamageDealer>();
        if (damage != null)
        {
            damage.damageMultiplier = cloneDamageMultiplier;
        }

        // Auto-destruição
        clone.AddComponent<CloneController>().Initialize(cloneLifetime, this);

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
    }

    public void RemoveClone(GameObject clone)
    {
        if (activeClones.Contains(clone))
        {
            activeClones.Remove(clone);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        // Remove todos os clones
        foreach (var clone in activeClones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        activeClones.Clear();
    }
}

public class CloneController : MonoBehaviour
{
    private float lifetime;
    private CloningPower power;
    private float timer;
    private bool isInitialized = false;

    public void Initialize(float lifetime, CloningPower power)
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
                power.RemoveClone(gameObject);
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (power != null)
        {
            power.RemoveClone(gameObject);
        }
    }
}