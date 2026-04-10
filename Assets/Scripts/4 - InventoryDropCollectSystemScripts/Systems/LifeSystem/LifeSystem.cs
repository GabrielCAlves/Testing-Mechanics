using UnityEngine;
using System;
using System.Collections.Generic;

public class LifeSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public event Action OnDeath;

    [Header("Health Bars")]
    public LifeBar healthBar; // Mantido o original
    public MultipleLifeBars multipleLifeBar; // Adicionado o novo MultipleLifeBars

    void Start()
    {
        currentHealth = maxHealth;

        // Inicializa a LifeBar original (se existir)
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.UpdateBar(currentHealth);
        }

        // Inicializa a MultipleLifeBars (se existir)
        if (multipleLifeBar != null)
        {
            multipleLifeBar.SetMaxHealth(maxHealth);
            multipleLifeBar.UpdateBar(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Atualiza a LifeBar original (se existir)
        if (healthBar != null)
        {
            healthBar.UpdateBar(currentHealth);
            //healthBar.DamageAnimation();
        }

        // Atualiza a MultipleLifeBars (se existir)
        if (multipleLifeBar != null)
        {
            multipleLifeBar.UpdateBar(currentHealth);
            //multipleLifeBar.DamageAnimation();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Atualiza a LifeBar original (se existir)
        if (healthBar != null)
            healthBar.UpdateBar(currentHealth);

        // Atualiza a MultipleLifeBars (se existir)
        if (multipleLifeBar != null)
            multipleLifeBar.UpdateBar(currentHealth);
    }

    void Die()
    {
        OnDeath?.Invoke();

        IDamageable damageable = GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Die();
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }
}