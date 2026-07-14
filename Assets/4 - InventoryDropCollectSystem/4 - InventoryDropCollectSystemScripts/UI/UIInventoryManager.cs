using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInventoryManager : MonoBehaviour
{
    [Header("Inventory References")]
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Consumables Section")]
    [SerializeField] private Transform consumableContainer;
    private List<UISlot> consumableSlots = new List<UISlot>();

    [Header("Ingredients Section")]
    [SerializeField] private UIIngredientManager ingredientManager;

    [Header("Inventory Canvas Menu")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private bool menuActivated;
    [SerializeField] private ItemSlotInventoryMenu[] itemSlots; //

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = FindFirstObjectByType<PlayerInventoryItem>().GetComponent<InventorySystem>();

        LoadConsumableSlots();

        inventorySystem.OnItemAdded += OnItemAdded;
        inventorySystem.OnItemRemoved += OnItemRemoved;

        RefreshAllSlots();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            Time.timeScale = 1;
            inventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            Time.timeScale = 0;
            inventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; ++i)
        {
            itemSlots[i].selectedPanel.SetActive(false);
            itemSlots[i].thisItemSelected = false;
        }
    }

    void LoadConsumableSlots()
    {
        consumableSlots.Clear();
        if (consumableContainer != null)
        {
            foreach (Transform child in consumableContainer)
            {
                UISlot slot = child.GetComponent<UISlot>();
                if (slot != null)
                {
                    slot.SetSlotIndex(consumableSlots.Count);
                    slot.SetInventory(inventorySystem);
                    consumableSlots.Add(slot);
                }
            }
        }
    }

    void RefreshAllSlots()
    {
        List<Item> consumables = new List<Item>();

        foreach (Item item in inventorySystem.items)
        {
            if (item.type == ItemType.Consumable)
            {
                consumables.Add(item);
            }
        }

        for (int i = 0; i < consumableSlots.Count; i++)
        {
            if (i < consumables.Count)
            {
                consumableSlots[i].SetSlot(consumables[i]);
            }
            else
            {
                consumableSlots[i].SetSlot(null);
            }
        }

        if (ingredientManager != null)
        {
            ingredientManager.RefreshIngredients();
        }
    }

    void OnItemAdded(Item item, int slotIndex)
    {
        if (item.type == ItemType.Consumable)
        {
            RefreshConsumableSlots();
        }
        else if (item.type == ItemType.Ingredient)
        {
            if (ingredientManager != null)
            {
                ingredientManager.RefreshIngredients();
            }
        }

        itemSlots[slotIndex].UpdateItemOnSlot(item);
    }

    void OnItemRemoved(Item item, int slotIndex, int quantity)
    {
        if (item != null)
        {
            if (item.type == ItemType.Consumable)
            {
                RefreshConsumableSlots();
            }
            else if (item.type == ItemType.Ingredient)
            {
                if (ingredientManager != null)
                {
                    ingredientManager.RefreshIngredients();
                }
            }
        }
        else
        {
            RefreshAllSlots();
        }


        itemSlots[slotIndex].UpdateItemOnSlot(item);
    }

    void RefreshConsumableSlots()
    {
        List<Item> consumables = new List<Item>();
        foreach (Item item in inventorySystem.items)
        {
            if (item.type == ItemType.Consumable)
            {
                consumables.Add(item);
            }
        }

        for (int i = 0; i < consumableSlots.Count; i++)
        {
            if (i < consumables.Count)
            {
                consumableSlots[i].SetSlot(consumables[i]);
            }
            else
            {
                consumableSlots[i].SetSlot(null);
            }
        }
    }

    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnItemAdded -= OnItemAdded;
            inventorySystem.OnItemRemoved -= OnItemRemoved;
        }
    }
}