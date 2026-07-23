using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public float baseDamage = 10f;
    public float damageMultiplier = 1f;
    public bool canDealDamage = true;
    public float damageCooldown = 0.5f;

    [Header("Elemento (para ElementalControlPower)")]
    public ElementalControlPower.Element elementType = ElementalControlPower.Element.Fire;
    public float elementalMultiplier = 1f;

    [Header("Efeitos")]
    public GameObject hitEffect;
    public AudioClip hitSound;
    public LayerMask targetLayers;

    private float cooldownTimer = 0f;
    private GameObject lastTarget;

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public void DealDamage(GameObject target)
    {
        if (!canDealDamage || cooldownTimer > 0 || target == null)
            return;

        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            float totalDamage = baseDamage * damageMultiplier * elementalMultiplier;
            health.TakeDamage(totalDamage);

            cooldownTimer = damageCooldown;
            lastTarget = target;

            // Efeitos
            if (hitEffect != null)
            {
                Instantiate(hitEffect, target.transform.position, Quaternion.identity);
            }
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, target.transform.position);
            }

            Debug.Log($"{gameObject.name} causou {totalDamage} de dano ({elementType}) em {target.name}");
        }
    }

    public void SetElement(ElementalControlPower.Element element)
    {
        elementType = element;
    }

    public void SetMultiplier(float multiplier)
    {
        elementalMultiplier = multiplier;
    }

    void OnTriggerEnter(Collider other)
    {
        // Para uso com projéteis ou áreas de dano
        if (other.gameObject != gameObject)
        {
            DealDamage(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Para uso com projéteis ou ataques físicos
        if (collision.gameObject != gameObject)
        {
            DealDamage(collision.gameObject);
        }
    }
}