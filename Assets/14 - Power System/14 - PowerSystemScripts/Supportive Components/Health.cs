// Health.cs
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Configurações de Saúde")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float regenRate = 5f;
    public float regenDelay = 3f;

    [Header("Status")]
    public bool isImmortal = false;
    public float damageMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public float stunMultiplier = 1f;

    private float lastDamageTime;
    private float freezeTimer = 0f;
    private bool isFrozen = false;
    private float poisonDamage;
    private float poisonTimer;
    private float poisonTickTimer;
    private float burnDamage;
    private float burnTimer;
    private float burnTickTimer;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                isFrozen = false;
            }
        }

        // Regeneração
        if (Time.time - lastDamageTime > regenDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + regenRate * Time.deltaTime, maxHealth);
        }

        // Veneno
        if (poisonTimer > 0)
        {
            poisonTimer -= Time.deltaTime;
            poisonTickTimer -= Time.deltaTime;
            if (poisonTickTimer <= 0)
            {
                TakeDamage(poisonDamage);
                poisonTickTimer = 1f;
            }
        }

        // Queimadura
        if (burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;
            burnTickTimer -= Time.deltaTime;
            if (burnTickTimer <= 0)
            {
                TakeDamage(burnDamage);
                burnTickTimer = 0.5f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isImmortal) return;

        float finalDamage = damage * damageMultiplier;
        currentHealth -= finalDamage;
        lastDamageTime = Time.time;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;
    }

    public void ApplyPoison(float damage, float duration, float tickRate)
    {
        poisonDamage = damage;
        poisonTimer = duration;
        poisonTickTimer = tickRate;
    }

    public void ApplyBurn(float damage, float duration)
    {
        burnDamage = damage;
        burnTimer = duration;
        burnTickTimer = 0.5f;
    }

    void Die()
    {
        // Implementar lógica de morte
        Debug.Log($"{gameObject.name} morreu!");
    }
}