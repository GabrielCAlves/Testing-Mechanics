// GiantPower.cs
using SceneScript;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGiantPower", menuName = "Powers/Movement/Giant Power")]
public class GiantPower : Power
{
    [Header("Configurações de Gigante")]
    public float sizeMultiplier = 3f;
    public float massMultiplier = 5f;
    public float damageMultiplier = 2f;
    public float speedReduction = 0.5f;
    public GameObject growthEffect;
    public AudioClip growthSound;

    private Vector3 originalScale;
    private float originalMass;
    private float originalSpeed;
    private bool isGiant = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        Grow(user);
    }

    void Grow(GameObject user)
    {
        isGiant = true;

        // Guarda valores originais
        originalScale = user.transform.localScale;

        // Cresce
        user.transform.localScale *= sizeMultiplier;

        // Ajusta massa
        var rb = user.GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalMass = rb.mass;
            rb.mass *= massMultiplier;
        }

        // Reduz velocidade
        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            movement.moveSpeed *= speedReduction;
        }

        // Aumenta dano de ataques físicos
        var melee = user.GetComponent<MeleeCombat>();
        if (melee != null)
        {
            melee.damageMultiplier = damageMultiplier;
        }

        // Efeitos
        if (growthEffect != null)
        {
            Instantiate(growthEffect, user.transform.position, Quaternion.identity);
        }

        if (growthSound != null)
        {
            AudioSource.PlayClipAtPoint(growthSound, user.transform.position);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isGiant = false;

        // Restaura tamanho
        user.transform.localScale = originalScale;

        // Restaura massa
        var rb = user.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = originalMass;
        }

        // Restaura velocidade
        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
        }
    }
}