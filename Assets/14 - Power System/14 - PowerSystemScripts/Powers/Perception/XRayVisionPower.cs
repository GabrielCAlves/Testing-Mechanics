using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewXRayVisionPower", menuName = "Powers/Perception/X-Ray Vision Power")]
public class XRayVisionPower : Power
{
    [Header("Configurações de Visão de Raio-X")]
    public float visionRange = 20f;
    public float visionAngle = 60f;
    public LayerMask targetLayers;
    public Material xrayMaterial;
    public Color xrayColor = new Color(0, 1, 0, 0.3f);
    public float revealDuration = 5f;
    public AudioClip xraySound;

    [Header("Efeitos Adicionais")]
    public float outlineWidth = 2f;
    public Color outlineColor = Color.cyan;

    private bool isActive = false;
    private List<GameObject> revealedObjects = new List<GameObject>();
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();
    private Dictionary<GameObject, float> revealTimers = new Dictionary<GameObject, float>();
    private float scanTimer = 0f;
    private float scanInterval = 0.5f;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;

        if (xraySound != null)
        {
            AudioSource.PlayClipAtPoint(xraySound, user.transform.position);
        }

        Debug.Log("Raio-X Ativado");
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || user == null) return;

        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            ScanForTargets(user);
        }

        UpdateRevealedObjects(user);
    }

    void ScanForTargets(GameObject user)
    {
        // --- CORREÇÃO: Usa Physics.OverlapSphere com a LayerMask correta ---
        Collider[] targets = Physics.OverlapSphere(user.transform.position, visionRange, targetLayers);

        Debug.Log($"Escaneando... Encontrados {targets.Length} objetos na camada {targetLayers.value}");

        foreach (var col in targets)
        {
            if (col.gameObject == user) continue;

            Vector3 direction = col.transform.position - user.transform.position;
            float angle = Vector3.Angle(user.transform.forward, direction);

            if (angle <= visionAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(user.transform.position, direction, out hit, visionRange))
                {
                    GameObject target = hit.collider.gameObject;

                    if (target == col.gameObject || target.CompareTag("Enemy"))
                    {
                        if (revealedObjects.Contains(target))
                        {
                            revealTimers[target] = revealDuration;
                        }
                        else
                        {
                            RevealObject(target);
                        }
                    }
                }
            }
        }
    }

    void RevealObject(GameObject obj)
    {
        if (obj == null) return;
        if (revealedObjects.Contains(obj)) return;
        if (xrayMaterial == null) return;

        // --- CORREÇÃO: Salva os materiais ORIGINAIS antes de modificar ---
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Material[] originalMats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Salva o material original
            originalMats[i] = renderers[i].material;

            // Cria uma instância do material para não afetar outros objetos
            Material newMat = new Material(xrayMaterial);
            newMat.color = xrayColor;

            // Aplica o material de raio-x
            renderers[i].material = newMat;
        }

        // Armazena
        revealedObjects.Add(obj);
        originalMaterials[obj] = originalMats;
        revealTimers[obj] = revealDuration;

        // Adiciona outline
        AddOutline(obj);

        Debug.Log($"Objeto revelado: {obj.name} - Materiais salvos: {originalMats.Length}");
    }

    void AddOutline(GameObject obj)
    {
        if (obj == null) return;

        var outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
            outline.outlineColor = outlineColor;
            outline.outlineWidth = outlineWidth;
        }
    }

    void UpdateRevealedObjects(GameObject user)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var obj in revealedObjects)
        {
            if (obj == null)
            {
                toRemove.Add(obj);
                continue;
            }

            // Verifica se ainda está no alcance
            float distance = Vector3.Distance(user.transform.position, obj.transform.position);
            if (distance > visionRange)
            {
                toRemove.Add(obj);
                continue;
            }

            // Verifica se ainda está no cone de visão
            Vector3 direction = obj.transform.position - user.transform.position;
            float angle = Vector3.Angle(user.transform.forward, direction);
            if (angle > visionAngle / 2)
            {
                toRemove.Add(obj);
                continue;
            }

            // Verifica se há obstáculo
            RaycastHit hit;
            if (Physics.Raycast(user.transform.position, direction, out hit, visionRange))
            {
                if (hit.collider.gameObject != obj)
                {
                    toRemove.Add(obj);
                    continue;
                }
            }

            // Atualiza timer
            revealTimers[obj] -= Time.deltaTime;
            if (revealTimers[obj] <= 0)
            {
                toRemove.Add(obj);
            }
        }

        // Remove objetos expirados
        foreach (var obj in toRemove)
        {
            UnrevealObject(obj);
        }
    }

    public void UnrevealObject(GameObject obj)
    {
        if (!revealedObjects.Contains(obj)) return;

        if (obj == null)
        {
            revealedObjects.Remove(obj);
            return;
        }

        // --- CORREÇÃO: Restaura os materiais ORIGINAIS ---
        if (originalMaterials.ContainsKey(obj))
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (originalMaterials[obj] != null && renderers.Length > 0)
            {
                Material[] originals = originalMaterials[obj];

                for (int i = 0; i < renderers.Length && i < originals.Length; i++)
                {
                    if (renderers[i] != null && originals[i] != null)
                    {
                        // Restaura o material original
                        renderers[i].material = originals[i];
                    }
                }
            }
        }

        // Remove outline
        var outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            DestroyImmediate(outline);
        }

        // Remove das listas
        revealedObjects.Remove(obj);
        originalMaterials.Remove(obj);
        revealTimers.Remove(obj);

        Debug.Log($"Objeto restaurado: {obj.name}");
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Restaura todos os objetos revelados
        List<GameObject> toRemove = new List<GameObject>(revealedObjects);
        foreach (var obj in toRemove)
        {
            UnrevealObject(obj);
        }

        Debug.Log("Raio-X Desativado");
    }
}