using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI quantityText;

    private Item currentItem;

    void Awake()
    {
        if (quantityText == null)
        {
            quantityText = GetComponentInChildren<TextMeshProUGUI>();

            if (quantityText == null)
            {
                GameObject textObj = new GameObject("Quantity");
                textObj.transform.SetParent(transform);
                quantityText = textObj.AddComponent<TextMeshProUGUI>();
                quantityText.fontSize = 24;
                quantityText.color = Color.white;
                quantityText.alignment = TextAlignmentOptions.BottomRight;

                RectTransform rect = quantityText.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                rect.anchoredPosition = new Vector2(-10, 10);
            }
        }
    }

    public void SetIngredient(Item item)
    {
        currentItem = item;

        if (item != null && quantityText != null)
        {
            quantityText.text = $"x{item.currentQuantity}";
        }
    }
}