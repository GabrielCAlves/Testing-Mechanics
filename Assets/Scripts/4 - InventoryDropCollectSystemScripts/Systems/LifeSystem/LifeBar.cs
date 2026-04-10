using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public SpriteRenderer fillSpriteRenderer;
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    public Text healthText;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (fillImage == null && healthSlider != null)
            fillImage = healthSlider.fillRect?.GetComponent<Image>();

        if (fillSpriteRenderer == null && healthSlider != null)
            fillSpriteRenderer = healthSlider.fillRect?.GetComponent<SpriteRenderer>();
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        if (healthText != null)
            healthText.text = $"{maxHealth} / {maxHealth}";
    }

    public void UpdateBar(int currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            if (fillImage != null)
            {
                float percentage = currentHealth / healthSlider.maxValue;

                if (percentage > 0.6f)
                    fillImage.color = fullHealthColor;
                else if (percentage > 0.3f)
                    fillImage.color = mediumHealthColor;
                else
                    fillImage.color = lowHealthColor;
            }

            if (fillSpriteRenderer != null)
            {
                float percentage = currentHealth / healthSlider.maxValue;

                if (percentage > 0.6f)
                    fillSpriteRenderer.color = fullHealthColor;
                else if (percentage > 0.3f)
                    fillSpriteRenderer.color = mediumHealthColor;
                else
                    fillSpriteRenderer.color = lowHealthColor;
            }
        }

        if (healthText != null)
        {
            int maxHealth = healthSlider != null ? (int)healthSlider.maxValue : 100;
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    public void DamageAnimation()
    {
        if (fillImage != null)
            StartCoroutine(FlashDamage());

        if (fillSpriteRenderer != null)
            StartCoroutine(FlashDamageSpriteRenderer());
    }

    System.Collections.IEnumerator FlashDamage()
    {
        Color originalColor = fillImage.color;
        fillImage.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        fillImage.color = originalColor;
    }

    System.Collections.IEnumerator FlashDamageSpriteRenderer()
    {
        Color originalColor = fillSpriteRenderer.color;
        fillSpriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        fillSpriteRenderer.color = originalColor;
    }
}