// ElasticityPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewElasticityPower", menuName = "Powers/Movement/Elasticity Power")]
public class ElasticityPower : Power
{
    [Header("Configurações da Elasticidade")]
    public float bouncinessMultiplier = 3f;
    public float stretchFactor = 1.5f;
    public float elasticForce = 50f;
    public float maxStretchDuration = 2f;

    private Vector3 originalScale;
    private float currentStretch;
    private bool isCharging = false;
    private float chargeTime = 0f;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        StartCharge(user);
    }

    void StartCharge(GameObject user)
    {
        isCharging = true;
        chargeTime = 0f;
        originalScale = user.transform.localScale;
    }

    public void UpdateElasticity(GameObject user)
    {
        if (!isCharging) return;

        chargeTime += Time.deltaTime;
        float chargePercent = Mathf.Clamp01(chargeTime / maxStretchDuration);

        // Estica o personagem
        Vector3 stretchScale = originalScale;
        stretchScale.z *= 1 + (stretchFactor - 1) * chargePercent;
        stretchScale.x *= 1 - (stretchFactor - 1) * chargePercent * 0.3f;
        stretchScale.y *= 1 - (stretchFactor - 1) * chargePercent * 0.3f;

        user.transform.localScale = stretchScale;

        // Lança quando solta o botão
        if (Input.GetKeyUp(activationKey))
        {
            ReleaseElasticity(user, chargePercent);
        }
    }

    void ReleaseElasticity(GameObject user, float chargePercent)
    {
        isCharging = false;

        // Restaura escala
        user.transform.localScale = originalScale;

        // Aplica impulso
        var rb = user.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchForce = user.transform.forward * elasticForce * chargePercent;
            rb.AddForce(launchForce, ForceMode.Impulse);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isCharging = false;
        user.transform.localScale = originalScale;
    }
}