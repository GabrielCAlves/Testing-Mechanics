using UnityEngine;
using System.Collections.Generic;

public class InventoryMenuManager : MonoBehaviour
{
    [Header("Inventory References")]
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Inventory Canvas Menu")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private bool menuActivated;
    [SerializeField] private List<ItemSlotInventoryMenu> itemSlots = new List<ItemSlotInventoryMenu>();

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = FindFirstObjectByType<PlayerInventoryItem>().GetComponent<InventorySystem>();

        for(int i = 0; i < itemSlots.Count; ++i)
        {
            itemSlots[i].index = i;
            itemSlots[i].item.slotIndex = i;
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Inventory") && menuActivated)
        {
            Time.timeScale = 1;
            inventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if(Input.GetButtonDown("Inventory") && !menuActivated)
        {
            Time.timeScale = 0;
            inventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }

    public void DeselectAllSlots()
    {
        for(int i = 0; i < itemSlots.Count; ++i)
        {
            itemSlots[i].selectedPanel.SetActive(false);
            itemSlots[i].thisItemSelected = false;
        }
    }

    public void UpdateInventoryMenuSlot(Item item, int slotIndex = -1)
    {
        if (slotIndex < 0)
        {
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                if(itemSlots[i].isEmpty)
                {
                    item.slotIndex = i;
                    itemSlots[i].UpdateItemOnSlot(item);
                    break;
                }
            }
        }else
        {
            Debug.Log($"Item name: {item.name} - Item's index: {item.slotIndex}");
            itemSlots[item.slotIndex].UpdateItemOnSlot(item);
        }
    }
}
