using UnityEngine;

public class CloneController : MonoBehaviour
{
    private float lifetime;
    private CloningPower power;
    private float timer;
    private bool isInitialized = false;

    public void Initialize(float lifetime, CloningPower power)
    {
        this.lifetime = lifetime;
        this.power = power;
        timer = 0f;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            DestroyClone();
        }
    }

    void DestroyClone()
    {
        if (power != null)
        {
            power.RemoveClone(gameObject);
        }

        // Efeito de morte (opcional)
        // Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (power != null)
        {
            power.RemoveClone(gameObject);
        }
    }
}