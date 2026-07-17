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
        }
    }

    private void Switch(GameObject droppedItem)
    {
        ItemSlotInventoryMenu droppedItemItemSlotInventoryMenu = droppedItem.GetComponentInParent<ItemSlotInventoryMenu>();

        if(!droppedItemItemSlotInventoryMenu.isEmpty)
        {
            //Vector3 tempPosition = /*item.transform.position*/ transform.position;
            //item.transform.position = droppedItemItemSlotInventoryMenu.transform.position;
            //droppedItem.transform.position = tempPosition;

            //Debug.Log($"(Switch) droppedItemItemSlotInventoryMenu.item.name = {droppedItemItemSlotInventoryMenu.item.name}. Index: {droppedItemItemSlotInventoryMenu.item.slotIndex}.");
            //Debug.Log($"(Switch) itemSlotInventoryMenu.item.name = {itemSlotInventoryMenu.item.name}. Index: {itemSlotInventoryMenu.item.slotIndex}.");

            //Debug.Log($"Trying to change the index of {droppedItemItemSlotInventoryMenu.item.name} from {droppedItemItemSlotInventoryMenu.item.slotIndex} to {itemSlotInventoryMenu.item.slotIndex}");
            //Debug.Log($"Trying to change the index of {itemSlotInventoryMenu.item.name} from {itemSlotInventoryMenu.item.slotIndex} to {droppedItemItemSlotInventoryMenu.item.slotIndex}");

            int tempIndex = droppedItemItemSlotInventoryMenu.item.slotIndex; // - 9
            droppedItemItemSlotInventoryMenu.item.slotIndex = itemSlotInventoryMenu.item.slotIndex/*item.slotIndex*/; // 9 - 15
            itemSlotInventoryMenu.item.slotIndex = tempIndex; // 15 - 9
            //itemSlotInventoryMenu.index = tempIndex;

            //Debug.Log($"(After Change) droppedItemItemSlotInventoryMenu.item.name = {droppedItemItemSlotInventoryMenu.item.name}. Index: {droppedItemItemSlotInventoryMenu.item.slotIndex}.");
            //Debug.Log($"(After Change) itemSlotInventoryMenu.item.name = {itemSlotInventoryMenu.item.name}. Index: {itemSlotInventoryMenu.item.slotIndex}.");

            // Second params are arbitrary, just to trigger the update of the slot, since the UpdateInventoryMenuSlot function only uses the second param for new stacks.       
            //The important part is that the item is updated in the correct slot index.
            Item tempItem = itemSlotInventoryMenu.item;

            //Debug.Log($"tempItem's name {tempItem.name}, tempItem.slotIndex = {tempItem.slotIndex} is a copy of itemSlotInventoryMenu.item's name {itemSlotInventoryMenu.item.name} and its index {itemSlotInventoryMenu.item.slotIndex}");

            //Debug.Log($"(Switch) droppedItemItemSlotInventoryMenu.item.name = {droppedItemItemSlotInventoryMenu.item.name} - droppedItemItemSlotInventoryMenu.index: {droppedItemItemSlotInventoryMenu.index/*item.slotIndex*/}. droppedItemItemSlotInventoryMenu.item.slotIndex: {droppedItemItemSlotInventoryMenu.item.slotIndex}.");
            inventoryMenuManager.UpdateInventoryMenuSlot(droppedItemItemSlotInventoryMenu.item, itemSlotInventoryMenu.item.slotIndex); // 15 - 9

            //Debug.Log($"(Switch) itemSlotInventoryMenu.item.name = {tempItem.name} - TempIndex: {tempIndex} - itemSlotInventoryMenu.index: {itemSlotInventoryMenu.index/*item.slotIndex*/}. itemSlotInventoryMenu.item.slotIndex: {tempItem.slotIndex}.");
            inventoryMenuManager.UpdateInventoryMenuSlot(tempItem, tempIndex); // 9 - 9

            //Vector3 tempAnchoredPosition = item.GetComponent<RectTransform>().anchoredPosition;
            //item.GetComponent<RectTransform>().anchoredPosition = droppedItem.GetComponent<RectTransform>().anchoredPosition;
            //droppedItem.GetComponent<RectTransform>().anchoredPosition = tempAnchoredPosition;
        }
        
    }
}
