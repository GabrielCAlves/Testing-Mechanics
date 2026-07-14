using UnityEngine;

public class ItemAlt : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private int quantity;
    [SerializeField] private Sprite sprite;
    [TextArea][SerializeField] private string itemDescription;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            Destroy(gameObject);
        }
    }
}
