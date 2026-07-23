// ForceFieldPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewForceFieldPower", menuName = "Powers/Defensive/Force Field Power")]
public class ForceFieldPower : Power
{
    [Header("Configurações do Campo de Força")]
    public float shieldHealth = 100f;
    public float shieldRadius = 3f;
    public float regenRate = 10f;
    public float regenDelay = 2f;
    public GameObject shieldPrefab;
    public Material shieldMaterial;
    public AudioClip shieldHitSound;

    private GameObject shieldObject;
    private float currentShieldHealth;
    private float lastHitTime;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        CreateShield(user);
    }

    void CreateShield(GameObject user)
    {
        isActive = true;
        currentShieldHealth = shieldHealth;
        lastHitTime = Time.time;

        if (shieldPrefab != null)
        {
            shieldObject = Instantiate(shieldPrefab, user.transform);
            shieldObject.transform.localPosition = Vector3.zero;
            shieldObject.transform.localScale = Vector3.one * shieldRadius;

            var renderer = shieldObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = shieldMaterial;
            }
        }
    }

    public void UpdateShield(GameObject user)
    {
        if (!isActive) return;

        // Regeneração
        if (Time.time - lastHitTime > regenDelay)
        {
            currentShieldHealth = Mathf.Min(currentShieldHealth + regenRate * Time.deltaTime, shieldHealth);
        }

        // Atualiza visual baseado na saúde
        if (shieldObject != null)
        {
            float healthPercent = currentShieldHealth / shieldHealth;
            shieldObject.transform.localScale = Vector3.one * shieldRadius * (0.5f + 0.5f * healthPercent);

            var renderer = shieldObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = 0.5f + 0.5f * healthPercent;
                renderer.material.color = color;
            }
        }
    }

    public bool AbsorbDamage(float damage)
    {
        if (!isActive) return false;

        currentShieldHealth -= damage;
        lastHitTime = Time.time;

        if (shieldHitSound != null)
        {
            AudioSource.PlayClipAtPoint(shieldHitSound, shieldObject.transform.position);
        }

        if (currentShieldHealth <= 0)
        {
            Deactivate(shieldObject);
            return false;
        }

        return true;
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        if (shieldObject != null)
        {
            Destroy(shieldObject);
        }
    }
}