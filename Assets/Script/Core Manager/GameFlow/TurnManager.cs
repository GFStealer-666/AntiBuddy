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
    
    [Header("Turn Transition Delays")]
    public float endPlayerTurnDelay = 1.5f; // Delay before starting pathogen turn
    public float pathogenTurnDelay = 2f; // Delay during pathogen actions
    public float startPlayerTurnDelay = 1f; // Delay before starting next player turn
    
    private TurnPhase currentPhase;
    private int turnNumber;
    private float turnTimer;
    private bool isProcessingTurn;
    private int cardsPlayedThisTurn;

    public static event Action<TurnPhase> OnTurnPhaseChanged;
    public static event Action<int> OnTurnNumberChanged;
    public static event Action<PlayerStats> OnPlayerStatsChanged;
    public static event Action<int, int> OnCardLimitChanged; // cardsPlayed, cardLimit

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (gameStateManager != null && gameStateManager.IsGameInProgress() && !isProcessingTurn)
        {
            HandleTurnTimer();
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
        
        Debug.Log(playerManager == null ? "TurnManager: Created new PlayerManager" : "TurnManager: Found existing PlayerManager");
        
        // Subscribe to player death event for immediate game over check
        if (playerManager != null && playerManager.GetPlayer() != null)
        {
            playerManager.GetPlayer().PlayerHealth.OnPlayerDied += OnPlayerDied;
        }
        
        // Find or create PathogenManager
        pathogenManager = FindFirstObjectByType<PathogenManager>();
        if (pathogenManager == null)
        {
            pathogenManager = gameObject.AddComponent<PathogenManager>();
        }
        
        // Subscribe to pathogen victory event for immediate win condition check
        if (pathogenManager != null)
        {
            pathogenManager.OnAllPathogensDefeated += OnAllPathogensDefeated;
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
        
        Debug.Log(playerManager != null ? "TurnManager: PlayerManager reference is valid" : "TurnManager: PlayerManager reference is null");
    }

    void HandleTurnTimer()
    {
        if (currentPhase == TurnPhase.PlayerTurn)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                Debug.Log("Turn time limit reached, ending player turn");
                EndPlayerTurn("time limit reached");
            }
        }
    }


    public void StartPlayerTurn()
    {
        if (isProcessingTurn) return;
        
        isProcessingTurn = true;
        currentPhase = TurnPhase.PlayerTurn;
        turnTimer = turnTimeLimit;
        
        // Draw cards: 5 on first turn, then equal to cards played last turn
        int cardsToDraw = (turnNumber == 1) ? 5 : cardsPlayedThisTurn;
        playerManager.StartTurn(cardsToDraw);
        cardsPlayedThisTurn = 0; // Reset cards played counter
        
        // Clear the card field now - before new turn starts
        if (cardField != null)
        {
            Debug.Log("TurnManager: Clearing field for new turn");
            cardField.ClearField();
        }
        
        // Reset temporary effects that last only until next turn
        if (playerManager != null)
        {
            playerManager.GetPlayer().ResetTemporaryEffects();
            Debug.Log("TurnManager: Reset temporary effects for new turn");
        }
        
        Debug.Log($"=== Player Turn {turnNumber} Started ===");
        Debug.Log($"Cards allowed this turn: {cardsPerTurn}");
        Debug.Log($"Cards to draw: {cardsToDraw} (Turn {turnNumber}: {(turnNumber == 1 ? "starting hand" : "based on last turn's plays")})");
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
        OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        OnCardLimitChanged?.Invoke(cardsPlayedThisTurn, cardsPerTurn); // Notify UI of card limit status
        
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
        
        // Log turn end via GameManager for UI display
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogTurnEnd();
        }
        
        Debug.Log("Cards remain in field for pathogen turn...");
        
        // DON'T clear the field here - keep cards visible during pathogen turn
        // Player can see what they played while pathogen responds
        
        CheckWinConditions();
        
        if (currentPhase != TurnPhase.GameOver)
        {
            Debug.Log($"TurnManager: Starting pathogen turn in {endPlayerTurnDelay} seconds...");
            // Add delay before pathogen turn starts
            Invoke(nameof(DelayedStartPathogenTurn), endPlayerTurnDelay);
        }
        else
        {
            Debug.Log("TurnManager: Game is over, not starting pathogen turn");
            isProcessingTurn = false;
        }
    }

    public void EndPlayerTurn(string reason)
    {
        if (isProcessingTurn || currentPhase != TurnPhase.PlayerTurn) 
        {
            Debug.LogWarning($"TurnManager: Cannot end player turn - isProcessingTurn: {isProcessingTurn}, currentPhase: {currentPhase}");
            return;
        }
        
        isProcessingTurn = true;
        Debug.Log($"=== Player Turn Ended: {reason} ===");
        Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}");
        
        // Log turn end with reason via GameManager for UI display
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogTurnEnd(reason);
        }
        
        Debug.Log("Cards remain in field for pathogen turn...");
        
        // DON'T clear the field here - keep cards visible during pathogen turn
        // Player can see what they played while pathogen responds
        
        CheckWinConditions();
        
        if (currentPhase != TurnPhase.GameOver)
        {
            Debug.Log($"TurnManager: Starting pathogen turn in {endPlayerTurnDelay} seconds...");
            // Add delay before pathogen turn starts
            Invoke(nameof(DelayedStartPathogenTurn), endPlayerTurnDelay);
        }
        else
        {
            Debug.Log("TurnManager: Game is over, not starting pathogen turn");
            isProcessingTurn = false;
        }
    }
    
    private void DelayedStartPathogenTurn()
    {
        isProcessingTurn = false;
        StartPathogenTurn();
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
        
        // Check if there are any active pathogens at start
        if (!pathogenManager.HasActivePathogens())
        {
            Debug.Log("TurnManager: No active pathogens at start of turn, ending pathogen turn");
            EndPathogenTurn();
            return;
        }
        
        // Get the played cards from this turn to pass to pathogen abilities
        var playedCards = playerManager.GetPlayedCards();
        
        // Execute pathogen turn with proper error handling and delay
        try
        {
            Debug.Log("TurnManager: Pathogen is responding to your actions...");
            
            // Add delay before pathogen acts for dramatic effect
            Invoke(nameof(DelayedPathogenActions), pathogenTurnDelay);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TurnManager: Error during pathogen turn execution: {e.Message}");
            EndPathogenTurn();
        }
    }
    
    private void DelayedPathogenActions()
    {
        if (pathogenManager == null || playerManager == null) 
        {
            EndPathogenTurn();
            return;
        }
        
        var playedCards = playerManager.GetPlayedCards();
        
        pathogenManager.ExecutePathogenTurn(playerManager.GetPlayer(), playedCards);
        OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        
        Debug.Log("TurnManager: Pathogen turn executed successfully");
        
        // Check if player is defeated
        if (playerManager.GetPlayer().HP <= 0)
        {
            EndGame(false); // Player loses
            return;
        }
        
        // End pathogen turn and start next player turn with delay
        Invoke(nameof(DelayedEndPathogenTurn), startPlayerTurnDelay);
    }
    
    private void DelayedEndPathogenTurn()
    {
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
        
        // Check if this is an item or regular card
        bool isItem = card is ItemSO;
        
        // Only count regular cards toward the turn limit, items are unlimited
        if (!isItem && cardsPlayedThisTurn >= cardsPerTurn)
        {
            Debug.LogWarning($"Cannot play more cards this turn. Limit: {cardsPerTurn} (Items don't count toward limit)");
            // Don't auto-end turn - let player decide when to end
            return false;
        }
        
        bool success;
        if (isItem)
        {
            // For items, check if it's in inventory and use the inventory method
            ItemSO item = card as ItemSO;
            if (playerManager.GetPlayer().PlayerInventory.HasItem(item))
            {
                success = playerManager.UseInventoryItem(item, target);
            }
            else
            {
                // Item might be in hand (not purchased yet), use regular play method
                success = playerManager.PlayCard(card, target);
            }
        }
        else
        {
            // For regular cards, use the standard method
            success = playerManager.PlayCard(card, target);
        }
        if (success)
        {
            // Only increment counter for regular cards, not items
            if (!isItem)
            {
                cardsPlayedThisTurn++;
            }
            
            // Add card to field if it has no immediate effect (like Helper T-Cell or inactive Cytotoxic)
            // Items typically don't stay in field, only immune cells do
            bool shouldStayInField = ShouldCardStayInField(card);
            if (shouldStayInField && cardField != null)
            {
                cardField.TryPlayCardToField(card);
            }
            
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
            
            if (isItem)
            {
                Debug.Log($"Used item: {card.cardName} (Items don't count toward turn limit)");
            }
            else
            {
                Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}/{cardsPerTurn}");
                
                // Notify UI that card limit status has changed
                OnCardLimitChanged?.Invoke(cardsPlayedThisTurn, cardsPerTurn);
            }
        }
        return success;
    }

    public void PlayCard(CardSO card)
    {
        if (card == null)
        {
            Debug.LogWarning("TurnManager: Cannot play a null card");
            return;
        }

        Debug.Log($"TurnManager: Attempting to play card {card.cardName}");

        // Try to add the card to the field
        var cardField = FindFirstObjectByType<CardField>();
        if (cardField != null)
        {
            bool success = cardField.TryPlayCardToField(card);
            if (success)
            {
                Debug.Log($"TurnManager: Successfully played {card.cardName} to the field");

                // Refresh PlayerHandUI
                var playerUI = FindFirstObjectByType<PlayerUI>();
                if (playerUI != null && playerUI.HandUI != null)
                {
                    playerUI.HandUI.RefreshHand();
                }
                else
                {
                    Debug.LogWarning("TurnManager: PlayerUI or PlayerHandUI not found");
                }
            }
            else
            {
                Debug.LogWarning($"TurnManager: Failed to play {card.cardName} to the field");
            }
        }
        else
        {
            Debug.LogError("TurnManager: CardField not found");
        }
    }
    
    private bool ShouldCardStayInField(CardSO card)
    {
        // Items don't stay in field - they have immediate effects
        if (card is ItemSO)
            return false;
            
        // Only immune cells stay in field for ongoing effects or enable combos
        return card is CardSO && !(card is ItemSO);
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

    /// <summary>
    /// Process combo effects at end of turn to ensure proper activation regardless of play order
    /// </summary>
    private void ProcessEndOfTurnCombos()
    {
        if (playerManager == null) return;
        
        var playedCards = playerManager.GetPlayedCards();
        if (playedCards.Count == 0) return;
        
        Debug.Log($"TurnManager: Processing end-of-turn combos for {playedCards.Count} cards");
        
        // Re-process combo cards now that all cards are in the played list
        var currentTarget = pathogenManager?.GetCurrentPathogen();
        
        foreach (var card in playedCards)
        {
            // Only re-process combo cards that need Helper T-Cell
            if (card is BCellCardSO || card is CytotoxicCellCardSO)
            {
                Debug.Log($"TurnManager: Re-processing combo card: {card.cardName}");
                card.ApplyEffect(playerManager.GetPlayer(), playedCards, currentTarget);
            }
        }
        
        Debug.Log("TurnManager: End-of-turn combo processing complete");
    }

    /// <summary>
    /// Called immediately when player dies - ensures instant game over
    /// </summary>
    void OnPlayerDied()
    {
        Debug.Log("TurnManager: Player died! Ending game immediately.");
        EndGame(false); // Player loses
    }

    /// <summary>
    /// Called immediately when all pathogens are defeated - ensures instant victory
    /// </summary>
    void OnAllPathogensDefeated()
    {
        Debug.Log("TurnManager: All pathogens defeated! Player wins immediately.");
        EndGame(true); // Player wins
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
    
    [ContextMenu("Test Turn Timing")]
    public void TestTurnTiming()
    {
        Debug.Log("=== Turn Timing Settings ===");
        Debug.Log($"End Player Turn Delay: {endPlayerTurnDelay}s");
        Debug.Log($"Pathogen Turn Delay: {pathogenTurnDelay}s");
        Debug.Log($"Start Player Turn Delay: {startPlayerTurnDelay}s");
        Debug.Log($"Total turn transition time: {endPlayerTurnDelay + pathogenTurnDelay + startPlayerTurnDelay}s");
    }
    
    [ContextMenu("Cancel Delayed Actions")]
    public void DebugCancelDelayedActions()
    {
        CancelDelayedTurnTransitions();
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
    
    // Card limit status methods
    public bool CanPlayMoreCards() => cardsPlayedThisTurn < cardsPerTurn;
    public int GetCardsPlayedThisTurn() => cardsPlayedThisTurn;
    public int GetCardLimit() => cardsPerTurn;
    public int GetRemainingCardPlays() => Mathf.Max(0, cardsPerTurn - cardsPlayedThisTurn);

    private void OnDestroy()
    {
        // Unsubscribe from player death event
        if (playerManager != null && playerManager.GetPlayer() != null)
        {
            playerManager.GetPlayer().PlayerHealth.OnPlayerDied -= OnPlayerDied;
        }
        
        // Unsubscribe from pathogen victory event
        if (pathogenManager != null)
        {
            pathogenManager.OnAllPathogensDefeated -= OnAllPathogensDefeated;
        }
        
        // Cancel any pending delayed actions when TurnManager is destroyed
        CancelInvoke();
    }
    
    /// <summary>
    /// Cancel all delayed turn transitions (useful for game restart/quit)
    /// </summary>
    public void CancelDelayedTurnTransitions()
    {
        CancelInvoke(nameof(DelayedStartPathogenTurn));
        CancelInvoke(nameof(DelayedPathogenActions));
        CancelInvoke(nameof(DelayedEndPathogenTurn));
        Debug.Log("TurnManager: Cancelled all delayed turn transitions");
    }
    
    /// <summary>
    /// Notify the system that a card has been played.
    /// </summary>
    /// <param name="card">The card that was played.</param>
    public void NotifyCardPlayed(CardSO card)
    {
        if (cardField != null)
        {
            cardField.TryPlayCardToField(card);
        }

        Debug.Log($"TurnManager: Notified that {card.cardName} was played.");
    }
}
