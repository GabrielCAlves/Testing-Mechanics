using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventSystem", menuName = "Events/EventSystem")]
public class EventSystem : ScriptableObject
{
    [Header("Configuração")]
    [SerializeField] private bool debugMode = true;
    [SerializeField] private List<EventSystem> childEvents = new List<EventSystem>();

    [Header("Eventos")]
    public UnityEvent onEventTriggered = new UnityEvent();
    public UnityEvent<object> onEventTriggeredWithData = new UnityEvent<object>();

    [Header("Referências")]
    public List<EventSystem> linkedEvents = new List<EventSystem>();

    // Lista de listeners registrados
    private List<EventListener> listeners = new List<EventListener>();
    private List<EventListener> listenersToRemove = new List<EventListener>();
    private bool isExecuting = false;

    #region Métodos Públicos

    /// <summary>
    /// Dispara o evento e todos os eventos vinculados
    /// </summary>
    public void Raise()
    {
        Raise(null);
    }

    /// <summary>
    /// Dispara o evento com dados e todos os eventos vinculados
    /// </summary>
    public void Raise(object data)
    {
        if (debugMode)
            Debug.Log($"[EventSystem] {name} disparado com dados: {data ?? "null"}");

        // Dispara os listeners locais
        ExecuteListeners(data);

        // Dispara eventos vinculados (sub-eventos)
        ExecuteLinkedEvents(data);

        // Dispara eventos filhos
        ExecuteChildEvents(data);
    }

    /// <summary>
    /// Registra um listener no evento
    /// </summary>
    public void RegisterListener(EventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);

            if (debugMode)
                Debug.Log($"[EventSystem] {listener.name} registrado em {name}");
        }
    }

    /// <summary>
    /// Desregistra um listener do evento
    /// </summary>
    public void UnregisterListener(EventListener listener)
    {
        if (isExecuting)
        {
            // Adiciona para remoção segura durante execução
            if (!listenersToRemove.Contains(listener))
                listenersToRemove.Add(listener);
        }
        else
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);

                if (debugMode)
                    Debug.Log($"[EventSystem] {listener.name} desregistrado de {name}");
            }
        }
    }

    /// <summary>
    /// Vincula outro EventSystem a este
    /// </summary>
    public void LinkEvent(EventSystem eventSystem)
    {
        if (!linkedEvents.Contains(eventSystem))
        {
            linkedEvents.Add(eventSystem);

            if (debugMode)
                Debug.Log($"[EventSystem] {eventSystem.name} vinculado a {name}");
        }
    }

    /// <summary>
    /// Desvincula um EventSystem deste
    /// </summary>
    public void UnlinkEvent(EventSystem eventSystem)
    {
        if (linkedEvents.Contains(eventSystem))
        {
            linkedEvents.Remove(eventSystem);

            if (debugMode)
                Debug.Log($"[EventSystem] {eventSystem.name} desvinculado de {name}");
        }
    }

    /// <summary>
    /// Adiciona um evento filho
    /// </summary>
    public void AddChildEvent(EventSystem childEvent)
    {
        if (!childEvents.Contains(childEvent))
        {
            childEvents.Add(childEvent);

            if (debugMode)
                Debug.Log($"[EventSystem] {childEvent.name} adicionado como filho de {name}");
        }
    }

    /// <summary>
    /// Remove um evento filho
    /// </summary>
    public void RemoveChildEvent(EventSystem childEvent)
    {
        if (childEvents.Contains(childEvent))
        {
            childEvents.Remove(childEvent);

            if (debugMode)
                Debug.Log($"[EventSystem] {childEvent.name} removido como filho de {name}");
        }
    }

    /// <summary>
    /// Limpa todos os listeners
    /// </summary>
    public void ClearListeners()
    {
        listeners.Clear();
        listenersToRemove.Clear();
    }

    #endregion

    #region Métodos Privados

    private void ExecuteListeners(object data)
    {
        isExecuting = true;

        // Executa todos os listeners
        foreach (var listener in listeners.ToArray())
        {
            if (listener != null && listener.gameObject != null)
            {
                listener.OnEventRaised(data);
            }
        }

        isExecuting = false;

        // Remove listeners marcados para remoção
        foreach (var listener in listenersToRemove)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
        listenersToRemove.Clear();

        // Dispara os UnityEvents
        onEventTriggered.Invoke();
        if (data != null)
            onEventTriggeredWithData.Invoke(data);
    }

    private void ExecuteLinkedEvents(object data)
    {
        foreach (var linkedEvent in linkedEvents)
        {
            if (linkedEvent != null)
            {
                if (debugMode)
                    Debug.Log($"[EventSystem] Propagando para evento vinculado: {linkedEvent.name}");

                linkedEvent.Raise(data);
            }
        }
    }

    private void ExecuteChildEvents(object data)
    {
        foreach (var childEvent in childEvents)
        {
            if (childEvent != null)
            {
                if (debugMode)
                    Debug.Log($"[EventSystem] Propagando para evento filho: {childEvent.name}");

                childEvent.Raise(data);
            }
        }
    }

    #endregion
}