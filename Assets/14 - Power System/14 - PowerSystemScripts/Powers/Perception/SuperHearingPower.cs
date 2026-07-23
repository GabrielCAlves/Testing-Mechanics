// SuperHearingPower.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuperHearingPower", menuName = "Powers/Perception/Super Hearing Power")]
public class SuperHearingPower : Power
{
    [Header("Configurações de Super Audição")]
    public float hearingRange = 30f;
    public float detectionMultiplier = 3f;
    public float whisperDetection = 5f;
    public LayerMask soundSources;
    public GameObject hearingVisualizer;
    public Color hearingColor = Color.green;

    private bool isActive = false;
    private GameObject visualizerObject;
    private List<AudioSource> detectedSounds = new List<AudioSource>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateSuperHearing(user);
    }

    void ActivateSuperHearing(GameObject user)
    {
        isActive = true;

        // Cria visualização
        if (hearingVisualizer != null)
        {
            visualizerObject = Instantiate(hearingVisualizer, user.transform);
            visualizerObject.transform.localPosition = Vector3.zero;

            var renderer = visualizerObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = hearingColor;
            }
        }

        // Detecta sons
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var source in audioSources)
        {
            if (source.gameObject != user && source.isPlaying)
            {
                float distance = Vector3.Distance(user.transform.position, source.transform.position);
                if (distance <= hearingRange)
                {
                    detectedSounds.Add(source);

                    // Visualização de som
                    CreateSoundVisualizer(source.transform.position, distance);
                }
            }
        }
    }

    void CreateSoundVisualizer(Vector3 position, float distance)
    {
        // Cria indicador visual do som
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.transform.position = position;
        indicator.transform.localScale = Vector3.one * (distance / hearingRange) * 2f;
        indicator.GetComponent<Renderer>().material.color = hearingColor;

        // Adiciona fade out
        indicator.AddComponent<SoundVisualizer>().Initialize(2f);
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        if (visualizerObject != null)
        {
            Destroy(visualizerObject);
        }

        // Remove visualizadores de som
        SoundVisualizer[] visualizers = FindObjectsOfType<SoundVisualizer>();
        foreach (var viz in visualizers)
        {
            Destroy(viz.gameObject);
        }
    }
}

public class SoundVisualizer : MonoBehaviour
{
    private float lifetime;
    private float timer;

    public void Initialize(float lifetime)
    {
        this.lifetime = lifetime;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = 1f - (timer / lifetime);

        Color color = GetComponent<Renderer>().material.color;
        color.a = alpha;
        GetComponent<Renderer>().material.color = color;

        transform.localScale += Vector3.one * Time.deltaTime * 2f;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}