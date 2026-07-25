// FirePower.cs
using FreeflowCombatSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[CreateAssetMenu(fileName = "NewFirePower", menuName = "Powers/Elemental/Fire Power")]
public class FirePower : Power
{
    [Header("Configurações do Fogo")]
    public float fireDamage = 30f;
    public float burnDuration = 3f;
    public float fireRange = 10f;
    public GameObject fireProjectilePrefab;
    public Quaternion fireProjectileRotation;
    public float projectileSpeed = 20f;
    public GameObject fireEffectPrefab;
    public float heightOffset = 0.5f;

    private GameObject instantiatedPrefab;
    private Rigidbody rb;

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
                new Vector3(user.transform.position.x, user.transform.position.y+heightOffset, user.transform.position.z) + user.transform.forward * 1.5f,
                fireProjectileRotation != Quaternion.identity ? fireProjectileRotation : user.transform.rotation);

            instantiatedPrefab = projectile;

            rb = instantiatedPrefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = user.transform.forward * projectileSpeed;
            }

            var fireProjectile = projectile.AddComponent<FireProjectile>();
            fireProjectile.damage = fireDamage;
            fireProjectile.burnDuration = burnDuration;
            fireProjectile.owner = user;
            fireProjectile.speed = projectileSpeed;

            Destroy(projectile, 5f);
        }
    }

    //IEnumerator MoveProjectile(GameObject projectile, GameObject user)
    //{
    //    float elapsedTime = 0f;
    //    while (elapsedTime < fireRange / projectileSpeed)
    //    {
    //        if (projectile == null) yield break;
    //        projectile.transform.position += user.transform.forward * projectileSpeed * Time.deltaTime;
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //}
}

// Componente para o projétil
public class FireProjectile : MonoBehaviour
{
    public float damage;
    public float burnDuration;
    public GameObject owner;
    public float speed;

    private void Update()
    {
        transform.position += owner.transform.forward * speed * Time.deltaTime;
        Debug.Log("gameObject.name = " + gameObject.name);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision.gameObject.name = " + collision.gameObject.name);

        var health = collision.gameObject.GetComponent<Health>();
        if (health != null && collision.gameObject != owner)
        {
            health.TakeDamage(damage);
            health.ApplyBurn(damage * 0.5f, burnDuration);
        }

        Destroy(gameObject);
    }
}