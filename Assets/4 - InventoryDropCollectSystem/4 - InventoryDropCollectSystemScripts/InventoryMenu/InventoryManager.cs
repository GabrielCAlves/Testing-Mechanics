using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private bool menuActivated;
    [SerializeField] public ItemSlotInventoryMenu[] itemSlots;

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

    //public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    //{
    //    for(int i = 0; i < itemSlot.Length; ++i)
    //    {
    //        itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
    //        return;
    //    }
    //}

    public void AddItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; ++i)
        {

            itemSlots[i].UpdateItemOnSlot(item);
            return;
        }
    }

    public void DeselectAllSlots()
    {
        for(int i = 0; i < itemSlots.Length; ++i)
        {
            itemSlots[i].selectedPanel.SetActive(false);
            itemSlots[i].thisItemSelected = false;
        }
    }
}
