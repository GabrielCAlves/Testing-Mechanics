// SuperResistancePower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuperResistancePower", menuName = "Powers/Defensive/Super Resistance Power")]
public class SuperResistancePower : Power
{
    [Header("Configurações de Resistência")]
    public float damageReductionMultiplier = 0.2f;
    public float knockbackReductionMultiplier = 0.3f;
    public float stunReductionMultiplier = 0.5f;
    public GameObject shieldEffect;
    public Material shieldMaterial;

    private GameObject shieldObject;
    private Health health;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateResistance(user);
    }

    void ActivateResistance(GameObject user)
    {
        health = user.GetComponent<Health>();
        if (health != null)
        {
            health.damageMultiplier = damageReductionMultiplier;
            health.knockbackMultiplier = knockbackReductionMultiplier;
            health.stunMultiplier = stunReductionMultiplier;
        }

        // Escudo visual
        if (shieldEffect != null)
        {
            shieldObject = Instantiate(shieldEffect, user.transform);
            shieldObject.transform.localPosition = Vector3.zero;
            shieldObject.transform.localScale = Vector3.one * 1.2f;

            var renderer = shieldObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = shieldMaterial;
            }
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);

        if (health != null)
        {
            health.damageMultiplier = 1f;
            health.knockbackMultiplier = 1f;
            health.stunMultiplier = 1f;
        }

        if (shieldObject != null)
        {
            Destroy(shieldObject);
        }
    }
}