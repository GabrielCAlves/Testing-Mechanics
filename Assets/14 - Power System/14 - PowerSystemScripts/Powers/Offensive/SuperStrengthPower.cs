// SuperStrengthPower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuperStrengthPower", menuName = "Powers/Offensive/Super Strength Power")]
public class SuperStrengthPower : Power
{
    [Header("Configurações da Super Força")]
    public float damage = 10f;
    public float damageMultiplier = 3f;
    public float knockbackForce = 20f;
    public float punchRange = 2f;
    public GameObject impactEffect;
    public AudioClip punchSound;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        PerformPunch(user);
    }

    void PerformPunch(GameObject user)
    {
        // Aplica força extra aos ataques corpo a corpo
        var melee = user.GetComponent<MeleeCombat>();
        if (melee != null)
        {
            melee.damageMultiplier = damageMultiplier;
            melee.knockbackMultiplier = knockbackForce;
        }

        // Verifica colisões próximas
        Collider[] hits = Physics.OverlapSphere(user.transform.position + user.transform.forward * 1f, punchRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject != user)
            {
                var rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (hit.transform.position - user.transform.position).normalized;
                    rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
                }

                var health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage * damageMultiplier);
                }
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, user.transform.position + user.transform.forward * 1.5f, Quaternion.identity);
        }
    }
}