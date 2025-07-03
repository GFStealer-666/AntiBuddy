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
    
    private GameStateManager gameStateManager;
    private PlayerManager playerManager;
    private PathogenManager pathogenManager;
    private CardField cardField;

    [Header("Turn Settings")]
    public int maxTurns = 50;
    public float turnTimeLimit = 60f; // seconds per turn
    public int cardsPerTurn = 2; // Maximum cards per turn
    public float pathogenTurnDelay = 15f; // Delay before pathogen attacks
    
    private TurnPhase currentPhase;
    private int turnNumber;
    private float turnTimer;
    private float pathogenTimer;
    private bool isProcessingTurn;
    private bool isWaitingForPathogenTurn;
    private int cardsPlayedThisTurn;
    private int cardsPlayedLastTurn; // Track cards played in previous turn for drawing

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
            HandlePathogenTimer();
            HandleInput();
        }
    }

    private void InitializeGame()
    {
        gameStateManager = GameStateManager.Instance;
        
        // Initialize player with deck manager
        Player player = new Player(100);
        playerManager = new PlayerManager(player, deckManager);
        pathogenManager = FindFirstObjectByType<PathogenManager>();
        
        if (pathogenManager == null)
        {
            pathogenManager = gameObject.AddComponent<PathogenManager>();
        }

        // Initialize CardField
        cardField = FindFirstObjectByType<CardField>();
        if (cardField == null)
        {
            GameObject cardFieldObj = new GameObject("CardField");
            cardField = cardFieldObj.AddComponent<CardField>();
        }

        currentPhase = TurnPhase.PlayerTurn;
        turnNumber = 1;
        turnTimer = turnTimeLimit;
        pathogenTimer = 0f;
        isWaitingForPathogenTurn = false;
        cardsPlayedThisTurn = 0;
        cardsPlayedLastTurn = 0; // Initialize to 0 for first turn
        
        StartPlayerTurn();
    }

    private void HandleTurnTimer()
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

    private void HandlePathogenTimer()
    {
        if (isWaitingForPathogenTurn)
        {
            pathogenTimer -= Time.deltaTime;
            if (pathogenTimer <= 0)
            {
                isWaitingForPathogenTurn = false;
                ExecutePathogenTurn();
            }
        }
    }

    void HandleInput()
    {
        if (currentPhase == TurnPhase.PlayerTurn)
        {
            // Example input handling - in a real game this would be handled by UI
            if (Input.GetKeyDown(KeyCode.H))
            {
                HealPlayer();
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EndPlayerTurn();
            }
        }
    }

    public void StartPlayerTurn()
    {
        if (isProcessingTurn) return;
        
        isProcessingTurn = true;
        currentPhase = TurnPhase.PlayerTurn;
        turnTimer = turnTimeLimit;
        cardsPlayedThisTurn = 0; // Reset cards played counter
        
        Debug.Log($"=== Player Turn {turnNumber} Started ===");
        Debug.Log($"Cards allowed to play this turn: {cardsPerTurn}");
        
        // For the first turn, draw 5 cards (fill hand to max). After that, draw cards equal to cards played last turn
        int cardsToDraw = (turnNumber == 1) ? 5 : cardsPlayedLastTurn;
        Debug.Log($"Cards to draw: {cardsToDraw}");
        
        // Pass the number of cards to draw based on previous turn
        playerManager.StartTurn(cardsToDraw);
        
        // Reset card activation status for new turn
        ResetCardActivations();
        
        OnTurnPhaseChanged?.Invoke(currentPhase);
        OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        
        isProcessingTurn = false;
    }

    public void EndPlayerTurn()
    {
        if (isProcessingTurn || currentPhase != TurnPhase.PlayerTurn) return;
        
        isProcessingTurn = true;
        Debug.Log("=== Player Turn Ended ===");
        Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}");
        
        // Store cards played this turn for next turn's drawing
        cardsPlayedLastTurn = cardsPlayedThisTurn;
        Debug.Log($"Next turn will draw {cardsPlayedLastTurn} cards");
        
        // Apply any pending card effects during end phase
        ApplyEndPhaseEffects();
        
        // Clear the card field after effects are processed
        if (cardField != null)
        {
            cardField.ClearField();
            Debug.Log("Card field cleared");
        }
        
        CheckWinConditions();
        
        if (currentPhase != TurnPhase.GameOver)
        {
            StartPathogenTurn();
        }
        
        isProcessingTurn = false;
    }

    public void StartPathogenTurn()
    {
        if (isProcessingTurn) return;
        
        isProcessingTurn = true;
        currentPhase = TurnPhase.PathogenTurn;
        
        Debug.Log("=== Pathogen Turn Started ===");
        Debug.Log($"Pathogen will attack in {pathogenTurnDelay} seconds...");
        OnTurnPhaseChanged?.Invoke(currentPhase);
        
        // Start the pathogen turn delay
        pathogenTimer = pathogenTurnDelay;
        isWaitingForPathogenTurn = true;
        
        isProcessingTurn = false;
    }

    void ExecutePathogenTurn()
    {
        Debug.Log("=== Executing Pathogen Actions ===");
        
        // Pathogen attacks player
        if (pathogenManager != null)
        {
            pathogenManager.PathogenAttack(playerManager.GetPlayer());
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        }
        
        // Check if player is defeated
        if (playerManager.GetPlayer().HP <= 0)
        {
            EndGame(false); // Player loses
            return;
        }
        
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

    public bool PlayCard(CardSO card, PathogenSO target = null)
    {
        if (currentPhase != TurnPhase.PlayerTurn || isProcessingTurn)
        {
            Debug.LogWarning("Cannot play card outside of player turn");
            return false;
        }
        
        if (cardsPlayedThisTurn >= cardsPerTurn)
        {
            Debug.LogWarning($"Cannot play more cards this turn. Limit: {cardsPerTurn}");
            EndPlayerTurn();
            return false;
        }
        
        // Try to play card through PlayerManager and place it in CardField
        bool success = playerManager.PlayCard(card, target);
        if (success)
        {
            // Place card in field for tracking and combo effects
            if (cardField != null && cardField.TryPlayCardToField(card))
            {
                cardsPlayedThisTurn++;
                
                // Check if card can activate immediately or needs combo
                ProcessCardActivation(card, target);
                
                OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
                Debug.Log($"Cards played this turn: {cardsPlayedThisTurn}/{cardsPerTurn}");
                
                // Auto-end turn if player has played maximum cards
                if (cardsPlayedThisTurn >= cardsPerTurn)
                {
                    Debug.Log("Maximum cards played, ending turn automatically");
                    EndPlayerTurn();
                }
            }
            else
            {
                Debug.LogWarning("Failed to place card in field!");
                return false;
            }
        }
        return success;
    }

    private void ProcessCardActivation(CardSO card, PathogenSO target = null)
    {
        // Cards now handle their own activation logic internally
        var cardsInField = cardField?.GetActiveCards() ?? new List<CardSO>();
        
        Debug.Log($"Processing activation for {card.cardName}");
        card.ApplyEffect(playerManager.GetPlayer(), cardsInField, target);
        
        // After playing this card, check if any previously played cards can now activate
        CheckForRetroactiveActivations(target);
    }
    
    private void CheckForRetroactiveActivations(PathogenSO target = null)
    {
        if (cardField == null) return;
        
        var cardsInField = cardField.GetActiveCards();
        
        // Check all cards in field to see if any can now activate due to new card
        foreach (var card in cardsInField)
        {
            if (card is ImmuneCardSO immuneCard && !immuneCard.hasActivatedThisTurn)
            {
                Debug.Log($"Checking retroactive activation for {card.cardName}");
                immuneCard.ApplyEffect(playerManager.GetPlayer(), cardsInField, target);
            }
        }
    }

    private bool HasHelperTInField()
    {
        if (cardField == null) return false;
        return cardField.HasCardTypeInField<HelperTCellCardSO>();
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

    public void HealPlayer()
    {
        if (currentPhase == TurnPhase.PlayerTurn && !isProcessingTurn)
        {
            playerManager.HealPlayer(10);
            OnPlayerStatsChanged?.Invoke(playerManager.GetPlayerStats());
        }
    }

    private void ApplyEndPhaseEffects()
    {
        // Apply any pending card effects that should trigger at end of turn
        // This ensures effects are applied even if the turn ends due to card limit or timer
        
        if (cardField != null)
        {
            var cardsInField = cardField.GetActiveCards();
            Debug.Log($"Applying end phase effects for {cardsInField.Count} cards in field");
            
            // Try to activate any cards that haven't been activated yet
            foreach (var card in cardsInField)
            {
                if (card is ImmuneCardSO immuneCard && !immuneCard.hasActivatedThisTurn)
                {
                    Debug.Log($"End phase activation attempt for {card.cardName}");
                    immuneCard.ApplyEffect(playerManager.GetPlayer(), cardsInField, null);
                }
            }
            
            // Check for field-wide effects (cytokine storm, memory response, etc.)
            cardField.CheckForFieldEffects(playerManager.GetPlayer(), null); // TODO: Get current pathogen target
        }
        
        // Note: Cards now handle their own activation logic internally
    }

    void CheckWinConditions()
    {
        // Check if all pathogens are defeated
        if (pathogenManager != null && pathogenManager.AllPathogensDefeated())
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

    // Public getters for UI and other systems
    public TurnPhase GetCurrentPhase() => currentPhase;
    public int GetTurnNumber() => turnNumber;
    public float GetTurnTimeRemaining() => turnTimer;
    public float GetPathogenTimeRemaining() => pathogenTimer;
    public bool IsWaitingForPathogen() => isWaitingForPathogenTurn;
    public PlayerManager GetPlayerManager() => playerManager;
    public PathogenManager GetPathogenManager() => pathogenManager;
    public List<CardSO> GetPlayerHand() => playerManager?.GetPlayerHand() ?? new List<CardSO>();
    public List<CardSO> GetPlayedCards() => playerManager?.GetPlayedCards() ?? new List<CardSO>();
    public List<CardSO> GetCardsInField() => cardField?.GetActiveCards() ?? new List<CardSO>();
    public CardField GetCardField() => cardField;
    public int GetCardsPlayedThisTurn() => cardsPlayedThisTurn;
    public int GetCardsPlayedLastTurn() => cardsPlayedLastTurn;
    public int GetMaxCardsPerTurn() => cardsPerTurn;
    public bool CanPlayMoreCards() => cardsPlayedThisTurn < cardsPerTurn;

    private void ResetCardActivations()
    {
        // Reset activation status for all cards in player's hand and played cards
        var allCards = new List<CardSO>();
        allCards.AddRange(playerManager.GetPlayerHand());
        allCards.AddRange(playerManager.GetPlayedCards());
        if (cardField != null)
            allCards.AddRange(cardField.GetActiveCards());
        
        foreach (var card in allCards)
        {
            if (card is ImmuneCardSO immuneCard)
            {
                immuneCard.ResetActivation();
            }
        }
        
        Debug.Log("Card activations reset for new turn");
    }
}
