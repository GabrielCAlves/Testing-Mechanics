// HeatVisionPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeatVisionPower", menuName = "Powers/Perception/Heat Vision Power")]
public class HeatVisionPower : Power
{
    [Header("Configurações de Visão de Calor")]
    public float heatVisionRange = 15f;
    public float heatDetectionThreshold = 30f;
    public Color heatColor = Color.red;
    public Color coldColor = Color.blue;
    public Material heatVisionMaterial;
    public bool showHeatSources = true;

    private bool isActive = false;
    private Texture2D heatTexture;
    private Renderer[] renderers;
    private Material[] originalMaterials;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateHeatVision(user);
    }

    void ActivateHeatVision(GameObject user)
    {
        isActive = true;

        // Encontra todos os objetos na área
        Collider[] objects = Physics.OverlapSphere(user.transform.position, heatVisionRange);

        foreach (var col in objects)
        {
            if (col.gameObject == user) continue;

            // Verifica temperatura
            var temperature = col.GetComponent<Temperature>();
            if (temperature != null)
            {
                float heatValue = temperature.currentTemperature / heatDetectionThreshold;
                ApplyHeatVision(col.gameObject, heatValue);
            }
            else if (showHeatSources)
            {
                // Mostra como fonte de calor
                ApplyHeatVision(col.gameObject, 1f);
            }
        }
    }

    void ApplyHeatVision(GameObject obj, float heatValue)
    {
        renderers = obj.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            renderers[i].material = heatVisionMaterial;

            // Gradiente de cor baseado na temperatura
            Color color = Color.Lerp(coldColor, heatColor, heatValue);
            renderers[i].material.color = color;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Restaura materiais originais
        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length && i < originalMaterials.Length; i++)
            {
                renderers[i].material = originalMaterials[i];
            }
        }
    }
}