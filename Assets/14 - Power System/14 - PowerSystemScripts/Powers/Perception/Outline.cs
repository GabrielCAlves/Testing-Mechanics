// Outline.cs (Versão Simplificada)
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    [Header("Configurações")]
    public Color outlineColor = Color.cyan;
    public float outlineWidth = 2f;

    private Renderer objectRenderer;
    private Material outlineMaterial;
    private Material[] originalMaterials;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // Guarda materiais originais
            originalMaterials = objectRenderer.materials;

            // Cria material de outline
            outlineMaterial = new Material(Shader.Find("Sprites/Default"));
            outlineMaterial.color = outlineColor;
            outlineMaterial.SetFloat("_Outline", outlineWidth);

            // Aplica outline
            ApplyOutline();
        }
    }

    void ApplyOutline()
    {
        if (objectRenderer == null) return;

        // Adiciona o material de outline como um material extra
        Material[] newMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = originalMaterials[i];
        }
        newMaterials[newMaterials.Length - 1] = outlineMaterial;
        objectRenderer.materials = newMaterials;
    }

    void OnDestroy()
    {
        // Restaura materiais originais
        if (objectRenderer != null && originalMaterials != null)
        {
            objectRenderer.materials = originalMaterials;
        }

        if (outlineMaterial != null)
        {
            DestroyImmediate(outlineMaterial);
        }
    }
}