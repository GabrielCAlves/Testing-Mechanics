using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MyMultipleLifeBars : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public List<Image> fillImages;
    public SpriteRenderer fillSpriteRenderer;
    public TextMeshProUGUI healthText;

    [Header("Health Colors")]
    public bool useHealthColors = false;
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    private LifeSystem lifeSystem;

    private int segmentCount;
    private float segmentValue;

    private float remainingHealth;
    private float percent;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (fillImages == null || fillImages.Count == 0)
        {
            Debug.LogWarning("MultipleLifeBars: No fillImage attributed to the list!");

            return;
        }

        if(lifeSystem  == null) { 
            lifeSystem = GetComponentInParent<LifeSystem>();
            if (lifeSystem == null)
                Debug.LogWarning("MultipleLifeBars: No LifeSystem found in parent objects!");
        }

        ConfiguringImageList();

        UpdateSegmentConfiguration();
    }

    void ConfiguringImageList()
    {
            // Configuring all the items in fillImages to start filled
            for (int i = 0; i < fillImages.Count; i++)
            {
                if (fillImages[i] != null)
                {
                    fillImages[i].fillAmount = 1f;
                    //fillImages[i].type = Image.Type.Filled;
                    //fillImages[i].fillMethod = Image.FillMethod.Horizontal;
                    //fillImages[i].fillOrigin = 0; // Starts filling from the left

                    Debug.Log($"Segment {i} configured with fillAmount = 1f");
                }
        }
    }

    void UpdateSegmentConfiguration()
    {
        if (fillImages == null || fillImages.Count == 0)
        {
            segmentCount = 1;
            segmentValue = healthSlider != null ? healthSlider.maxValue : 100;

            return;
        }

        segmentCount = fillImages.Count;
        segmentValue = healthSlider.maxValue / segmentCount;

        Debug.Log($"SegmentValue = {segmentValue} (maxHealth/{segmentCount})");
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        UpdateSegmentConfiguration();

        // Resets all the fillImages to full (starting from the first)
        for (int i = 0; i < fillImages.Count; i++)
        {
            if (fillImages[i] != null)
                fillImages[i].fillAmount = 1f;
        }

        UpdateBar(maxHealth);

        //if (healthText != null)
        //    healthText.text = $"{maxHealth} / {maxHealth}";

        SetHealthText(maxHealth);
    }

    void SetHealthText(int currentHealth)
    {
        if (healthText != null)
        {
            int maxHealth = healthSlider != null ? (int)healthSlider.maxValue : 100;
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    public void UpdateBar(int currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            // Calculates the remaining life
            remainingHealth = currentHealth;

            Debug.Log($"=== UpdateBar: currentHealth = {currentHealth}, remainingHealth = {remainingHealth} ===");

            // Goes through the fillImages from the FIRST till the LAST (index 0, 1, 2, 3...)
            for (int i = 0; i < fillImages.Count; i++)
            {
                if (fillImages[(fillImages.Count - 1) - i] == null) continue;

                if (remainingHealth >= segmentValue)
                {
                    // This segment is completely filled
                    fillImages[(fillImages.Count - 1) - i].fillAmount = 1f;
                    remainingHealth -= segmentValue;
                    Debug.Log($"Segment {i}: FULL (fillAmount = 1f), remainingHealth now = {remainingHealth}");
                }
                else if (remainingHealth > 0)
                {
                    // This segment is partially filled
                    percent = remainingHealth / segmentValue;
                    fillImages[(fillImages.Count - 1) - i].fillAmount = Mathf.Clamp01(percent);
                    Debug.Log($"Segment {i}: PARCIAL (fillAmount = {fillImages[(fillImages.Count - 1) - i].fillAmount}), percent = {percent}");
                    remainingHealth = 0;
                }
                else
                {
                    // This segment is empty
                    fillImages[(fillImages.Count - 1) - i].fillAmount = 0f;
                    Debug.Log($"Segment {i}: EMPTY (fillAmount = 0f)");
                }
            }
        }

        SetHealthColors();

        //if (healthText != null)
        //{
        //    int maxHealth = healthSlider != null ? (int)healthSlider.maxValue : 100;
        //    healthText.text = $"{currentHealth} / {maxHealth}";
        //}

        SetHealthText(currentHealth);
    }

    private void SetHealthColors() { 
        if (useHealthColors)
        {
            percent = lifeSystem != null ? (float)lifeSystem.currentHealth / lifeSystem.maxHealth : 1f;

            Debug.Log($"Setting health colors based on percent: {percent} = {lifeSystem.currentHealth} / {lifeSystem.maxHealth}");
                
            for (int i = 0; i < fillImages.Count; i++)
            {
                if (fillImages[i] != null)
                {
                    if (percent > 0.6f)
                        fillImages[i].color = fullHealthColor;
                    else if (percent > 0.3f)
                        fillImages[i].color = mediumHealthColor;
                    else
                        fillImages[i].color = lowHealthColor;
                }
            }
        }
    }

    public void DamageAnimation()
    {
        if (fillImages != null)
        {
            StartCoroutine(FlashDamageAllImages());
        }

        if (fillSpriteRenderer != null)
            StartCoroutine(FlashDamageSpriteRenderer());
    }

    IEnumerator FlashDamageAllImages()
    {
        List<Color> originalColors = new List<Color>();
        foreach (var img in fillImages)
        {
            if (img != null)
                originalColors.Add(img.color);
            else
                originalColors.Add(Color.white);
        }

        foreach (var img in fillImages)
        {
            if (img != null)
                img.color = Color.white;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < fillImages.Count; i++)
        {
            if (fillImages[i] != null && i < originalColors.Count)
                fillImages[i].color = originalColors[i];
        }
    }

    IEnumerator FlashDamageSpriteRenderer()
    {
        if (fillSpriteRenderer == null) yield break;

        Color originalColor = fillSpriteRenderer.color;
        fillSpriteRenderer.color = Color.white;
        
        yield return new WaitForSeconds(0.1f);

        fillSpriteRenderer.color = originalColor;
    }
}