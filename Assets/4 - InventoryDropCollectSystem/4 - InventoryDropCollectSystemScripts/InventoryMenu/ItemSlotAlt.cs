using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotInventoryMenu : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    [SerializeField] public string itemName;
    [SerializeField] public int quantity;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public bool isFull;
    [SerializeField] public string itemDescription;
    [SerializeField] private Sprite emptySprite;

    //ItemSlot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] public GameObject selectedPanel;
    [SerializeField] public bool thisItemSelected;
    //[SerializeField] private InventoryManager inventoryManager;
    //[SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private UIInventoryManager uiInventoryManager;

    //Item Description Slot
    [SerializeField] public Image itemDescriptionImage;
    [SerializeField] public TMP_Text itemDescriptionNameText;
    [SerializeField] public TMP_Text itemDescriptionText;

    private void Start()
    {
        uiInventoryManager = GameObject.Find("UIInventoryManager").GetComponent<UIInventoryManager>();
    }

    public void UpdateItemOnSlot(Item item)
    {
        if(item != null)
        {
            this.itemName = item.name;
            this.quantity = item.currentQuantity;
            this.itemSprite = item.icon;
            isFull = true;
            this.itemDescription = item.itemDescription;
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
            itemImage.sprite = itemSprite;
        }
        else
        {
            this.itemName = "";
            this.quantity = 0;
            this.itemSprite = emptySprite;
            isFull = false;
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
        //inventoryManager.DeselectAllSlots();
        uiInventoryManager.DeselectAllSlots();
        selectedPanel.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;

        if(itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }
    }

    public void OnRightClick()
    {

    }
}
