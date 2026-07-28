using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSuperHearingPower", menuName = "Powers/Perception/Super Hearing Power")]
public class SuperHearingPower : Power
{
    [Header("Configurações de Super Audição")]
    public float hearingRange = 30f;
    public float detectionMultiplier = 3f;
    public float whisperDetection = 5f;
    public LayerMask soundSources;

    [Header("Visualização")]
    public GameObject hearingVisualizer;  // Seu prefab SoundVisualizer
    public Color hearingColor = Color.green;
    public float visualizerDuration = 2f;
    public float visualizerMaxScale = 5f;

    [Header("Filtros de Som")]
    public bool detectFootsteps = true;
    public bool detectGunshots = true;
    public bool detectVoices = true;
    public bool detectExplosions = true;

    private bool isActive = false;
    private GameObject visualizerObject;
    private List<AudioSource> detectedSounds = new List<AudioSource>();
    private List<GameObject> activeVisualizers = new List<GameObject>();
    private float detectionTimer = 0f;
    private float detectionInterval = 0.5f; // Verifica a cada 0.5 segundos

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateSuperHearing(user);
    }

    void ActivateSuperHearing(GameObject user)
    {
        isActive = true;

        // Cria visualização (se tiver)
        if (hearingVisualizer != null)
        {
            visualizerObject = Instantiate(hearingVisualizer, user.transform);
            visualizerObject.transform.localPosition = Vector3.zero;

            // Configura a cor
            var renderer = visualizerObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = hearingColor;
            }

            // Ajusta escala
            visualizerObject.transform.localScale = Vector3.one * hearingRange / 10f;
        }

        // Detecta sons imediatamente
        DetectSounds(user);

        Debug.Log($"Super Audição Ativada - Alcance: {hearingRange}m");
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || user == null) return;

        // Atualiza a cada intervalo
        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            DetectSounds(user);
        }

        // Atualiza visualizadores ativos
        UpdateVisualizers();
    }

    void DetectSounds(GameObject user)
    {
        // Limpa lista anterior
        detectedSounds.Clear();

        // Encontra todas as fontes de som na cena
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var source in allAudioSources)
        {
            if (source.gameObject == user) continue; // Ignora o próprio player
            if (!source.isPlaying) continue; // Ignora sons parados

            // Verifica distância
            float distance = Vector3.Distance(user.transform.position, source.transform.position);
            if (distance > hearingRange) continue;

            // Verifica se o som é relevante (filtro por tags ou nome)
            if (!IsSoundRelevant(source)) continue;

            // Calcula intensidade baseada no volume e distância
            float intensity = source.volume * (1 - distance / hearingRange);

            // Adiciona à lista
            detectedSounds.Add(source);

            // Cria visualizador na posição do som
            CreateSoundVisualizer(source.transform.position, intensity, distance);

            // Debug
            Debug.Log($"Som detectado: {source.gameObject.name} - Distância: {distance:F1}m - Intensidade: {intensity:F2}");
        }
    }

    bool IsSoundRelevant(AudioSource source)
    {
        // Verifica por tags
        string tag = source.gameObject.tag;

        if (tag == "Footstep" && !detectFootsteps) return false;
        if (tag == "Gunshot" && !detectGunshots) return false;
        if (tag == "Voice" && !detectVoices) return false;
        if (tag == "Explosion" && !detectExplosions) return false;

        // Verifica por nome do GameObject
        string name = source.gameObject.name.ToLower();
        if (name.Contains("foot") && !detectFootsteps) return false;
        if (name.Contains("gun") && !detectGunshots) return false;
        if (name.Contains("voice") && !detectVoices) return false;
        if (name.Contains("explosion") && !detectExplosions) return false;

        return true;
    }

    void CreateSoundVisualizer(Vector3 position, float intensity, float distance)
    {
        if (hearingVisualizer == null) return;

        // Instancia o visualizador na posição do som
        GameObject visualizer = Instantiate(hearingVisualizer, position, Quaternion.identity);

        // Ajusta a cor baseado na intensidade
        var renderer = visualizer.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = hearingColor;
            // Quanto mais intenso, mais brilhante
            color.a = Mathf.Clamp01(intensity * 2f);
            renderer.material.color = color;
        }

        // Ajusta escala baseado na intensidade
        float scale = 1f + intensity * visualizerMaxScale;
        visualizer.transform.localScale = Vector3.one * scale;

        // Adiciona o script de auto-destruição se não tiver animação
        var soundVis = visualizer.GetComponent<SoundVisualizer>();
        if (soundVis == null)
        {
            // Adiciona um script simples para desvanecer e destruir
            var autoDestroy = visualizer.AddComponent<AutoDestroyVisualizer>();
            autoDestroy.duration = visualizerDuration;
            autoDestroy.maxScale = visualizerMaxScale;
        }

        // Adiciona à lista de visualizadores ativos
        activeVisualizers.Add(visualizer);

        // Remove visualizadores antigos se houver muitos
        if (activeVisualizers.Count > 20)
        {
            Destroy(activeVisualizers[0]);
            activeVisualizers.RemoveAt(0);
        }
    }

    void UpdateVisualizers()
    {
        // Remove visualizadores que já foram destruídos
        activeVisualizers.RemoveAll(v => v == null);
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Remove visualizador do player
        if (visualizerObject != null)
        {
            Destroy(visualizerObject);
            visualizerObject = null;
        }

        // Remove todos os visualizadores ativos
        foreach (var viz in activeVisualizers)
        {
            if (viz != null)
            {
                Destroy(viz);
            }
        }
        activeVisualizers.Clear();

        Debug.Log("Super Audição Desativada");
    }
}

//// SuperHearingPower.cs
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "NewSuperHearingPower", menuName = "Powers/Perception/Super Hearing Power")]
//public class SuperHearingPower : Power
//{
//    [Header("Configurações de Super Audição")]
//    public float hearingRange = 30f;
//    public float detectionMultiplier = 3f;
//    public float whisperDetection = 5f;
//    public LayerMask soundSources;
//    public GameObject hearingVisualizer;
//    public Color hearingColor = Color.green;

//    private bool isActive = false;
//    private GameObject visualizerObject;
//    private List<AudioSource> detectedSounds = new List<AudioSource>();

//    public override void Activate(GameObject user)
//    {
//        base.Activate(user);
//        ActivateSuperHearing(user);
//    }

//    void ActivateSuperHearing(GameObject user)
//    {
//        isActive = true;

//        // Cria visualização
//        if (hearingVisualizer != null)
//        {
//            visualizerObject = Instantiate(hearingVisualizer, user.transform);
//            visualizerObject.transform.localPosition = Vector3.zero;

//            var renderer = visualizerObject.GetComponent<Renderer>();
//            if (renderer != null)
//            {
//                renderer.material.color = hearingColor;
//            }
//        }

//        // Detecta sons
//        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
//        foreach (var source in audioSources)
//        {
//            if (source.gameObject != user && source.isPlaying)
//            {
//                float distance = Vector3.Distance(user.transform.position, source.transform.position);
//                if (distance <= hearingRange)
//                {
//                    detectedSounds.Add(source);

//                    // Visualização de som
//                    CreateSoundVisualizer(source.transform.position, distance);
//                }
//            }
//        }
//    }

//    void CreateSoundVisualizer(Vector3 position, float distance)
//    {
//        // Cria indicador visual do som
//        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        indicator.transform.position = position;
//        indicator.transform.localScale = Vector3.one * (distance / hearingRange) * 2f;
//        indicator.GetComponent<Renderer>().material.color = hearingColor;

//        // Adiciona fade out
//        indicator.AddComponent<SoundVisualizer>().Initialize(2f);
//    }

//    public override void Deactivate(GameObject user)
//    {
//        base.Deactivate(user);
//        isActive = false;

//        if (visualizerObject != null)
//        {
//            Destroy(visualizerObject);
//        }

//        // Remove visualizadores de som
//        SoundVisualizer[] visualizers = FindObjectsOfType<SoundVisualizer>();
//        foreach (var viz in visualizers)
//        {
//            Destroy(viz.gameObject);
//        }
//    }
//}

//public class SoundVisualizer : MonoBehaviour
//{
//    private float lifetime;
//    private float timer;

//    public void Initialize(float lifetime)
//    {
//        this.lifetime = lifetime;
//        timer = 0f;
//    }

//    void Update()
//    {
//        timer += Time.deltaTime;
//        float alpha = 1f - (timer / lifetime);

//        Color color = GetComponent<Renderer>().material.color;
//        color.a = alpha;
//        GetComponent<Renderer>().material.color = color;

//        transform.localScale += Vector3.one * Time.deltaTime * 2f;

//        if (timer >= lifetime)
//        {
//            Destroy(gameObject);
//        }
//    }
//}