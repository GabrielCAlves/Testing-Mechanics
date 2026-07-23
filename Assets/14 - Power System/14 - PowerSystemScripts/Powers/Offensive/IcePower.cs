// IcePower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIcePower", menuName = "Powers/Elemental/Ice Power")]
public class IcePower : Power
{
    [Header("Configurações do Gelo")]
    public float freezeDuration = 3f;
    public float freezeRadius = 5f;
    public float iceDamage = 20f;
    public GameObject iceEffectPrefab;
    public GameObject iceProjectilePrefab;
    public float projectileSpeed = 15f;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ShootIce(user);
    }

    void ShootIce(GameObject user)
    {
        if (iceProjectilePrefab != null)
        {
            GameObject projectile = Instantiate(iceProjectilePrefab,
                user.transform.position + user.transform.forward * 1.5f,
                user.transform.rotation);

            var rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = user.transform.forward * projectileSpeed;
            }

            var iceProj = projectile.AddComponent<IceProjectile>();
            iceProj.damage = iceDamage;
            iceProj.freezeDuration = freezeDuration;
            iceProj.freezeRadius = freezeRadius;
            iceProj.owner = user;

            Destroy(projectile, 5f);
        }
    }
}

public class IceProjectile : MonoBehaviour
{
    public float damage;
    public float freezeDuration;
    public float freezeRadius;
    public GameObject owner;
    public GameObject iceEffectPrefab;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != owner)
        {
            // Dano e congelamento
            var health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                health.Freeze(freezeDuration);
            }

            // Área de congelamento
            Collider[] nearby = Physics.OverlapSphere(transform.position, freezeRadius);
            foreach (var col in nearby)
            {
                if (col.gameObject != owner && col.gameObject != collision.gameObject)
                {
                    var nearbyHealth = col.GetComponent<Health>();
                    if (nearbyHealth != null)
                    {
                        nearbyHealth.Freeze(freezeDuration * 0.5f);
                    }
                }
            }

            if (iceEffectPrefab != null)
            {
                Instantiate(iceEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}