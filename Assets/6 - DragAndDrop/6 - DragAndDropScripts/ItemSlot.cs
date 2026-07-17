using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryMenuManager inventoryMenuManager;
    [SerializeField] private GameObject item;
    [SerializeField] private ItemSlotInventoryMenu itemSlotInventoryMenu;
    [SerializeField] private bool switchPlaces;

    private void Start()
    {
        if (item == null)
            item = GetComponentInChildren<DraggableUI>().gameObject;

        if(itemSlotInventoryMenu == null)
            itemSlotInventoryMenu = GetComponent<ItemSlotInventoryMenu>();

        if(inventoryMenuManager == null)
            inventoryMenuManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryMenuManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.GetComponent<RectTransform>().position = item.transform.position;
            //eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            if (switchPlaces)
                Switch(eventData.pointerDrag.GetComponent<RectTransform>().gameObject);
            else
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }

    private void Switch(GameObject droppedItem)
    {
        ItemSlotInventoryMenu droppedItemItemSlotInventoryMenu = droppedItem.GetComponentInParent<ItemSlotInventoryMenu>();

        if(!droppedItemItemSlotInventoryMenu.isEmpty)
        {
            int tempIndex = droppedItemItemSlotInventoryMenu.item.slotIndex;
            droppedItemItemSlotInventoryMenu.item.slotIndex = itemSlotInventoryMenu.item.slotIndex;
            itemSlotInventoryMenu.item.slotIndex = tempIndex;

            // Second params are arbitrary, just to trigger the update of the slot, since the UpdateInventoryMenuSlot function only uses the second param for new stacks.       
            // The important part is that the item is updated in the correct slot index.
            Item tempItem = itemSlotInventoryMenu.item;

            inventoryMenuManager.UpdateInventoryMenuSlot(droppedItemItemSlotInventoryMenu.item, itemSlotInventoryMenu.item.slotIndex);

            inventoryMenuManager.UpdateInventoryMenuSlot(tempItem, tempIndex);
        }
        
    }
}
