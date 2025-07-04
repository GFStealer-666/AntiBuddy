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
        
        // Subscribe to manager events
        if (pathogenManager != null)
        {
            pathogenManager.OnAllPathogensDefeated += HandleGameWon;
        }
        
        Debug.Log("GameManager: All managers initialized");
    }
    
    private void StartGame()
    {
        gameStateManager?.StartGame();
        StartPlayerTurn();
    }
    
    #endregion
    
    #region Turn Flow Control
    
    public void StartPlayerTurn()
    {
        Debug.Log("=== PLAYER TURN STARTED ===");
        
        // Notify TurnManager to start player turn
        turnManager?.StartPlayerTurn();
        
        // Notify PlayerManager to prepare for turn
        playerManager?.StartTurn();
        
        OnPlayerTurnStart?.Invoke();
    }
    
    public void EndPlayerTurn()
    {
        Debug.Log("=== PLAYER TURN ENDED ===");
        
        // Notify TurnManager to end player turn
        turnManager?.EndPlayerTurn();
        
        OnPlayerTurnEnd?.Invoke();
        
        // Start pathogen turn
        StartPathogenTurn();
    }
    
    public void StartPathogenTurn()
    {
        Debug.Log("=== PATHOGEN TURN STARTED ===");
        
        // Notify TurnManager to start pathogen turn
        turnManager?.StartPathogenTurn();
        
        // Execute pathogen actions through PathogenManager
        if (pathogenManager != null && playerManager != null)
        {
            var player = playerManager.GetPlayer();
            var playedCards = playerManager.GetPlayedCards();
            pathogenManager.ExecutePathogenTurn(player, playedCards);
        }
        
        OnPathogenTurnStart?.Invoke();
        
        // Check if player died
        if (playerManager?.GetPlayer()?.HP <= 0)
        {
            HandleGameOver();
            return;
        }
        
        // End pathogen turn after delay
        Invoke(nameof(EndPathogenTurn), 2f);
    }
    
    public void EndPathogenTurn()
    {
        Debug.Log("=== PATHOGEN TURN ENDED ===");
        
        OnPathogenTurnEnd?.Invoke();
        
        // Start next player turn
        StartPlayerTurn();
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
    
    #endregion
    
    #region Public Interface
    
    public bool PlayCard(CardSO card, Pathogen target = null)
    {
        // Check if it's player turn through TurnManager
        if (!turnManager.IsPlayerTurn())
        {
            Debug.LogWarning("Cannot play cards during pathogen turn");
            return false;
        }
        
        // Check if card is blocked by pathogens
        if (pathogenManager.IsCardBlocked(card.GetType()))
        {
            Debug.LogWarning($"{card.cardName} is blocked by pathogen abilities");
            return false;
        }
        
        // Try to play card through PlayerManager and TurnManager
        if (playerManager.PlayCard(card, target))
        {
            turnManager.OnCardPlayed(card);
            
            // Check if turn should end
            if (!turnManager.CanPlayMoreCards())
            {
                EndPlayerTurn();
            }
            
            return true;
        }
        
        return false;
    }
    
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
        if (turnManager.GetCurrentPhase() == TurnPhase.PlayerTurn)
        {
            EndPlayerTurn();
        }
    }
    
    [ContextMenu("Heal Player")]
    public void HealPlayerDebug()
    {
        playerManager?.HealPlayer(10);
    }

    
    #endregion
}
    
