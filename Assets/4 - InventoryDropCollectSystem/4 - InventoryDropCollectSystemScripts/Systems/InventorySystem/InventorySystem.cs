using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxCapacity = 20;

    public event System.Action<Item, int> OnItemAdded;
    public event System.Action<Item, int, int> OnItemRemoved;

    public bool AddItem(Item item, int quantity = 1)
    {
        if (items.Count >= maxCapacity && !CanStack(item))
            return false;

        int originalQuantity = quantity;

        if (item.maxQuantity > 1)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item currentItem = items[i];

                if (currentItem.id == item.id && currentItem.currentQuantity < currentItem.maxQuantity)
                {
                    int space = currentItem.maxQuantity - currentItem.currentQuantity;
                    int add = Mathf.Min(quantity, space);

                    currentItem.currentQuantity += add;
                    quantity -= add;

                    OnItemAdded?.Invoke(currentItem, i);

                    if (quantity <= 0) return true;
                }
            }
        }

        while (quantity > 0 && items.Count < maxCapacity)
        {
            Item newItem = new Item(item);
            newItem.currentQuantity = Mathf.Min(quantity, item.maxQuantity);
            quantity -= newItem.currentQuantity;

            int newIndex = items.Count;
            items.Add(newItem);

            OnItemAdded?.Invoke(newItem, newIndex);
        }

        bool addedAll = quantity == 0;

        if (addedAll)
        {
            Debug.Log($"Added {originalQuantity - quantity}x {item.name}");
        }
        else
        {
            Debug.Log($"Added {originalQuantity - quantity}x {item.name}. {quantity} left");
        }

        return addedAll;
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        int quantityToRemove = quantity;
        int originalQuantity = quantity;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].id == itemId)
            {
                if (items[i].currentQuantity > quantityToRemove)
                {
                    items[i].currentQuantity -= quantityToRemove;

                    OnItemRemoved?.Invoke(items[i], i, quantityToRemove);

                    Debug.Log($"Removed {quantityToRemove}x {items[i].name}. {items[i].currentQuantity} left");
                    return true;
                }
                else
                {
                    int removedQuantity = items[i].currentQuantity;
                    quantityToRemove -= removedQuantity;

                    Item removedItem = items[i];
                    int removedIndex = i;

                    items.RemoveAt(i);

                    OnItemRemoved?.Invoke(null, removedIndex, removedQuantity);

                    if (quantityToRemove <= 0)
                    {
                        return true;
                    }
                }
            }
        }

        bool removedAll = quantityToRemove == 0;
        if (!removedAll)
        {
            Debug.Log($"Wasn't possible to remove {quantityToRemove}x of the item {itemId}");
        }

        return removedAll;
    }

    public bool UseItem(string itemId)
    {
        Item itemToUse = items.FirstOrDefault(i => i.id == itemId);

        if (itemToUse != null)
        {
            switch (itemToUse.type)
            {
                case ItemType.Consumable:
                    LifeSystem lifeSystem = GetComponent<LifeSystem>();

                    if (lifeSystem != null && itemToUse.originalData != null)
                    {
                        int heal = itemToUse.originalData.healAmount; // /100

                        Debug.Log("Heal: " + heal);

                        if (heal > 0)
                        {
                            lifeSystem.Heal(heal);
                        }
                    }

                    //PlayerBehavior player = GetComponent<PlayerBehavior>();

                    //if (player != null && itemToUse.originalData != null)
                    //{
                    //    float speed = itemToUse.originalData.speedBoost;
                    //    float duration = itemToUse.originalData.duration;

                    //    if (speed > 0)
                    //    {
                    //        player.ApplySpeedBoost(speed, duration);
                    //    }
                    //}


                    break;

                case ItemType.Ingredient:
                    return false;

                default:
                    return false;
            }

            return RemoveItem(itemId, 1);
        }

        return false;
    }

    public bool CanStack(Item item)
    {
        return item.maxQuantity > 1 &&
               items.Any(i => i.id == item.id && i.currentQuantity < i.maxQuantity);
    }

    public bool HasQuantity(string itemId, int quantity)
    {
        int total = 0;
        foreach (Item item in items)
        {
            if (item.id == itemId)
            {
                total += item.currentQuantity;
                if (total >= quantity)
                    return true;
            }
        }
        return false;
    }

    public int GetTotalQuantity(string itemId)
    {
        int total = 0;
        foreach (Item item in items)
        {
            if (item.id == itemId)
            {
                total += item.currentQuantity;
            }
        }
        return total;
    }

    public void ClearInventory()
    {
        items.Clear();
    }
}

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public Sprite icon;

    public int maxQuantity = 99;
    public int currentQuantity;

    public ItemType type;
    public GameObject worldPrefab;
    public ItemData originalData;

    public Item(Item other)
    {
        id = other.id;
        name = other.name;
        icon = other.icon;
        maxQuantity = other.maxQuantity;
        currentQuantity = other.currentQuantity;
        type = other.type;
        worldPrefab = other.worldPrefab;
        originalData = other.originalData;
    }

    public Item(ItemData data)
    {
        id = data.id;
        name = data.itemName;
        icon = data.icon;
        maxQuantity = data.maxStack;
        type = data.type;
        worldPrefab = data.worldPrefab;
        currentQuantity = 1;
        originalData = data;
    }
}

public enum ItemType
{
    Consumable,
    Ingredient,
}