using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public Button useButton;

    private Item currentItem;
    private InventorySystem inventory;
    private int slotIndex;

    //public Health _Health;

    void Start()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseCurrentItem);
        }
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void SetInventory(InventorySystem inv)
    {
        inventory = inv;
    }

    public void SetSlot(Item item)
    {
        currentItem = item;

        if (item != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
            }

            if (quantityText != null)
            {
                quantityText.text = item.currentQuantity > 0 ? $"x{item.currentQuantity.ToString()}" : "";
            }

            if (useButton != null)
            {
                useButton.interactable = true;
            }
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.text = "";
            }

            if (useButton != null)
            {
                useButton.interactable = false;
            }
        }
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (currentItem != null && quantityText != null)
        {
            quantityText.text = newQuantity > 1 ? newQuantity.ToString() : "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseCurrentItem();
        }
    }

    void UseCurrentItem()
    {
        if (currentItem != null && inventory != null)
        {
            inventory.UseItem(currentItem.id);
        }
    }

    void OnDestroy()
    {
        if (useButton != null)
        {
            useButton.onClick.RemoveListener(UseCurrentItem);
        }
    }
}