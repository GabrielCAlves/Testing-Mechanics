using UnityEngine;

public class Craftable : MonoBehaviour
{
    public CraftingRecipe craftingRecipe;
    public InventorySystem inventorySystem;

    [Header("UI Feedback")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    private float messageTimer = 0f;
    private bool isShowingMessage = false;

    void Start()
    {
        if (craftingRecipe == null)
        {
            craftingRecipe = GetComponent<CraftingRecipe>();
        }

        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isShowingMessage)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
            {
                if (messagePanel != null)
                {
                    messagePanel.SetActive(false);
                }
                isShowingMessage = false;
            }
        }
    }

    public void Build()
    {
        if (craftingRecipe != null && inventorySystem != null)
        {
            if (craftingRecipe.CanCraft(inventorySystem))
            {
                craftingRecipe.Craft(inventorySystem);
                ShowMessage("Crafted successfully!", Color.white);
            }
            else
            {
                string missingText = craftingRecipe.GetMissingIngredientsText(inventorySystem);
                ShowMessage(missingText, Color.white);
            }
        }
        else
        {
            ShowMessage("Crafting system error!", Color.red);
            Debug.LogError("Craftable: craftingRecipe or inventorySystem is null!");
        }
    }

    private void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }

        messageTimer = messageDuration;
        isShowingMessage = true;

        Debug.Log($"Crafting message: {message}");
    }

    public void SetupMessageUI(GameObject panel, TMPro.TextMeshProUGUI text, float duration = 2f)
    {
        messagePanel = panel;
        messageText = text;
        messageDuration = duration;

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
}