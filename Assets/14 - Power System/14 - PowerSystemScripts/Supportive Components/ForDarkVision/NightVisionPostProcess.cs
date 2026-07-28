using UnityEngine;

[RequireComponent(typeof(Camera))]
public class NightVisionPostProcess : MonoBehaviour
{
    private Camera cam;
    private bool isActive = false;
    private Color visionColor = new Color(0.2f, 1f, 0.2f, 0.3f);
    private float intensity = 1.5f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void Activate(Color color, float intensity)
    {
        this.visionColor = color;
        this.intensity = intensity;
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!isActive)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Cria uma textura temporária
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

        // Desenha a imagem original
        Graphics.Blit(source, temp);

        // Aplica o efeito de visão noturna
        Texture2D result = new Texture2D(source.width, source.height);
        RenderTexture.active = temp;
        result.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        result.Apply();

        // Converte para tons de verde
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float gray = pixels[i].r * 0.299f + pixels[i].g * 0.587f + pixels[i].b * 0.114f;
            pixels[i] = new Color(
                gray * visionColor.r * intensity,
                gray * visionColor.g * intensity,
                gray * visionColor.b * intensity,
                pixels[i].a
            );
        }
        result.SetPixels(pixels);
        result.Apply();

        // Aplica o resultado
        Graphics.Blit(result, destination);

        // Limpa
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(temp);
    }
}