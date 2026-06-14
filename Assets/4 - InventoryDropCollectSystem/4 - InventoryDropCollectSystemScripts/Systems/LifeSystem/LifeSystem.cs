using UnityEngine;
using System;
using System.Collections.Generic;
using Cainos.LucidEditor;

public class LifeSystem : MonoBehaviour // Don't forget the EventSystem canvas gameobject added, otherwise the events won't work, and the health bars won't update.]
{
    public int maxHealth = 100;
    public int currentHealth;

    public event Action OnDeath;

    [Header("Health Bars")]
    public LifeBar healthBar; // One Life Bar
    public MultipleLifeBars multipleLifeBar; // Multiple Life Bars

    [Header("Animations")]
    [SerializeField] private Animator animator;
    private const string ANIM_TAKEHIT = "TakeHit";
    private const string ANIM_DIE = "Die";

    void Start()
    {
        currentHealth = maxHealth;

        // Inicializa a LifeBar original
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.UpdateBar(currentHealth);
        }

        // Inicializa a MultipleLifeBars
        if (multipleLifeBar != null)
        {
            multipleLifeBar.SetMaxHealth(maxHealth);
            multipleLifeBar.UpdateBar(currentHealth);
        }

        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (animator != null)
            animator.SetTrigger(ANIM_TAKEHIT);

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
            if (animator != null)
                animator.SetTrigger(ANIM_DIE);

            Destroy(gameObject, 2f);
        }
    }
}