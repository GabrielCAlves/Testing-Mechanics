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
    private float scanInterval = 0.5f; // Escaneia a cada 0.5 segundos

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

        // Escaneia continuamente
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            ScanForTargets(user);
        }

        // Atualiza timers dos objetos revelados
        UpdateRevealedObjects(user);
    }

    void ScanForTargets(GameObject user)
    {
        // Encontra todos os alvos na área
        Collider[] targets = Physics.OverlapSphere(user.transform.position, visionRange, targetLayers);

        foreach (var col in targets)
        {
            if (col.gameObject == user) continue;

            // Verifica se está no cone de visão
            Vector3 direction = col.transform.position - user.transform.position;
            float angle = Vector3.Angle(user.transform.forward, direction);

            if (angle <= visionAngle / 2)
            {
                // Verifica se há linha de visão direta
                RaycastHit hit;
                if (Physics.Raycast(user.transform.position, direction, out hit, visionRange))
                {
                    GameObject target = hit.collider.gameObject;

                    // Se atingiu o alvo ou um objeto com tag Enemy
                    if (target == col.gameObject || target.CompareTag("Enemy"))
                    {
                        // Se o objeto já está revelado, renova o timer
                        if (revealedObjects.Contains(target))
                        {
                            revealTimers[target] = revealDuration;
                        }
                        else
                        {
                            // Revela o objeto
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

        // Salva materiais originais
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Material[] originalMats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMats[i] = renderers[i].material;

            // Aplica material de raio-x
            renderers[i].material = xrayMaterial;
            renderers[i].material.color = xrayColor;
            renderers[i].material.SetFloat("_Intensity", 1.5f);
        }

        // Armazena
        revealedObjects.Add(obj);
        originalMaterials[obj] = originalMats;
        revealTimers[obj] = revealDuration;

        // Adiciona outline
        AddOutline(obj);

        Debug.Log($"Objeto revelado: {obj.name}");
    }

    void AddOutline(GameObject obj)
    {
        if (obj == null) return;

        // Verifica se já tem outline
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
                    // Não está vendo diretamente o objeto
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

        // Restaura materiais originais
        if (originalMaterials.ContainsKey(obj))
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Material[] originals = originalMaterials[obj];

            for (int i = 0; i < renderers.Length && i < originals.Length; i++)
            {
                renderers[i].material = originals[i];
            }
        }

        // Remove outline
        var outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }

        // Remove das listas
        revealedObjects.Remove(obj);
        originalMaterials.Remove(obj);
        revealTimers.Remove(obj);

        Debug.Log($"Objeto não está mais revelado: {obj.name}");
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