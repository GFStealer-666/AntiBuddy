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
    // Card field system will be added later when compilation issues are resolved
    
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
        
        // Initialize player with deck manager
        Player player = new Player(100);
        playerManager = new PlayerManager(player, deckManager);
        pathogenManager = FindFirstObjectByType<PathogenManager>();
        
        if (pathogenManager == null)
        {
            pathogenManager = gameObject.AddComponent<PathogenManager>();
        }

        /*
        if (cardField == null)
        {
            cardField = FindFirstObjectByType<CardField>();
            if (cardField == null)
            {
                GameObject cardFieldObj = new GameObject("CardField");
                cardField = cardFieldObj.AddComponent<CardField>();
            }
        }
        */

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
        if (currentPhase == TurnPhase.PlayerTurn)
        {
            // Example input handling - in a real game this would be handled by UI
            if (Input.GetKeyDown(KeyCode.D))
            {
                DrawCard();
            }
            
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
        Debug.Log($"Cards allowed this turn: {cardsPerTurn}");
        
        playerManager.StartTurn();
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
        
        // Clear the card field after effects are processed
        /*
        if (cardField != null)
        {
            cardField.ClearField();
        }
        */
        
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
        OnTurnPhaseChanged?.Invoke(currentPhase);
        
        // Execute pathogen actions
        ExecutePathogenTurn();
        
        isProcessingTurn = false;
    }

    void ExecutePathogenTurn()
    {
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
            // Auto-end turn when limit reached
            EndPlayerTurn();
            return false;
        }
        
        bool success = playerManager.PlayCard(card, target);
        if (success)
        {
            cardsPlayedThisTurn++;
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
    public PlayerManager GetPlayerManager() => playerManager;
    public PathogenManager GetPathogenManager() => pathogenManager;
    public List<CardSO> GetPlayerHand() => playerManager?.GetPlayerHand() ?? new List<CardSO>();
    public int GetCardsPlayedThisTurn() => cardsPlayedThisTurn;
    public int GetMaxCardsPerTurn() => cardsPerTurn;
    public bool CanPlayMoreCards() => cardsPlayedThisTurn < cardsPerTurn;
}
