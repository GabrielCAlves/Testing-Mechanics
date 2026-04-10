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

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = FindFirstObjectByType<PlayerInventoryItem>().GetComponent<InventorySystem>();

        LoadConsumableSlots();

        inventorySystem.OnItemAdded += OnItemAdded;
        inventorySystem.OnItemRemoved += OnItemRemoved;

        RefreshAllSlots();
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