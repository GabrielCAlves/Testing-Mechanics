using UnityEngine;

public class Temperature : MonoBehaviour
{
    [Header("Temperatura")]
    public float currentTemperature = 37f;
    public float maxTemperature = 100f;
    public float minTemperature = -20f;
    public float temperatureChangeRate = 0.5f;

    [Header("Efeitos")]
    public float damagePerSecond = 5f;
    public float thresholdDamage = 60f;
    public bool applyDamageOnExtremeTemperatures = true;
    public GameObject extremeTemperatureEffect;

    [Header("Regeneração")]
    public bool autoRegenerate = true;
    public float regenRate = 2f;
    public float regenDelay = 5f;

    private Health health;
    private float timeSinceLastChange = 0f;
    private float timeSinceExtremeTemp = 0f;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (autoRegenerate && timeSinceLastChange > regenDelay)
        {
            // Regenera para temperatura normal (37°C)
            if (currentTemperature < 37f)
                currentTemperature += regenRate * Time.deltaTime;
            else if (currentTemperature > 37f)
                currentTemperature -= regenRate * Time.deltaTime;
        }

        // Aplica dano em temperaturas extremas
        if (applyDamageOnExtremeTemperatures && health != null)
        {
            if (currentTemperature > thresholdDamage || currentTemperature < -thresholdDamage)
            {
                timeSinceExtremeTemp += Time.deltaTime;
                if (timeSinceExtremeTemp >= 1f)
                {
                    health.TakeDamage(damagePerSecond);
                    timeSinceExtremeTemp = 0f;
                }
            }
            else
            {
                timeSinceExtremeTemp = 0f;
            }
        }

        // Atualiza efeito visual
        if (extremeTemperatureEffect != null)
        {
            bool isExtreme = currentTemperature > thresholdDamage || currentTemperature < -thresholdDamage;
            extremeTemperatureEffect.SetActive(isExtreme);
        }
    }

    public void ChangeTemperature(float amount)
    {
        currentTemperature = Mathf.Clamp(currentTemperature + amount, minTemperature, maxTemperature);
        timeSinceLastChange = 0f;
    }

    public void SetTemperature(float temperature)
    {
        currentTemperature = Mathf.Clamp(temperature, minTemperature, maxTemperature);
    }

    public void Heat(float amount)
    {
        ChangeTemperature(amount);
    }

    public void Cool(float amount)
    {
        ChangeTemperature(-amount);
    }

    public float GetHeatPercentage()
    {
        return Mathf.InverseLerp(minTemperature, maxTemperature, currentTemperature);
    }
}