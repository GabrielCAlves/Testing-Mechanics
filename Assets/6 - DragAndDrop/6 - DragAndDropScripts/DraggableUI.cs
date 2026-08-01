using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public ItemSlotInventoryMenu itemSlotInventoryMenu;

    [SerializeField] private Transform originalParent;
    private int originalSiblingIndex;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if(canvasGroup == null) 
            canvasGroup = GetComponent<CanvasGroup>();

        if(itemSlotInventoryMenu == null)
            itemSlotInventoryMenu = GetComponentInParent<ItemSlotInventoryMenu>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log($"transform.parent.name.Contains(\"ItemSlot\") = " + transform.parent.name.Contains("ItemSlot"));

        if (itemSlotInventoryMenu != null)
        {
            originalParent = itemSlotInventoryMenu.gameObject.transform;
            originalSiblingIndex = itemSlotInventoryMenu.gameObject.transform.GetSiblingIndex();
            transform.SetParent(parentCanvas.transform);
        }

        canvasGroup.alpha = 0.6f; // Makes the item semi-transparent during the drag
        canvasGroup.blocksRaycasts = false; // Doesn't let other elements receive clicking events during the drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 newPosition) && (itemSlotInventoryMenu != null && !itemSlotInventoryMenu.isEmpty))
        {
            rectTransform.anchoredPosition = newPosition;
        }
        else
        {
            rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }

        //Alternative without repeting code in OnPointerDown
        //rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    // Have attention to:
    // - If the slot or item image has an anchor other than the center, it may result in unexpected on-screen positioning; 
    // - Check image priority to ensure they do not appear behind other UI elements (change the order in the Hierarchy).
    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemSlotInventoryMenu != null)
        {
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalSiblingIndex);
        }
            

        canvasGroup.alpha = 1f; // Restores the item's opacity
        canvasGroup.blocksRaycasts = true; // Allows other elements to receive click events again.

        if (itemSlotInventoryMenu != null)
            rectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 newPosition) && (itemSlotInventoryMenu != null && !itemSlotInventoryMenu.isEmpty))
        {
            //rectTransform.anchoredPosition = newPosition;
        }
        else
        {
            rectTransform.anchoredPosition = newPosition;
        }
    }
}
