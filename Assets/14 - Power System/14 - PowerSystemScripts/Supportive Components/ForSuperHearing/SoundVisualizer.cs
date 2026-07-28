using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
public class SoundVisualizer : MonoBehaviour
{
    [Header("Configurações")]
    public float duration = 2f;
    public float maxScale = 5f;
    public float expandSpeed = 2f;
    public float fadeSpeed = 1f;

    [Header("Cores")]
    public Color startColor = Color.green;
    public Color endColor = Color.clear;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            startColor = originalColor;
            spriteRenderer.color = startColor;
        }

        originalScale = transform.localScale;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration);

        // Expande
        float scale = Mathf.Lerp(1, maxScale, progress * expandSpeed);
        transform.localScale = originalScale * scale;

        // Desvanece
        if (spriteRenderer != null)
        {
            Color color = Color.Lerp(startColor, endColor, progress * fadeSpeed);
            spriteRenderer.color = color;
        }

        // Destroi após o tempo máximo
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    // Método para definir a intensidade do som
    public void SetIntensity(float intensity)
    {
        if (spriteRenderer != null)
        {
            Color color = startColor;
            color.a = Mathf.Clamp01(intensity * 2f);
            spriteRenderer.color = color;
        }

        // Ajusta escala baseado na intensidade
        float scale = 1f + intensity * maxScale;
        transform.localScale = originalScale * scale;
    }
}