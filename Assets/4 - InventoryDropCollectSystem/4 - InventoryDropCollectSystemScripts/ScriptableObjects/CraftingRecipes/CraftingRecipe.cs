using UnityEngine;
using System.Collections.Generic;
using System.Text;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    public ItemData result;
    public int quantityResult = 1;

    [System.Serializable]
    public struct Ingredient
    {
        public ItemData item;
        public int quantity;
    }

    public Ingredient[] ingredients;

    public bool CanCraft(InventorySystem inventory)
    {
        foreach (var ingredient in ingredients)
        {
            if (!inventory.HasQuantity(ingredient.item.id, ingredient.quantity))
            {
                return false;
            }
        }
        return true;
    }

    public List<string> GetMissingIngredients(InventorySystem inventory)
    {
        List<string> missing = new List<string>();

        foreach (var ingredient in ingredients)
        {
            int currentQuantity = inventory.GetTotalQuantity(ingredient.item.id);
            if (currentQuantity < ingredient.quantity)
            {
                int missingQuantity = ingredient.quantity - currentQuantity;
                missing.Add($"{ingredient.item.itemName} ({missingQuantity})");
            }
        }

        return missing;
    }

    public string GetMissingIngredientsText(InventorySystem inventory)
    {
        List<string> missing = GetMissingIngredients(inventory);

        if (missing.Count == 0)
            return "";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Missing ingredients:");

        for (int i = 0; i < missing.Count; i++)
        {
            sb.Append($"- {missing[i]}");
            if (i < missing.Count - 1)
                sb.AppendLine();
        }

        return sb.ToString();
    }

    public bool Craft(InventorySystem inventory)
    {
        if (!CanCraft(inventory)) return false;

        foreach (var ingredient in ingredients)
        {
            inventory.RemoveItem(ingredient.item.id, ingredient.quantity);
        }

        inventory.AddItem(new Item(result), quantityResult);

        return true;
    }
}