using UnityEngine;

public class AutoDestroyVisualizer : MonoBehaviour
{
    public float duration = 2f;
    public float maxScale = 5f;
    public bool useAnimation = true;

    private float timer = 0f;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Se tiver animator, usa a animação
        var animator = GetComponent<Animator>();
        if (animator != null && useAnimation)
        {
            // A animação já está rodando
            return;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        // Se não tiver animação, faz manualmente
        if (!useAnimation || GetComponent<Animator>() == null)
        {
            // Expande
            float scale = Mathf.Lerp(1, maxScale, progress);
            transform.localScale = originalScale * scale;

            // Desvanece
            if (spriteRenderer != null)
            {
                Color color = originalColor;
                color.a = Mathf.Lerp(originalColor.a, 0, progress);
                spriteRenderer.color = color;
            }
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}