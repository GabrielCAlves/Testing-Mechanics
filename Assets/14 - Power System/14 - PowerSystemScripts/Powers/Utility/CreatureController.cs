using UnityEngine;

public class CreatureController : MonoBehaviour
{
    private float lifetime;
    private CreatureSummonPower power;
    private float timer;
    private bool isInitialized = false;
    private Health health;
    private GameObject character; // Referência ao character (filho ou o próprio container)

    public void Initialize(float lifetime, CreatureSummonPower power, GameObject character = null)
    {
        this.lifetime = lifetime;
        this.power = power;
        this.character = character != null ? character : gameObject;
        timer = 0f;
        isInitialized = true;

        // Pega o Health do character
        health = this.character.GetComponent<Health>();
    }

    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;

        // Verifica se o character está morto
        if (health != null && health.currentHealth <= 0)
        {
            if (power != null)
            {
                power.RemoveCreature(gameObject);
            }
            Destroy(gameObject);
            return;
        }

        if (timer >= lifetime)
        {
            if (power != null)
            {
                power.RemoveCreature(gameObject);
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (power != null)
        {
            power.RemoveCreature(gameObject);
        }
    }
}