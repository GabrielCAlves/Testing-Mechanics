using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [Header("Configuração do Evento")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool registerOnEnable = true;

    [Header("Respostas")]
    public UnityEvent onEventRaised;
    public UnityEvent<object> onEventRaisedWithData;

    [Header("Filtros")]
    [SerializeField] private bool useFilters = false;
    [SerializeField] private List<EventFilter> filters = new List<EventFilter>();

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    #region Propriedades

    public EventSystem EventSystem
    {
        get => eventSystem;
        set
        {
            if (eventSystem != null)
                Unregister();

            eventSystem = value;

            if (registerOnEnable && isActiveAndEnabled)
                Register();
        }
    }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (registerOnEnable)
            Register();
    }

    private void OnDisable()
    {
        if (registerOnEnable)
            Unregister();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Registra o listener no evento
    /// </summary>
    public void Register()
    {
        if (eventSystem == null)
        {
            Debug.LogWarning($"[EventListener] {gameObject.name}: EventSystem não configurado!");
            return;
        }

        eventSystem.RegisterListener(this);

        if (debugMode)
            Debug.Log($"[EventListener] {gameObject.name} registrado em {eventSystem.name}");
    }

    /// <summary>
    /// Desregistra o listener do evento
    /// </summary>
    public void Unregister()
    {
        if (eventSystem == null)
            return;

        eventSystem.UnregisterListener(this);

        if (debugMode)
            Debug.Log($"[EventListener] {gameObject.name} desregistrado de {eventSystem.name}");
    }

    /// <summary>
    /// Método chamado quando o evento é disparado
    /// </summary>
    public void OnEventRaised(object data)
    {
        if (eventSystem == null)
            return;

        // Aplica filtros
        if (useFilters && !PassesFilters(data))
            return;

        if (debugMode)
            Debug.Log($"[EventListener] {gameObject.name} recebeu evento {eventSystem.name} com dados: {data ?? "null"}");

        // Dispara os UnityEvents
        onEventRaised.Invoke();
        if (data != null)
            onEventRaisedWithData.Invoke(data);
    }

    #endregion

    #region Métodos Privados

    private bool PassesFilters(object data)
    {
        foreach (var filter in filters)
        {
            if (!filter.PassesFilter(data))
                return false;
        }
        return true;
    }

    #endregion
}

[System.Serializable]
public class EventFilter
{
    public enum FilterType
    {
        None,
        Tag,
        Layer,
        Type,
        Custom
    }

    public FilterType filterType = FilterType.None;
    public string tagFilter;
    public LayerMask layerMask;
    public string typeName;
    public UnityEvent<object> customFilter = new UnityEvent<object>();

    public bool PassesFilter(object data)
    {
        switch (filterType)
        {
            case FilterType.None:
                return true;

            case FilterType.Tag:
                if (data is GameObject go)
                    return go.CompareTag(tagFilter);
                if (data is Component comp)
                    return comp.CompareTag(tagFilter);
                return false;

            case FilterType.Layer:
                if (data is GameObject goLayer)
                    return (layerMask & (1 << goLayer.layer)) != 0;
                if (data is Component compLayer)
                    return (layerMask & (1 << compLayer.gameObject.layer)) != 0;
                return false;

            case FilterType.Type:
                if (data == null)
                    return false;
                var targetType = System.Type.GetType(typeName);
                if (targetType == null)
                    return false;
                return targetType.IsAssignableFrom(data.GetType());

            case FilterType.Custom:
                customFilter.Invoke(data);
                return true;

            default:
                return true;
        }
    }
}