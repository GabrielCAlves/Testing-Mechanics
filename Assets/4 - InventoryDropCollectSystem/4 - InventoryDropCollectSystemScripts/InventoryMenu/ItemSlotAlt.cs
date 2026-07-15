using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotInventoryMenu : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    [SerializeField] public Item item;
    [SerializeField] public string itemName;
    [SerializeField] public int quantity;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public bool isEmpty = true;
    [SerializeField] public string itemDescription;
    [SerializeField] private Image emptyImage;

    //ItemSlot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] public GameObject selectedPanel;
    [SerializeField] public bool thisItemSelected;
    [SerializeField] private InventoryMenuManager inventoryMenuManager;

    //Item Description Slot
    [SerializeField] public Image itemDescriptionImage;
    [SerializeField] public TMP_Text itemDescriptionNameText;
    [SerializeField] public TMP_Text itemDescriptionText;

    private void Start()
    {
        inventoryMenuManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryMenuManager>();
    }

    public void UpdateItemOnSlot(Item item = null)
    {
        Debug.Log($"Is {item.name} null? Answer: {item == null}. Index: {item.slotIndex}. Current quantity: {item.currentQuantity}");

        if (item.currentQuantity > 0)
        {
            this.item = item;
            this.itemName = item.name;
            this.quantity = item.currentQuantity;
            this.itemSprite = item.icon;
            isEmpty = false;
            this.itemDescription = item.itemDescription;
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
            itemImage.sprite = itemSprite;
        }
        else
        {
            item.slotIndex = -1;

            this.item = null;
            this.itemName = "";
            this.quantity = 0;
            this.itemSprite = emptyImage.sprite;
            isEmpty = true;
            this.itemDescription = "";
            quantityText.text = quantity.ToString();
            quantityText.enabled = false;
            itemImage.sprite = itemSprite;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryMenuManager.DeselectAllSlots();
        selectedPanel.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;

        if(itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptyImage.sprite;
        }
    }

    public void OnRightClick()
    {

    }
}
