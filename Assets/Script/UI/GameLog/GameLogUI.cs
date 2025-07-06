using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Data structure for storing individual log messages
/// Can be serialized to JSON for persistence
/// </summary>
[System.Serializable]
public class LogMessage
{
    public int id;
    public string timestamp;
    public string message;
    public string colorHex;
    public LogCategory category;
    public int turnNumber;
    public string additionalData; // For future expansion (JSON data, etc.)
    
    public LogMessage(int id, string message, Color color, LogCategory category, int turn = 0)
    {
        this.id = id;
        this.timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        this.message = message;
        this.colorHex = ColorUtility.ToHtmlStringRGB(color);
        this.category = category;
        this.turnNumber = turn;
        this.additionalData = "";
    }
}

/// <summary>
/// Categories for organizing log messages
/// </summary>
[System.Serializable]
public enum LogCategory
{
    System,
    PlayerAction,
    EnemyAction,
    Damage,
    Healing,
    CardEffect,
    ItemUse,
    TurnChange,
    GameState
}

/// <summary>
/// Container for exporting/importing complete game logs
/// </summary>
[System.Serializable]
public class GameLogData
{
    public string gameSession;
    public string startTime;
    public List<LogMessage> messages;
    public int totalTurns;
    public string gameResult; // "Victory", "Defeat", "Ongoing"
    
    public GameLogData()
    {
        gameSession = System.Guid.NewGuid().ToString();
        startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        messages = new List<LogMessage>();
        totalTurns = 0;
        gameResult = "Ongoing";
    }
}

/// <summary>
/// Simple on-screen game log that displays one message at a time
/// Each new action overrides the previous message
/// </summary>
public class GameLogUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI logText;
    
    [Header("Colors")]
    [SerializeField] private Color playerActionColor = Color.cyan;
    [SerializeField] private Color enemyActionColor = Color.red;
    [SerializeField] private Color systemColor = Color.yellow;
    [SerializeField] private Color damageColor = Color.orange;
    [SerializeField] private Color healColor = Color.green;
    
    // Singleton for easy access
    public static GameLogUI Instance { get; private set; }
    
    #region Unity Lifecycle
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Subscribe to game events
        SubscribeToEvents();
    }
    
    void Start()
    {
        // Show initial message
        ShowMessage("Game Started", systemColor);
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    #endregion
    
    #region Event Subscriptions
    
    private void SubscribeToEvents()
    {
        // Turn Manager events
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
        
        // Player events - we'll subscribe to these when we find the player
        StartCoroutine(SubscribeToPlayerEventsCoroutine());
    }
    
    private System.Collections.IEnumerator SubscribeToPlayerEventsCoroutine()
    {
        // Wait for managers to be ready
        yield return new WaitForSeconds(0.5f);
        
        // Find and subscribe to PlayerManager events
        var playerManager = FindFirstObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.OnCardPlayed += OnPlayerCardPlayed;
            playerManager.OnPlayerHealed += OnPlayerHealed;
        }
        
        // Find and subscribe to PathogenManager events
        var pathogenManager = FindFirstObjectByType<PathogenManager>();
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated += OnPathogenDefeated;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        // Turn Manager events
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
        
        // Player Manager events
        var playerManager = FindFirstObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.OnCardPlayed -= OnPlayerCardPlayed;
            playerManager.OnPlayerHealed -= OnPlayerHealed;
        }
        
        // Pathogen Manager events
        var pathogenManager = FindFirstObjectByType<PathogenManager>();
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= OnPathogenSpawned;
            pathogenManager.OnPathogenDefeated -= OnPathogenDefeated;
        }
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnTurnPhaseChanged(TurnPhase phase)
    {
        switch (phase)
        {
            case TurnPhase.PlayerTurn:
                ShowMessage("Player turn started", playerActionColor);
                break;
            case TurnPhase.PathogenTurn:
                ShowMessage("Pathogen turn started", enemyActionColor);
                break;
            case TurnPhase.GameOver:
                ShowMessage("GAME OVER", systemColor);
                break;
        }
    }
    
    private void OnPlayerCardPlayed(CardSO card)
    {
        string cardType = (card is ItemSO) ? " (Item)" : "";
        string damageInfo = "";
        
        // Check for specific damage-dealing cards
        if (card is MacrophageCardSO)
        {
            damageInfo = " and dealt 5 damage";
        }
        else if (card is CytotoxicCellCardSO)
        {
            damageInfo = " and dealt 25 damage";
        }
        else if (card is NaturalKillerCardSO)
        {
            damageInfo = " and dealt damage"; // Random 5-20, so we can't be specific
        }
        else if (card is BCellCardSO)
        {
            damageInfo = " (provides defense, no damage)";
        }
        
        ShowMessage($"Player used {card.cardName}{cardType}{damageInfo}", playerActionColor);
    }
    
    private void OnPlayerHealed(int healAmount)
    {
        ShowMessage($"Player healed for {healAmount} HP", healColor);
    }
    
    private void OnPathogenSpawned(Pathogen pathogen)
    {
        ShowMessage($"New pathogen appeared: {pathogen.GetPathogenName()}", enemyActionColor);
    }
    
    private void OnPathogenDefeated(Pathogen pathogen)
    {
        ShowMessage($"Pathogen defeated: {pathogen.GetPathogenName()}", playerActionColor);
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Show a simple message - replaces any previous message
    /// </summary>
    public void ShowMessage(string message)
    {
        ShowMessage(message, Color.white);
    }
    
    /// <summary>
    /// Show a simple message with color - replaces any previous message
    /// </summary>
    public void ShowMessage(string message, Color color)
    {
        if (logText != null)
        {
            logText.text = message; // No color formatting, just plain text
        }
    }
    
    /// <summary>
    /// Legacy method for compatibility - now just shows simple message
    /// </summary>
    public void AddLogEntry(string message)
    {
        ShowMessage(message);
    }
    
    /// <summary>
    /// Legacy method for compatibility - now just shows simple message
    /// </summary>
    public void AddLogEntry(string message, Color color)
    {
        ShowMessage(message, color);
    }
    
    /// <summary>
    /// Legacy method for compatibility - now just shows simple message
    /// </summary>
    public void AddLogEntry(string message, Color color, LogCategory category)
    {
        ShowMessage(message, color);
    }
    
    /// <summary>
    /// Show damage message
    /// </summary>
    public void LogDamage(string attacker, string target, int damage)
    {
        ShowMessage($"{attacker} dealt {damage} damage to {target}", damageColor);
    }
    
    /// <summary>
    /// Show healing message
    /// </summary>
    public void LogHealing(string target, int healAmount)
    {
        ShowMessage($"{target} healed for {healAmount} HP", healColor);
    }
    
    /// <summary>
    /// Show item use message
    /// </summary>
    public void LogItemUse(string itemName)
    {
        ShowMessage($"Player used {itemName}", playerActionColor);
    }
    
    /// <summary>
    /// Log an item purchase (from shop)
    /// </summary>
    public void LogItemPurchase(string purchaseMessage)
    {
        ShowMessage($"Player purchased {purchaseMessage}", playerActionColor);
    }
    
    /// <summary>
    /// Clear the message
    /// </summary>
    [ContextMenu("Clear Message")]
    public void ClearLog()
    {
        if (logText != null)
        {
            logText.text = "";
        }
    }
    
    #endregion
}
