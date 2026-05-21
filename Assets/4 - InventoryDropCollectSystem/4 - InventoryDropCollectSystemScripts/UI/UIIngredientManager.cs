using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UIIngredientManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ingredientsContent;
    [SerializeField] private GameObject ingredientGroupPrefab;
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Settings")]
    [SerializeField] private float updateDelay = 0.1f;

    private Dictionary<string, UIIngredientGroup> ingredientGroups = new Dictionary<string, UIIngredientGroup>();
    private float lastUpdateTime;

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = FindFirstObjectByType<PlayerInventoryItem>().GetComponent<InventorySystem>();

        if (inventorySystem != null)
        {
            inventorySystem.OnItemAdded += OnInventoryChanged;
            inventorySystem.OnItemRemoved += OnInventoryChanged;
        }

        RefreshIngredients();
    }

    void OnInventoryChanged(Item item, int slotIndex)
    {
        RefreshIngredients();
    }

    void OnInventoryChanged(Item item, int slotIndex, int quantity)
    {
        RefreshIngredients();
    }

    public void RefreshIngredients()
    {
        if (inventorySystem == null) return;

        Dictionary<string, int> groupedIngredients = new Dictionary<string, int>();

        foreach (Item item in inventorySystem.items)
        {
            if (item.type == ItemType.Ingredient && item.originalData != null)
            {
                if (groupedIngredients.ContainsKey(item.id))
                {
                    groupedIngredients[item.id] += item.currentQuantity;
                }
                else
                {
                    groupedIngredients[item.id] = item.currentQuantity;
                }
            }
        }

        List<string> idsToRemove = new List<string>();
        foreach (string id in ingredientGroups.Keys)
        {
            if (!groupedIngredients.ContainsKey(id))
            {
                idsToRemove.Add(id);
            }
        }

        foreach (string id in idsToRemove)
        {
            Destroy(ingredientGroups[id].gameObject);
            ingredientGroups.Remove(id);
        }

        foreach (var kvp in groupedIngredients)
        {
            if (ingredientGroups.ContainsKey(kvp.Key))
            {
                ingredientGroups[kvp.Key].UpdateQuantity(kvp.Value);
            }
            else
            {
                ItemData originalData = GetItemDataById(kvp.Key);
                if (originalData != null && ingredientGroupPrefab != null)
                {
                    GameObject newGroup = Instantiate(ingredientGroupPrefab, ingredientsContent);
                    UIIngredientGroup group = newGroup.GetComponent<UIIngredientGroup>();

                    if (group != null)
                    {
                        group.Initialize(originalData, kvp.Value);
                        ingredientGroups.Add(kvp.Key, group);
                    }
                }
            }
        }

        if (ingredientsContent != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(ingredientsContent as RectTransform);
        }
    }

    ItemData GetItemDataById(string id)
    {
        foreach (Item item in inventorySystem.items)
        {
            if (item.id == id && item.originalData != null)
            {
                return item.originalData;
            }
        }
        return null;
    }

    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnItemAdded -= OnInventoryChanged;
            inventorySystem.OnItemRemoved -= OnInventoryChanged;
        }
    }
}