using UnityEngine;
using System;
using System.Collections.Generic;
public enum TurnPhase
{
    PlayerTurn,
    PathogenTurn,
    GameOver
}

public class TurnManager : MonoBehaviour
{
    [Header("Managers")]
    public DeckManager deckManager;
    public CardField cardField;
    
    private GameStateManager gameStateManager;
    private PlayerManager playerManager;
    private PathogenManager pathogenManager;

    [Header("Turn Settings")]
    public int maxTurns = 50;
    public float turnTimeLimit = 60f; // seconds per turn
    public int cardsPerTurn = 2; // Maximum cards player can play per turn
    
    private TurnPhase currentPhase;
    private int turnNumber;
    private float turnTimer;
    private bool isProcessingTurn;
    private int cardsPlayedThisTurn;

    public static event Action<TurnPhase> OnTurnPhaseChanged;
    public static event Action<int> OnTurnNumberChanged;
    public static event Action<PlayerStats> OnPlayerStatsChanged;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (gameStateManager != null && gameStateManager.IsGameInProgress() && !isProcessingTurn)
        {
            HandleTurnTimer();
            HandleInput();
        }
    }

    void InitializeGame()
    {
        gameStateManager = GameStateManager.Instance;
        
        // Find or create PlayerManager
        playerManager = FindFirstObjectByType<PlayerManager>();
        if (playerManager == null)
        {
            GameObject playerManagerObj = new GameObject("PlayerManager");
            playerManager = playerManagerObj.AddComponent<PlayerManager>();
        }
        
        // Find or create PathogenManager
        pathogenManager = FindFirstObjectByType<PathogenManager>();
        if (pathogenManager == null)
        {
            pathogenManager = gameObject.AddComponent<PathogenManager>();
        }

        if (cardField == null)
        {
            cardField = FindFirstObjectByType<CardField>();
            if (cardField == null)
            {
                GameObject cardFieldObj = new GameObject("CardField");
                cardField = cardFieldObj.AddComponent<CardField>();
            }
        }

        currentPhase = TurnPhase.PlayerTurn;
        turnNumber = 1;
        turnTimer = turnTimeLimit;
        cardsPlayedThisTurn = 0;
        
        StartPlayerTurn();
    }

    void HandleTurnTimer()
    {
        if (currentPhase == TurnPhase.PlayerTurn)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                Debug.Log("Turn time limit reached, ending player turn");
                EndPlayerTurn();
            }
        }
    }

    void HandleInput()
    {
        // if (currentPhase == TurnPhase.PlayerTurn)
        // {
        //     // Example input handling - in a real game this would be handled by UI
        //     if (Input.GetKeyDown(KeyCode.D))
        //     {
        //         DrawCard();
        //     }
            
        //     if (Input.GetKeyDown(KeyCode.H))
        //     {
        //         HealPlayer();
        //     }
            
        //     if (Input.GetKeyDown(KeyCode.Return))
        //     {
        //         EndPlayerTurn();
        //     }
        // }
    }

    public void StartPlayerTurn()
    {
        if (isProcessingTurn) return;
        
        isProcessingTurn = true;
        currentPhase = TurnPhase.PlayerTurn;
        turnTimer = turnTimeLimit;
         int cardsToDraw = (turnNumber == 1) ? 5 : cardsPlayedThisTurn;
        cardsPlayedThisTurn = 0; // Reset cards played counter
        Debug.Log($"=== Player Turn {turnNumber} Started ===");
        Debug.Log($"Cards allowed this turn: {cardsPerTurn}");
        
        // Draw cards at start of turn (typically 1-2 cards per turn)
        // Draw 5 cards on first turn, 1 on subsequent turns
        playerManager.StartTurn(cardsToDraw);
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
        OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        
        isProcessingTurn = false;
    }

    public void EndPlayerTurn()
    {
        if (isProcessingTurn || currentPhase != TurnPhase.PlayerTurn) 
        {
            Debug.LogWarning($"TurnManager: Cannot end player turn - isProcessingTurn: {isProcessingTurn}, currentPhase: {currentPhase}");
            return;
        }
        
        isProcessingTurn = true;
        Debug.Log("=== Player Turn Ended ===");
        Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}");
        
        // Clear the card field after effects are processed
        if (cardField != null)
        {
            cardField.ClearField();
        }
        
        CheckWinConditions();
        
        if (currentPhase != TurnPhase.GameOver)
        {
            isProcessingTurn = false;
            Debug.Log("TurnManager: Starting pathogen turn...");
            StartPathogenTurn();
        }
        else
        {
            Debug.Log("TurnManager: Game is over, not starting pathogen turn");
        }
    }

    public void StartPathogenTurn()
    {
        if (isProcessingTurn) 
        {
            Debug.LogWarning("TurnManager: Already processing a turn, cannot start pathogen turn");
            return;
        }
        
        isProcessingTurn = true;
        currentPhase = TurnPhase.PathogenTurn;
        
        Debug.Log("=== Pathogen Turn Started ===");
        Debug.Log($"Turn Number: {turnNumber}");
        Debug.Log($"Active Pathogens: {pathogenManager?.GetActivePathogenCount() ?? 0}");
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
        
        // Execute pathogen actions
        ExecutePathogenTurn();
        
        isProcessingTurn = false;
    }

    void ExecutePathogenTurn()
    {
        if (pathogenManager == null || playerManager == null)
        {
            Debug.LogError("TurnManager: Missing PathogenManager or PlayerManager!");
            EndPathogenTurn();
            return;
        }
        
        // Check if there are any active pathogens
        if (!pathogenManager.HasActivePathogens())
        {
            Debug.Log("TurnManager: No active pathogens, checking if we should spawn next or end game");
            EndPathogenTurn();
            return;
        }
        
        // Get the played cards from this turn to pass to pathogen abilities
        var playedCards = playerManager.GetPlayedCards();
        
        // Execute pathogen turn with proper error handling
        try
        {
            pathogenManager.ExecutePathogenTurn(playerManager.GetPlayer(), playedCards);
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
            
            Debug.Log("TurnManager: Pathogen turn executed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TurnManager: Error during pathogen turn execution: {e.Message}");
        }
        
        // Check if player is defeated
        if (playerManager.GetPlayer().HP <= 0)
        {
            EndGame(false); // Player loses
            return;
        }

        isProcessingTurn = false;
        // End pathogen turn and start next player turn
        EndPathogenTurn();
    }

    public void EndPathogenTurn()
    {
        if (isProcessingTurn || currentPhase != TurnPhase.PathogenTurn) return;
        
        Debug.Log("=== Pathogen Turn Ended ===");
        
        turnNumber++;
        OnTurnNumberChanged?.Invoke(turnNumber);
        
        // Check turn limit
        if (turnNumber > maxTurns)
        {
            EndGame(false); // Game over due to turn limit
            return;
        }
        
        CheckWinConditions();
        
        if (currentPhase != TurnPhase.GameOver)
        {
            StartPlayerTurn();
        }
    }

    public bool PlayCard(CardSO card, Pathogen target = null)
    {
        if (currentPhase != TurnPhase.PlayerTurn || isProcessingTurn)
        {
            Debug.LogWarning("Cannot play card outside of player turn");
            return false;
        }
        
        if (cardsPlayedThisTurn >= cardsPerTurn)
        {
            Debug.LogWarning($"Cannot play more cards this turn. Limit: {cardsPerTurn}");
            // Auto-end turn when limit reached
            EndPlayerTurn();
            return false;
        }
        
        bool success = playerManager.PlayCard(card, target);
        if (success)
        {
            cardsPlayedThisTurn++;
            
            // Add card to field if it has no immediate effect (like Helper T-Cell or inactive Cytotoxic)
            bool shouldStayInField = ShouldCardStayInField(card);
            if (shouldStayInField && cardField != null)
            {
                cardField.TryPlayCardToField(card);
            }
            
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
            
            Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}/{cardsPerTurn}");
            
            // Auto-end turn if player has played maximum cards
            if (cardsPlayedThisTurn >= cardsPerTurn)
            {
                Debug.Log("Maximum cards played, ending turn automatically");
                EndPlayerTurn();
            }
        }
        return success;
    }

    private bool ShouldCardStayInField(CardSO card)
    {
        // Cards that provide ongoing effects or enable combos should stay in field
        return card is HelperTCellCardSO || 
               (card is CytotoxicCellCardSO && !HasHelperTInPlayedCards()) ||
               (card is BCellCardSO && !HasHelperTInPlayedCards());
    }

    private bool HasHelperTInPlayedCards()
    {
        var playedCards = playerManager.GetPlayedCards();
        foreach (var card in playedCards)
        {
            if (card is HelperTCellCardSO)
                return true;
        }
        return false;
    }

    public void DrawCard()
    {
        if (currentPhase == TurnPhase.PlayerTurn && !isProcessingTurn)
        {
            playerManager.DrawCards(1);
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        }
    }

    public void HealPlayer()
    {
        if (currentPhase == TurnPhase.PlayerTurn && !isProcessingTurn)
        {
            playerManager.HealPlayer(10);
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        }
    }

    void CheckWinConditions()
    {
        // Check if all pathogens are defeated
        if (pathogenManager != null && pathogenManager.IsPathogenAllDefeated())
        {
            EndGame(true); // Player wins
        }
        
        // Check if player is defeated
        if (playerManager.GetPlayer().HP <= 0)
        {
            EndGame(false); // Player loses
        }
    }

    void EndGame(bool playerWon)
    {
        currentPhase = TurnPhase.GameOver;
        OnTurnPhaseChanged?.Invoke(currentPhase);
        
        if (gameStateManager != null)
        {
            gameStateManager.EndGame(playerWon);
        }
        
        Debug.Log(playerWon ? "Player Wins!" : "Player Loses!");
    }

    #region Debug Methods
    
    [ContextMenu("Force Start Pathogen Turn")]
    public void ForceStartPathogenTurn()
    {
        Debug.Log("=== FORCE STARTING PATHOGEN TURN ===");
        if (currentPhase == TurnPhase.PathogenTurn)
        {
            Debug.Log("Already in pathogen turn phase");
            return;
        }
        
        StartPathogenTurn();
    }
    
    [ContextMenu("Debug Turn State")]
    public void DebugTurnState()
    {
        Debug.Log("=== Turn Manager State ===");
        Debug.Log($"Current Phase: {currentPhase}");
        Debug.Log($"Turn Number: {turnNumber}");
        Debug.Log($"Is Processing Turn: {isProcessingTurn}");
        Debug.Log($"Cards Played This Turn: {cardsPlayedThisTurn}/{cardsPerTurn}");
        Debug.Log($"Turn Timer: {turnTimer:F1}s");
        
        if (pathogenManager != null)
        {
            Debug.Log($"Active Pathogens: {pathogenManager.GetActivePathogenCount()}");
            Debug.Log($"Has Active Pathogens: {pathogenManager.HasActivePathogens()}");
        }
        else
        {
            Debug.Log("PathogenManager is NULL!");
        }
        
        if (playerManager != null)
        {
            Debug.Log($"Player HP: {playerManager.GetPlayer()?.HP ?? -1}");
        }
        else
        {
            Debug.Log("PlayerManager is NULL!");
        }
    }
    
    #endregion

    // Public getters for UI and other systems
    public TurnPhase GetCurrentPhase() => currentPhase;
    public bool IsPlayerTurn() => currentPhase == TurnPhase.PlayerTurn;
    public bool IsPathogenTurn() => currentPhase == TurnPhase.PathogenTurn;
    public int GetTurnNumber() => turnNumber;
    public float GetTurnTimeRemaining() => turnTimer;
    public PlayerManager GetPlayerManager() => playerManager;
    public PathogenManager GetPathogenManager() => pathogenManager;
    public List<CardSO> GetPlayerHand() => playerManager?.GetPlayerHand() ?? new List<CardSO>();
}
