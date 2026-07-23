using UnityEngine;

public class MotionBlur : MonoBehaviour
{
    [Header("Configurações")]
    [Range(0f, 1f)]
    public float blurAmount = 0f;
    public float maxBlurAmount = 0.8f;
    public float transitionSpeed = 2f;

    [Header("Renderização")]
    public Material blurMaterial;
    public int blurIterations = 3;
    public float blurSpread = 0.6f;

    private float targetBlur = 0f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        if (blurMaterial == null)
        {
            Debug.LogWarning("MotionBlur: Nenhum material de blur foi atribuído!");
        }
    }

    void Update()
    {
        // Transição suave
        blurAmount = Mathf.Lerp(blurAmount, targetBlur, Time.deltaTime * transitionSpeed);
    }

    public void SetBlur(float amount)
    {
        targetBlur = Mathf.Clamp01(amount) * maxBlurAmount;
    }

    public void EnableBlur(float intensity = 0.5f)
    {
        SetBlur(intensity);
    }

    public void DisableBlur()
    {
        SetBlur(0f);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurMaterial == null || blurAmount <= 0.001f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Aplica blur simples
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

        Graphics.Blit(source, temp);

        for (int i = 0; i < blurIterations; i++)
        {
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width, source.height);
            blurMaterial.SetFloat("_BlurSize", blurSpread * (i + 1) * blurAmount);
            Graphics.Blit(temp, temp2, blurMaterial);
            RenderTexture.ReleaseTemporary(temp);
            temp = temp2;
        }

        // Combina com a imagem original baseado no blur amount
        blurMaterial.SetFloat("_Blend", blurAmount);
        Graphics.Blit(temp, destination);

        RenderTexture.ReleaseTemporary(temp);
    }
}