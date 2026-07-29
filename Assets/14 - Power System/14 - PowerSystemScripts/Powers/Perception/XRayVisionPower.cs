using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewXRayVisionPower", menuName = "Powers/Perception/X-Ray Vision Power")]
public class XRayVisionPower : Power
{
    [Header("Configurações de Visão de Raio-X")]
    public float visionRange = 20f;
    public float visionAngle = 60f;
    public float revealDuration = 5f;
    public AudioClip xraySound;

    [Header("Configurações de Layer (SwitchLayer)")]
    public LayerMask defaultLayer;
    public LayerMask xRayLayer;

    private bool isActive = false;
    private List<GameObject> revealedObjects = new List<GameObject>();
    private Dictionary<GameObject, float> revealTimers = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();
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
        // Usa as layers definidas no Inspector
        Collider[] targets = Physics.OverlapSphere(user.transform.position, visionRange, defaultLayer);

        foreach (var col in targets)
        {
            if (col.gameObject == user) continue;

            Vector3 direction = col.transform.position - user.transform.position;
            float angle = Vector3.Angle(user.transform.forward, direction);

            if (angle <= visionAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(user.transform.position, direction, out hit, visionRange, defaultLayer))
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

        // --- SWITCH LAYER: SALVA E ALTERA ---
        if (xRayLayer.value != 0)
        {
            int xRayLayerNum = (int)Mathf.Log(xRayLayer.value, 2);

            // Salva a layer original
            originalLayers[obj] = obj.layer;

            // Altera a layer do objeto
            obj.layer = xRayLayerNum;

            // Altera a layer de todos os filhos
            SetLayerAllChildren(obj.transform, xRayLayerNum);

            Debug.Log($"Layer alterada: {obj.name} -> {LayerMask.LayerToName(xRayLayerNum)}");
        }

        // Armazena o objeto revelado
        revealedObjects.Add(obj);
        revealTimers[obj] = revealDuration;

        Debug.Log($"Objeto revelado: {obj.name}");
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

        // --- RESTAURA LAYER ORIGINAL ---
        if (originalLayers.ContainsKey(obj))
        {
            int originalLayer = originalLayers[obj];
            obj.layer = originalLayer;

            // Restaura layer de todos os filhos
            SetLayerAllChildren(obj.transform, originalLayer);

            Debug.Log($"Layer restaurada: {obj.name} -> {LayerMask.LayerToName(originalLayer)}");
            originalLayers.Remove(obj);
        }

        // Remove das listas
        revealedObjects.Remove(obj);
        revealTimers.Remove(obj);

        Debug.Log($"Objeto restaurado: {obj.name}");
    }

    private void SetLayerAllChildren(Transform parent, int layer)
    {
        var children = parent.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
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