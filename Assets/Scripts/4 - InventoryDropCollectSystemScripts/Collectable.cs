using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [Header("Floating Animation")]
    [SerializeField] private float floatAmplitude = 30f;
    [SerializeField] private float floatSpeed = 1.5f;

    private RectTransform rectTransform;
    private Image image;
    private Vector3 startPosition;
    private float floatOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void Start()
    {
        if (rectTransform != null)
        {
            startPosition = rectTransform.localPosition;
        }
        else
        {
            startPosition = transform.localPosition;
        }

        floatOffset = Random.Range(0f, Mathf.PI * 2f); // So the drops don't all float in sync

        Debug.Log($"Collectable initialized at position: {startPosition}");
    }

    void Update()
    {
        if (rectTransform != null)
        {
            float time = Time.time * floatSpeed;
            float sinValue = Mathf.Sin(time + floatOffset);

            float newY = startPosition.y + (sinValue * floatAmplitude);

            Vector3 newPosition = new Vector3(startPosition.x, newY, startPosition.z);
            rectTransform.localPosition = newPosition;
        }
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;

        if (image != null && itemData != null && itemData.icon != null)
        {
            image.sprite = itemData.icon;
            image.enabled = true;

            rectTransform.sizeDelta = new Vector2(50, 50);
        }
        else if (image != null)
        {
            Debug.LogWarning($"Couldn't update sprite: image exists but itemData or icon is null. ItemData: {itemData != null}, Icon: {(itemData != null ? itemData.icon != null : false)}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other); // Só vai chamar aqui se o Player e o Collectable estiverem no mesmo espaço no mundo (Ambos no World Space ou ambos no Canvas)
        }
    }

    public void Collect(Collider2D other)
    {
        InventorySystem inventory = other.GetComponent<InventorySystem>();
        if (inventory != null && itemData != null)
        {
            if (inventory.items.Count >= inventory.maxCapacity && !inventory.CanStack(new Item(itemData)))
            {
                Debug.Log($"Inventory full! Cannot collect {itemData.itemName}");
                return;
            }

            Item item = new Item(itemData);
            bool added = inventory.AddItem(item, 1);

            if (added)
            {
                if (itemData.collectionEffect != null)
                    Instantiate(itemData.collectionEffect, transform.position, Quaternion.identity);

                if (itemData.collectionSound != null)
                    AudioSource.PlayClipAtPoint(itemData.collectionSound, transform.position);

                Debug.Log($"{itemData.itemName} collected!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Could not add item to inventory (inventory full?)");
            }
        }
        else
        {
            Debug.LogWarning($"Cannot collect item: Inventory null? {inventory == null}, ItemData null? {itemData == null}");
        }
    }
}