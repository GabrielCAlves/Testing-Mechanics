using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Referências Globais")]
    public static EventManager Instance { get; private set; }

    [Header("Eventos Principais")]
    [SerializeField] private EventSystem gameStartEvent;
    [SerializeField] private EventSystem gameOverEvent;
    [SerializeField] private EventSystem playerDeathEvent;
    [SerializeField] private EventSystem playerScoreEvent;
    [SerializeField] private EventSystem levelCompleteEvent;

    [Header("Eventos de Sistema")]
    [SerializeField] private EventSystem inputEvent;
    [SerializeField] private EventSystem audioEvent;
    [SerializeField] private EventSystem uiEvent;

    [Header("Eventos de Dados")]
    [SerializeField] private EventSystem dataSavedEvent;
    [SerializeField] private EventSystem dataLoadedEvent;

    // Dicionário para acesso rápido por nome
    private Dictionary<string, EventSystem> eventDictionary = new Dictionary<string, EventSystem>();

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

    private void RegisterEvent(string name, EventSystem eventSystem)
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
    public EventSystem GetEvent(string name)
    {
        if (eventDictionary.TryGetValue(name, out EventSystem eventSystem))
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
    public void RegisterRuntimeEvent(string name, EventSystem eventSystem)
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

    public EventSystem GameStart => gameStartEvent;
    public EventSystem GameOver => gameOverEvent;
    public EventSystem PlayerDeath => playerDeathEvent;
    public EventSystem PlayerScore => playerScoreEvent;
    public EventSystem LevelComplete => levelCompleteEvent;
    public EventSystem InputEvent => inputEvent;
    public EventSystem AudioEvent => audioEvent;
    public EventSystem UIEvent => uiEvent;
    public EventSystem DataSaved => dataSavedEvent;
    public EventSystem DataLoaded => dataLoadedEvent;

    #endregion
}