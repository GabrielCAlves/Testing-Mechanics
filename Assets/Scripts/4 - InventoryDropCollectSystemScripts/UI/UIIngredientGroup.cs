using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIIngredientGroup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;

    private ItemData itemData;
    private int currentQuantity;

    public void Initialize(ItemData data, int quantity)
    {
        itemData = data;
        currentQuantity = quantity;

        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = true;
        }

        if (nameText != null)
        {
            nameText.text = data.itemName;
        }

        UpdateQuantity(quantity);
    }

    public void UpdateQuantity(int quantity)
    {
        currentQuantity = quantity;

        if (quantityText != null)
        {
            quantityText.text = $"x{quantity}";
        }
    }

    public string GetItemId()
    {
        return itemData != null ? itemData.id : "";
    }
}