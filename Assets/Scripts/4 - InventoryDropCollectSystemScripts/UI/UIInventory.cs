using UnityEngine;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    [Header("References")]
    public InventorySystem inventory;
    public GameObject slotPrefab;
    public Transform gridContainer;

    [Header("Optional")]
    public bool showEmptySlots = true;

    private List<UISlot> slotList = new List<UISlot>();
    private bool isInitialized = false;

    void Start()
    {
        if (inventory == null)
        {
            return;
        }

        inventory.OnItemAdded += OnItemAdded;
        inventory.OnItemRemoved += OnItemRemoved;

        CreateInventorySlots();
        isInitialized = true;
    }

    void CreateInventorySlots()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        slotList.Clear();

        if (showEmptySlots)
        {
            for (int i = 0; i < inventory.maxCapacity; i++)
            {
                CreateSlot(i);
            }

            RefreshAllSlots();
        }
        else
        {
            for (int i = 0; i < inventory.items.Count; i++)
            {
                CreateSlot(i).SetSlot(inventory.items[i]);
            }
        }
    }

    UISlot CreateSlot(int index)
    {
        GameObject slotGO = Instantiate(slotPrefab, gridContainer);
        UISlot slot = slotGO.GetComponent<UISlot>();

        slot.SetSlotIndex(index);

        slot.SetInventory(inventory);

        slotList.Add(slot);
        return slot;
    }

    void OnItemAdded(Item item, int slotIndex)
    {
        if (!isInitialized) return;

        Debug.Log($"UI: Item '{item.name}' added on slot {slotIndex}");

        if (showEmptySlots)
        {
            if (slotIndex >= 0 && slotIndex < slotList.Count)
            {
                slotList[slotIndex].SetSlot(item);
            }
            else
            {
                Debug.LogWarning($"Slot index {slotIndex} out of range");
                RefreshAllSlots();
            }
        }
        else
        {
            RecreateAllSlots();
        }
    }

    void OnItemRemoved(Item item, int slotIndex, int quantity)
    {
        if (!isInitialized) return;

        if (item == null)
        {
            Debug.Log($"UI: Removed item from slot {slotIndex}, quantity: {quantity}");
        }
        else
        {
            Debug.Log($"UI: '{item.name}' quantity changed on slot {slotIndex}: -{quantity}, {item.currentQuantity} left");
        }

        if (showEmptySlots)
        {
            if (item == null)
            {
                RearrangeSlotsFrom(slotIndex);
            }
            else
            {
                if (slotIndex >= 0 && slotIndex < slotList.Count)
                {
                    slotList[slotIndex].SetSlot(item);
                }
                else
                {
                    Debug.LogWarning($"Slot index {slotIndex} out of range");
                    RefreshAllSlots();
                }
            }
        }
        else
        {
            RecreateAllSlots();
        }
    }

    void RearrangeSlotsFrom(int startIndex)
    {
        for (int i = startIndex; i < slotList.Count; i++)
        {
            if (i < inventory.items.Count)
            {
                slotList[i].SetSlot(inventory.items[i]);
            }
            else
            {
                slotList[i].SetSlot(null);
            }
        }
    }

    void RefreshAllSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < inventory.items.Count)
                slotList[i].SetSlot(inventory.items[i]);
            else
                slotList[i].SetSlot(null);
        }

        Debug.Log($"UI: All slots updated ({slotList.Count} slots)");
    }

    void RecreateAllSlots()
    {
        CreateInventorySlots();
    }

    public void ManualRefresh()
    {
        if (!showEmptySlots)
        {
            RecreateAllSlots();
        }
        else
        {
            RefreshAllSlots();
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemAdded -= OnItemAdded;
            inventory.OnItemRemoved -= OnItemRemoved;
        }
    }
}