using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewDarkVisionPower", menuName = "Powers/Perception/Dark Vision Power")]
public class DarkVisionPower : Power
{
    [Header("Configurações de Visão no Escuro")]
    public float visionRange = 20f;
    public float visionIntensity = 1.5f;
    public Color visionColor = new Color(0.2f, 1f, 0.2f, 0.3f);
    public AudioClip activateSound;

    [Header("Configurações de Iluminação")]
    public float lightIntensity = 3f;
    public float lightRange = 15f;
    public float lightSpotAngle = 60f;
    public Color lightColor = Color.white;

    [Header("Efeito de Overlay (Recomendado)")]
    public Texture2D overlayTexture;
    public Color overlayColor = new Color(0, 0.5f, 0, 0.2f);

    [Header("Efeito de Pós-Processamento (Opcional)")]
    public bool usePostProcessing = false;

    private bool isActive = false;
    private Light playerLight;
    private float originalLightIntensity;
    private float originalLightRange;
    private float originalLightSpotAngle;
    private Color originalLightColor;
    private GameObject overlayObject;
    private Camera mainCamera;
    private NightVisionPostProcess postProcess;
    private float originalAmbientIntensity;
    private Color originalAmbientLight;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateDarkVision(user);
    }

    void ActivateDarkVision(GameObject user)
    {
        isActive = true;
        mainCamera = Camera.main;

        // --- 1. LUZ NO PLAYER ---
        playerLight = user.GetComponentInChildren<Light>();
        if (playerLight == null)
        {
            GameObject lightObj = new GameObject("DarkVisionLight");
            lightObj.transform.SetParent(user.transform);
            lightObj.transform.localPosition = Vector3.up * 1.5f + Vector3.forward * 0.5f;
            playerLight = lightObj.AddComponent<Light>();
            playerLight.type = LightType.Spot;
        }

        originalLightIntensity = playerLight.intensity;
        originalLightRange = playerLight.range;
        originalLightSpotAngle = playerLight.spotAngle;
        originalLightColor = playerLight.color;

        playerLight.intensity = lightIntensity;
        playerLight.range = lightRange;
        playerLight.spotAngle = lightSpotAngle;
        playerLight.color = lightColor;
        playerLight.shadows = LightShadows.Soft;

        // --- 2. AUMENTA A ILUMINAÇÃO AMBIENTE ---
        originalAmbientIntensity = RenderSettings.ambientIntensity;
        originalAmbientLight = RenderSettings.ambientLight;

        RenderSettings.ambientIntensity = visionIntensity;
        RenderSettings.ambientLight = visionColor;

        // --- 3. OVERLAY (SEMPRE FUNCIONA) ---
        CreateOverlay();

        // --- 4. PÓS-PROCESSAMENTO (Opcional) ---
        if (usePostProcessing && mainCamera != null)
        {
            postProcess = mainCamera.GetComponent<NightVisionPostProcess>();
            if (postProcess == null)
            {
                postProcess = mainCamera.gameObject.AddComponent<NightVisionPostProcess>();
            }
            postProcess.Activate(visionColor, visionIntensity);
        }

        // --- 5. EFEITO SONORO ---
        if (activateSound != null)
        {
            AudioSource.PlayClipAtPoint(activateSound, user.transform.position);
        }

        Debug.Log($"Dark Vision Ativada - Usando Pós-Processamento: {usePostProcessing}");
    }

    private void CreateOverlay()
    {
        if (overlayObject != null) return;

        // Cria um GameObject para o overlay
        overlayObject = new GameObject("DarkVisionOverlay");

        // Adiciona Canvas
        Canvas canvas = overlayObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        // Adiciona a imagem
        RawImage rawImage = overlayObject.AddComponent<RawImage>();

        // Se tiver textura, usa ela
        if (overlayTexture != null)
        {
            rawImage.texture = overlayTexture;
        }

        rawImage.color = overlayColor;
        rawImage.raycastTarget = false;

        // Configura o RectTransform
        RectTransform rect = overlayObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        Debug.Log("Overlay de visão noturna criado");
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || user == null) return;

        // Mantém a luz seguindo o player
        if (playerLight != null)
        {
            playerLight.transform.position = user.transform.position + Vector3.up * 1.5f + user.transform.forward * 0.5f;
            playerLight.transform.rotation = user.transform.rotation;
        }

        // Efeito de pulsação no overlay
        if (overlayObject != null)
        {
            RawImage rawImage = overlayObject.GetComponent<RawImage>();
            if (rawImage != null)
            {
                float pulse = Mathf.Sin(Time.time * 0.5f) * 0.05f + 0.95f;
                Color color = overlayColor;
                color.a = color.a * pulse;
                rawImage.color = color;
            }
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // --- RESTAURA LUZ ---
        if (playerLight != null)
        {
            playerLight.intensity = originalLightIntensity;
            playerLight.range = originalLightRange;
            playerLight.spotAngle = originalLightSpotAngle;
            playerLight.color = originalLightColor;
            playerLight.shadows = LightShadows.None;
        }

        // --- RESTAURA ILUMINAÇÃO AMBIENTE ---
        RenderSettings.ambientIntensity = originalAmbientIntensity;
        RenderSettings.ambientLight = originalAmbientLight;

        // --- REMOVE OVERLAY ---
        if (overlayObject != null)
        {
            Destroy(overlayObject);
            overlayObject = null;
        }

        // --- REMOVE PÓS-PROCESSAMENTO ---
        if (postProcess != null)
        {
            postProcess.Deactivate();
        }

        Debug.Log("Dark Vision Desativada");
    }
}