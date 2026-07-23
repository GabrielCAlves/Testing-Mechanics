// XRayVisionPower.cs
using System.Collections.Generic;
using UnityEngine;

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

    private bool isActive = false;
    private List<GameObject> revealedObjects = new List<GameObject>();
    private Material[] originalMaterials;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateXRay(user);
    }

    void ActivateXRay(GameObject user)
    {
        isActive = true;

        if (xraySound != null)
        {
            AudioSource.PlayClipAtPoint(xraySound, user.transform.position);
        }

        // Encontra objetos na visão
        Collider[] targets = Physics.OverlapSphere(user.transform.position, visionRange, targetLayers);

        foreach (var col in targets)
        {
            // Verifica se está no cone de visão
            Vector3 direction = col.transform.position - user.transform.position;
            float angle = Vector3.Angle(user.transform.forward, direction);

            if (angle <= visionAngle / 2)
            {
                // Verifica se há obstáculos
                RaycastHit hit;
                if (Physics.Raycast(user.transform.position, direction, out hit, visionRange))
                {
                    if (hit.collider.gameObject == col.gameObject || hit.collider.CompareTag("Enemy"))
                    {
                        RevealObject(col.gameObject);
                    }
                }
            }
        }
    }

    void RevealObject(GameObject obj)
    {
        // Salva materiais originais e aplica raio-x
        var renderers = obj.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            renderers[i].material = xrayMaterial;
            renderers[i].material.color = xrayColor;
        }

        revealedObjects.Add(obj);
        obj.AddComponent<RevealObject>().Initialize(revealDuration, this);
    }

    public void UnrevealObject(GameObject obj)
    {
        if (revealedObjects.Contains(obj))
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length && i < originalMaterials.Length; i++)
            {
                renderers[i].material = originalMaterials[i];
            }
            revealedObjects.Remove(obj);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Restaura todos os objetos revelados
        foreach (var obj in revealedObjects)
        {
            if (obj != null)
            {
                UnrevealObject(obj);
            }
        }
        revealedObjects.Clear();
    }
}

public class RevealObject : MonoBehaviour
{
    private float revealDuration;
    private XRayVisionPower power;
    private float timer;

    public void Initialize(float duration, XRayVisionPower power)
    {
        this.revealDuration = duration;
        this.power = power;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= revealDuration)
        {
            if (power != null)
            {
                power.UnrevealObject(gameObject);
            }
            Destroy(this);
        }
    }
}