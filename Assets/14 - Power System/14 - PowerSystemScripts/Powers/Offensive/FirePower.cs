// FirePower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFirePower", menuName = "Powers/Elemental/Fire Power")]
public class FirePower : Power
{
    [Header("Configurações do Fogo")]
    public float fireDamage = 30f;
    public float burnDuration = 3f;
    public float fireRange = 10f;
    public GameObject fireProjectilePrefab;
    public float projectileSpeed = 20f;
    public GameObject fireEffectPrefab;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ShootFire(user);
    }

    void ShootFire(GameObject user)
    {
        if (fireProjectilePrefab != null)
        {
            GameObject projectile = Instantiate(fireProjectilePrefab,
                user.transform.position + user.transform.forward * 1.5f,
                user.transform.rotation);

            var rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = user.transform.forward * projectileSpeed;
            }

            var fireProjectile = projectile.AddComponent<FireProjectile>();
            fireProjectile.damage = fireDamage;
            fireProjectile.burnDuration = burnDuration;
            fireProjectile.owner = user;

            Destroy(projectile, 5f);
        }
    }
}

// Componente para o projétil
public class FireProjectile : MonoBehaviour
{
    public float damage;
    public float burnDuration;
    public GameObject owner;

    void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null && collision.gameObject != owner)
        {
            health.TakeDamage(damage);
            health.ApplyBurn(damage * 0.5f, burnDuration);
        }

        Destroy(gameObject);
    }
}