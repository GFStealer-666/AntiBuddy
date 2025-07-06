using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main game manager that coordinates overall game flow and other managers
/// Handles turn transitions and game state changes
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Core Managers")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PathogenManager pathogenManager;
    [SerializeField] private GameStateManager gameStateManager;
    
    [Header("UI")]
    [SerializeField] private GameLogUI gameLogUI;
    
    // Events for other systems to listen to
    public System.Action OnPlayerTurnStart;
    public System.Action OnPlayerTurnEnd;
    public System.Action OnPathogenTurnStart;
    public System.Action OnPathogenTurnEnd;
    public System.Action OnGameOver;
    public System.Action OnGameWon;
    
    #region Initialization
    
    void Start()
    {
        InitializeManagers();
        StartGame();
    }
    
    private void InitializeManagers()
    {
        // Find managers if not assigned
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();
        if (playerManager == null)
            playerManager = FindFirstObjectByType<PlayerManager>();
        if (pathogenManager == null)
            pathogenManager = FindFirstObjectByType<PathogenManager>();
        if (gameStateManager == null)
            gameStateManager = GameStateManager.Instance;
        if (gameLogUI == null)
            gameLogUI = FindFirstObjectByType<GameLogUI>();
        
        // Subscribe to manager events for logging
        SubscribeToEvents();
        
        Debug.Log("GameManager: All managers initialized");
    }
    
    private void SubscribeToEvents()
    {
        // Pathogen events
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenDefeated += HandleReward;
            pathogenManager.OnAllPathogensDefeated += HandleGameWon;
            pathogenManager.OnPathogenSpawned += LogPathogenSpawned;
            pathogenManager.OnPathogenDefeated += LogPathogenDefeated;
        }
        
        // Player events
        if (playerManager != null)
        {
            playerManager.OnCardPlayed += LogCardPlayed;
            playerManager.OnPlayerHealed += LogPlayerHealed;
        }
        
        // Turn events
        if (turnManager != null)
        {
            TurnManager.OnTurnPhaseChanged += LogTurnPhaseChanged;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromEvents();
    }
    
    private void UnsubscribeFromEvents()
    {
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenDefeated -= HandleReward;
            pathogenManager.OnAllPathogensDefeated -= HandleGameWon;
            pathogenManager.OnPathogenSpawned -= LogPathogenSpawned;
            pathogenManager.OnPathogenDefeated -= LogPathogenDefeated;
        }
        
        if (playerManager != null)
        {
            playerManager.OnCardPlayed -= LogCardPlayed;
            playerManager.OnPlayerHealed -= LogPlayerHealed;
        }
        
        TurnManager.OnTurnPhaseChanged -= LogTurnPhaseChanged;
    }
    
    private void StartGame()
    {
        gameStateManager?.StartGame();
        // TurnManager will handle starting the first turn in its own Start() method
        Debug.Log("GameManager: Game started, TurnManager will handle turn flow");
    }
    
    #endregion
    #region Game State Management
    
    private void HandleGameOver()
    {
        Debug.Log("=== GAME OVER ===");
        gameStateManager?.EndGame(false);
        OnGameOver?.Invoke();
    }
    
    private void HandleGameWon()
    {
        Debug.Log("=== PLAYER WINS ===");
        gameStateManager?.EndGame(true);
        OnGameWon?.Invoke();
    }
    private void HandleReward(Pathogen defeatedPathogen)
    {
        // Handle rewards for defeating a pathogen
        Debug.Log($"Player defeated {defeatedPathogen.GetPathogenName()} and earned rewards");
        playerManager.GetPlayer().PlayerTokens.AddTokens(defeatedPathogen.GetTokenDropOnDeath());
        // Notify other systems if needed   
        // For example, update player stats, grant items, etc.
    }
    
    #endregion

    #region Public Interface
    
    // Getters for other systems
    public bool IsPlayerTurn() => turnManager?.IsPlayerTurn() ?? false;
    public int GetCurrentTurn() => turnManager?.GetTurnNumber() ?? 0;
    public Player GetPlayer() => playerManager?.GetPlayer();
    public List<Pathogen> GetActivePathogens() => pathogenManager?.GetActivePathogens() ?? new List<Pathogen>();
    public bool IsGameOver() => gameStateManager?.CurrentState == GameState.GameOver;
    


    public List<CardSO> GetPlayerHand() => playerManager?.GetPlayerHand() ?? new List<CardSO>();
    public Pathogen GetCurrentTargetedPathogen() => pathogenManager?.GetCurrentPathogen();
    public bool IsGameWon() => pathogenManager?.IsPathogenAllDefeated() ?? false;
    
    #endregion
    
    #region UI Methods
    
    [ContextMenu("End Turn")]
    public void EndTurnButtonPressed()
    {
        if (turnManager?.GetCurrentPhase() == TurnPhase.PlayerTurn)
        {
            turnManager.EndPlayerTurn("player manually ended turn");
        }
    }
    
    [ContextMenu("Heal Player")]
    public void HealPlayerDebug()
    {
        playerManager?.HealPlayer(10);
    }

    
    #endregion
    
    #region Game Logging Methods
    
    private void LogTurnPhaseChanged(TurnPhase phase)
    {
        string message = "";
        switch (phase)
        {
            case TurnPhase.PlayerTurn:
                message = "Player turn started";
                break;
            case TurnPhase.PathogenTurn:
                message = "Pathogen turn started";
                break;
            case TurnPhase.GameOver:
                message = "=== GAME OVER ===";
                break;
        }
        
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log($"GameManager: {message}");
            // Will add GameLogUI integration here once it compiles
        }
    }
    
    private void LogCardPlayed(CardSO card)
    {
        string cardType = "";
        if (card is ItemSO)
        {
            cardType = " (Item)";
        }
        
        string message = $"Player played {card.cardName}{cardType}";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    private void LogPlayerHealed(int healAmount)
    {
        string message = $"Player healed for {healAmount} HP";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    private void LogPathogenSpawned(Pathogen pathogen)
    {
        string message = $"New pathogen appeared: {pathogen.GetPathogenName()}";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    private void LogPathogenDefeated(Pathogen pathogen)
    {
        string message = $"Pathogen defeated: {pathogen.GetPathogenName()}";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    /// <summary>
    /// Log damage dealt from any source
    /// </summary>
    public void LogDamage(string attacker, string target, int damage)
    {
        string message = $"{attacker} dealt {damage} damage to {target}";
        Debug.Log($"GameManager: {message}");
        
        if (gameLogUI != null)
        {
            gameLogUI.LogDamage(attacker, target, damage);
        }
    }
    
    /// <summary>
    /// Log item usage (called when items are used directly from inventory)
    /// </summary>
    public void LogItemUse(string itemName)
    {
        string message = $"Player used item: {itemName}";
        Debug.Log($"GameManager: {message}");
        
        if (gameLogUI != null)
        {
            gameLogUI.LogItemUse(itemName);
        }
    }
    
    /// <summary>
    /// Log item purchase (called when items are bought from shop)
    /// </summary>
    public void LogItemPurchase(string purchaseMessage)
    {
        string message = $"Player purchased {purchaseMessage}";
        Debug.Log($"GameManager: {message}");
        
        if (gameLogUI != null)
        {
            gameLogUI.LogItemPurchase(purchaseMessage);
        }
    }
    
    /// <summary>
    /// Log any card effect application with detailed info
    /// </summary>
    public void LogCardEffect(string cardName, string effect)
    {
        string message = $"{cardName}: {effect}";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    /// <summary>
    /// Log card addition to hand
    /// </summary>
    public void LogCardAddedToHand(string cardName)
    {
        string message = $"Added {cardName} to hand";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    /// <summary>
    /// Log turn summary (cards played count)
    /// </summary>
    public void LogTurnSummary()
    {
        if (playerManager != null)
        {
            int cardsPlayed = playerManager.GetPlayedCards().Count;
            string message = $"Player played {cardsPlayed} card(s) this turn";
            Debug.Log($"GameManager: {message}");
            
            if (gameLogUI != null)
            {
                gameLogUI.LogItemUse(message);
            }
        }
    }
    
    #endregion
    
    #region Turn Event Handlers
    
    /// <summary>
    /// Log turn end and summarize what happened
    /// </summary>
    public void LogTurnEnd()
    {
        LogTurnSummary();
        
        string message = "Player turn ended";
        Debug.Log($"GameManager: {message}");
        
        if (gameLogUI != null)
        {
            gameLogUI.LogItemUse(message);
        }
    }
    
    /// <summary>
    /// Log turn end with specific reason
    /// </summary>
    public void LogTurnEnd(string reason)
    {
        if (playerManager != null)
        {
            int cardsPlayed = playerManager.GetPlayedCards().Count;
            string message = $"Turn ended: {reason} ({cardsPlayed} cards played)";
            Debug.Log($"GameManager: {message}");
            
            if (gameLogUI != null)
            {
                gameLogUI.LogItemUse(message);
            }
        }
    }
    
    /// <summary>
    /// Log pathogen turn actions
    /// </summary>
    public void LogPathogenAction(string pathogenName, string action)
    {
        string message = $"{pathogenName}: {action}";
        Debug.Log($"GameManager: {message}");
        // Will add GameLogUI integration here once it compiles
    }
    
    #endregion
}

