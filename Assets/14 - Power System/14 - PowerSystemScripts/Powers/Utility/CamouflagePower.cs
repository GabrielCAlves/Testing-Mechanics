// CamouflagePower.cs
using SceneScript;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamouflagePower", menuName = "Powers/Utility/Camouflage Power")]
public class CamouflagePower : Power
{
    [Header("Configurações de Camuflagem")]
    public float duration = 15f;
    public float detectionReduction = 0.3f;
    public float movementPenalty = 0.7f;
    public string originalTag;
    public string camouflageTag;
    public Material camouflageMaterial;
    //public Color camouflageColor = new Color(0, 0.5f, 0, 0.5f);

    private bool isCamouflaged = false;
    private float timer;
    private Renderer[] renderers;
    private Material[] originalMaterials;
    private float originalSpeed;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ApplyCamouflage(user);
    }

    void ApplyCamouflage(GameObject user)
    {
        isCamouflaged = true;
        timer = duration;

        renderers = user.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // Aplica material de camuflagem
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            renderers[i].material = camouflageMaterial;
            //renderers[i].material.color = camouflageColor;
        }

        // Reduz velocidade
        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            movement.moveSpeed *= movementPenalty;
        }

        user.tag = camouflageTag;

        // Reduz detecção por inimigos
        //var enemyDetection = user.GetComponent<EnemyDetection>();
        //if (enemyDetection != null)
        //{
        //    enemyDetection.detectionRange *= detectionReduction;
        //}
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isCamouflaged) return;

        timer -= Time.deltaTime;

        // Efeito de cintilação
        if (renderers != null && camouflageMaterial != null)
        {
            float shimmer = Mathf.Sin(Time.time * 10f) * 0.1f + 0.9f;
            //Color color = camouflageColor;
            //color.a = color.a * shimmer;
            //camouflageMaterial.color = color;
        }

        if (timer <= 0)
        {
            Deactivate(user);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isCamouflaged = false;

        // Restaura materiais originais
        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length && i < originalMaterials.Length; i++)
            {
                renderers[i].material = originalMaterials[i];
            }
        }

        // Restaura velocidade
        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
        }

        user.tag = originalTag;

        // Restaura detecção
        //var enemyDetection = user.GetComponent<EnemyDetection>();
        //if (enemyDetection != null)
        //{
        //    enemyDetection.detectionRange /= detectionReduction;
        //}
    }
}