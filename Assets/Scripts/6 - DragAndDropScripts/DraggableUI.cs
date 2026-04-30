using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if(canvasGroup == null) 
            canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Deixa o item semi-transparente durante o arrasto
        canvasGroup.blocksRaycasts = false; // Não permite que outros elementos recebam eventos de clique durante o arrasto
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 newPosition))
        {
            rectTransform.anchoredPosition = newPosition;
        }

        //Alternativa sem repetir código no OnPointerDown
        //rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    // Ter atenção a:
    // - Se a imagem do slot ou do item tem a âncora diferente do centro, o que pode causar um posicionamento inesperado na tela;
    // - Olhar a prioridade das imagens para não ficar atrás de outros elementos da UI (Alterar ordem no Hierarchy).
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Restaura a opacidade do item
        canvasGroup.blocksRaycasts = true; // Permite que outros elementos recebam eventos de clique novamente
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 newPosition))
        {
            rectTransform.anchoredPosition = newPosition;
        }
    }
}
