using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Referências Globais")]
    public static EventManager Instance { get; private set; }

    [Header("Eventos Principais")]
    [SerializeField] private EventSystemBase gameStartEvent;
    [SerializeField] private EventSystemBase gameOverEvent;
    [SerializeField] private EventSystemBase playerDeathEvent;
    [SerializeField] private EventSystemBase playerScoreEvent;
    [SerializeField] private EventSystemBase levelCompleteEvent;

    [Header("Eventos de Sistema")]
    [SerializeField] private EventSystemBase inputEvent;
    [SerializeField] private EventSystemBase audioEvent;
    [SerializeField] private EventSystemBase uiEvent;

    [Header("Eventos de Dados")]
    [SerializeField] private EventSystemBase dataSavedEvent;
    [SerializeField] private EventSystemBase dataLoadedEvent;

    // Dicionário para acesso rápido por nome
    private Dictionary<string, EventSystemBase> eventDictionary = new Dictionary<string, EventSystemBase>();

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Inicialização

    private void InitializeEvents()
    {
        // Registra todos os eventos no dicionário
        RegisterEvent("GameStart", gameStartEvent);
        RegisterEvent("GameOver", gameOverEvent);
        RegisterEvent("PlayerDeath", playerDeathEvent);
        RegisterEvent("PlayerScore", playerScoreEvent);
        RegisterEvent("LevelComplete", levelCompleteEvent);
        RegisterEvent("Input", inputEvent);
        RegisterEvent("Audio", audioEvent);
        RegisterEvent("UI", uiEvent);
        RegisterEvent("DataSaved", dataSavedEvent);
        RegisterEvent("DataLoaded", dataLoadedEvent);

        // Configura hierarquia de eventos
        SetupEventHierarchy();
    }

    private void RegisterEvent(string name, EventSystemBase eventSystem)
    {
        if (eventSystem != null && !eventDictionary.ContainsKey(name))
        {
            eventDictionary.Add(name, eventSystem);
        }
    }

    private void SetupEventHierarchy()
    {
        // Exemplo: PlayerDeath dispara GameOver se for a última vida
        if (playerDeathEvent != null && gameOverEvent != null)
        {
            playerDeathEvent.LinkEvent(gameOverEvent);
        }

        // Exemplo: LevelComplete dispara UI e DataSaved
        if (levelCompleteEvent != null)
        {
            if (uiEvent != null)
                levelCompleteEvent.LinkEvent(uiEvent);
            if (dataSavedEvent != null)
                levelCompleteEvent.LinkEvent(dataSavedEvent);
        }
    }

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Obtém um evento por nome
    /// </summary>
    public EventSystemBase GetEvent(string name)
    {
        if (eventDictionary.TryGetValue(name, out EventSystemBase eventSystem))
            return eventSystem;

        Debug.LogWarning($"[EventManager] Evento '{name}' não encontrado!");
        return null;
    }

    /// <summary>
    /// Dispara um evento por nome
    /// </summary>
    public void RaiseEvent(string name, object data = null)
    {
        var eventSystem = GetEvent(name);
        if (eventSystem != null)
        {
            if (data != null)
                eventSystem.Raise(data);
            else
                eventSystem.Raise();
        }
    }

    /// <summary>
    /// Registra um novo evento em runtime
    /// </summary>
    public void RegisterRuntimeEvent(string name, EventSystemBase eventSystem)
    {
        if (!eventDictionary.ContainsKey(name))
        {
            eventDictionary.Add(name, eventSystem);
        }
        else
        {
            Debug.LogWarning($"[EventManager] Evento '{name}' já existe!");
        }
    }

    #endregion

    #region Getters Públicos

    public EventSystemBase GameStart => gameStartEvent;
    public EventSystemBase GameOver => gameOverEvent;
    public EventSystemBase PlayerDeath => playerDeathEvent;
    public EventSystemBase PlayerScore => playerScoreEvent;
    public EventSystemBase LevelComplete => levelCompleteEvent;
    public EventSystemBase InputEvent => inputEvent;
    public EventSystemBase AudioEvent => audioEvent;
    public EventSystemBase UIEvent => uiEvent;
    public EventSystemBase DataSaved => dataSavedEvent;
    public EventSystemBase DataLoaded => dataLoadedEvent;

    #endregion
}