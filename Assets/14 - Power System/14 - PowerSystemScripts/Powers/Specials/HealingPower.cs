// HealingPower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealingPower", menuName = "Powers/Support/Healing Power")]
public class HealingPower : Power
{
    [Header("Configurações de Cura")]
    public float healAmount = 50f;
    public float healRadius = 5f;
    public bool healSelf = true;
    public bool healAllies = true;
    public LayerMask allyLayers;
    public GameObject healEffect;
    public GameObject healParticles;
    public AudioClip healSound;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ApplyHeal(user);
    }

    void ApplyHeal(GameObject user)
    {
        // Cura o próprio usuário
        if (healSelf)
        {
            var health = user.GetComponent<Health>();
            if (health != null)
            {
                health.Heal(healAmount);
            }
        }

        // Cura aliados na área
        if (healAllies)
        {
            Collider[] allies = Physics.OverlapSphere(user.transform.position, healRadius, allyLayers);

            foreach (var col in allies)
            {
                if (col.gameObject == user) continue;

                var health = col.GetComponent<Health>();
                if (health != null)
                {
                    health.Heal(healAmount);
                }

                // Efeito em aliados
                if (healParticles != null)
                {
                    Instantiate(healParticles, col.transform.position, Quaternion.identity);
                }
            }
        }

        // Efeito principal
        if (healEffect != null)
        {
            Instantiate(healEffect, user.transform.position, Quaternion.identity);
        }

        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, user.transform.position);
        }
    }
}