// ImmortalityPower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewImmortalityPower", menuName = "Powers/Defensive/Immortality Power")]
public class ImmortalityPower : Power
{
    [Header("Configurações de Imortalidade")]
    public float duration = 10f;
    public float healOnActivation = 50f;
    public bool autoRevive = true;
    public GameObject divineEffectPrefab;
    public AudioClip divineSound;
    public Color auraColor = Color.gold;

    private bool isImmortal = false;
    private float timer;
    private GameObject divineEffect;
    private Health health;
    private float originalHealthRegen;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        BecomeImmortal(user);
    }

    void BecomeImmortal(GameObject user)
    {
        isImmortal = true;
        timer = duration;

        health = user.GetComponent<Health>();
        if (health != null)
        {
            // Cura ao ativar
            health.Heal(healOnActivation);

            // Previne morte
            health.isImmortal = true;

            // Aumenta regeneração
            originalHealthRegen = health.regenRate;
            health.regenRate *= 2f;
        }

        // Efeito visual divino
        if (divineEffectPrefab != null)
        {
            divineEffect = Instantiate(divineEffectPrefab, user.transform);
            divineEffect.transform.localPosition = Vector3.zero;

            // Ajusta cor
            var renderer = divineEffect.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = auraColor;
            }

            // Adiciona luz
            var light = divineEffect.AddComponent<Light>();
            light.color = auraColor;
            light.intensity = 2f;
            light.range = 3f;
        }

        if (divineSound != null)
        {
            AudioSource.PlayClipAtPoint(divineSound, user.transform.position);
        }
    }

    public void UpdateImmortality(GameObject user)
    {
        if (!isImmortal) return;

        timer -= Time.deltaTime;

        // Efeito de brilho pulsante
        if (divineEffect != null)
        {
            float pulse = Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f;
            divineEffect.transform.localScale = Vector3.one * (1f + pulse * 0.2f);
        }

        if (timer <= 0)
        {
            Deactivate(user);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isImmortal = false;

        if (health != null)
        {
            health.isImmortal = false;
            health.regenRate = originalHealthRegen;
        }

        if (divineEffect != null)
        {
            Destroy(divineEffect);
        }
    }
}