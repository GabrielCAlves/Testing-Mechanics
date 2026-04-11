using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeBar : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public Image fillImage;
    public SpriteRenderer fillSpriteRenderer;
    public Image lifeDecreaseBackground;
    public TextMeshProUGUI healthText;

    [Header("Health Colors")]
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (fillImage == null && healthSlider != null)
            fillImage = healthSlider.fillRect?.GetComponent<Image>();

        if (fillSpriteRenderer == null && healthSlider != null)
            fillSpriteRenderer = healthSlider.fillRect?.GetComponent<SpriteRenderer>();

        if(lifeDecreaseBackground == null && healthSlider != null)
            lifeDecreaseBackground = healthSlider.transform.Find("LifeDecreaseBackground")?.GetComponent<Image>();

        if(healthText == null)
            healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;

            UpdateLifeDecreaseBackground();
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

            UpdateLifeDecreaseBackground();
        }

        if (healthText != null)
        {
            int maxHealth = healthSlider != null ? (int)healthSlider.maxValue : 100;
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void UpdateLifeDecreaseBackground()
    {
        if(lifeDecreaseBackground != null)
        {
            float healthPercentage = healthSlider.value / healthSlider.maxValue;

            if(lifeDecreaseBackground.fillAmount != healthPercentage)
            {
                Debug.Log($"Updating lifeDecreaseBackground fillAmount from {lifeDecreaseBackground.fillAmount} to {healthPercentage}");
                
                StartCoroutine(AnimateLifeDecreaseBackground(healthPercentage));

                return;
            }

            lifeDecreaseBackground.fillAmount = healthSlider.value / healthSlider.maxValue;
        }
    }

    public void DamageAnimation()
    {
        if (fillImage != null)
            StartCoroutine(FlashDamage());

        if (fillSpriteRenderer != null)
            StartCoroutine(FlashDamageSpriteRenderer());
    }

    System.Collections.IEnumerator AnimateLifeDecreaseBackground(float targetFillAmount)
    {
        float initialFillAmount = lifeDecreaseBackground.fillAmount;
        float elapsedTime = 0f;
        float animationDuration = 0.5f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            lifeDecreaseBackground.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / animationDuration);
            yield return null;
        }
        lifeDecreaseBackground.fillAmount = targetFillAmount;
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