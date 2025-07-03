using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Main game manager that coordinates game state, pathogens, and card blocking
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private Player player;
    [SerializeField] private List<PathogenSO> activePathogens = new List<PathogenSO>();
    [SerializeField] private PlayerHandUI playerHandUI;
    [SerializeField] private DeckManager deckManager;
    
    [Header("Turn Management")]
    [SerializeField] private int currentTurn = 1;
    [SerializeField] private bool isPlayerTurn = true;
    
    // Events
    public System.Action<int> OnTurnStart;
    public System.Action<int> OnTurnEnd;
    public System.Action<PathogenSO> OnPathogenAdded;
    public System.Action<PathogenSO> OnPathogenRemoved;
    public System.Action OnGameOver;
    public System.Action OnGameWon;
    
    #region Initialization
    
    void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        // Find components if not assigned
        if (playerHandUI == null)
            playerHandUI = FindFirstObjectByType<PlayerHandUI>();
            
        if (deckManager == null)
            deckManager = FindFirstObjectByType<DeckManager>();
        
        // Create player if not assigned
        if (player == null)
        {
            player = new Player(100); // Starting HP
        }
        
        // Set player reference in PlayerHandUI
        if (playerHandUI != null && player != null)
        {
            playerHandUI.SetPlayer(player);
        }
        
        StartPlayerTurn();
    }
    
    #endregion
    
    #region Pathogen Management
    
    public void AddPathogen(PathogenSO pathogen)
    {
        if (pathogen != null && !activePathogens.Contains(pathogen))
        {
            activePathogens.Add(pathogen);
            OnPathogenAdded?.Invoke(pathogen);
            
            // Pathogen added - PlayerHandUI will automatically update on next refresh
            Debug.Log($"Pathogen added: {pathogen.name}");
        }
    }
    
    public void RemovePathogen(PathogenSO pathogen)
    {
        if (activePathogens.Contains(pathogen))
        {
            activePathogens.Remove(pathogen);
            OnPathogenRemoved?.Invoke(pathogen);
            
            // Pathogen removed - PlayerHandUI will automatically update on next refresh
            Debug.Log($"Pathogen removed: {pathogen.name}");
            
            // Check win condition
            if (activePathogens.Count == 0)
            {
                OnGameWon?.Invoke();
            }
        }
    }
    
    public List<PathogenSO> GetActivePathogens()
    {
        return new List<PathogenSO>(activePathogens);
    }
    
    #endregion
    
    #region Card Blocking System
    
    public bool IsCardBlocked(System.Type cardType)
    {
        // Check if any active pathogen blocks this card type
        foreach (var pathogen in activePathogens)
        {
            if (pathogen != null && pathogen.IsCardBlocked(cardType))
            {
                return true;
            }
        }
        return false;
    }
    
    public List<PathogenSO> GetPathogensBlockingCard(System.Type cardType)
    {
        return activePathogens.Where(p => p != null && p.IsCardBlocked(cardType)).ToList();
    }
    
    #endregion
    
    #region Turn Management
    
    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        currentTurn++;
        
        Debug.Log($"Player Turn {currentTurn} started");
        
        // Reset player stats for new turn
        if (player != null)
            player.ResetTurnStats();
        
        // Draw cards from deck
        if (deckManager != null && player != null)
        {
            // Draw cards at start of turn (you can adjust the number as needed)
            for (int i = 0; i < 3; i++) // Draw 3 cards per turn
            {
                CardSO drawnCard = deckManager.DrawCard();
                if (drawnCard != null)
                {
                    player.AddCardToHand(drawnCard);
                    Debug.Log($"Drew card: {drawnCard.cardName}");
                }
            }
        }
        
        // Notify pathogens of turn start
        foreach (var pathogen in activePathogens)
        {
            if (pathogen != null)
            {
                var playedCards = player?.PlayedCards ?? new List<CardSO>();
                pathogen.OnTurnStart(playedCards);
            }
        }
        
        OnTurnStart?.Invoke(currentTurn);
    }
    
    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;
        
        Debug.Log($"Player Turn {currentTurn} ended");
        
        OnTurnEnd?.Invoke(currentTurn);
        
        // Start pathogen turn
        StartPathogenTurn();
    }
    
    public void StartPathogenTurn()
    {
        isPlayerTurn = false;
        
        Debug.Log("Pathogen Turn started");
        
        // Process pathogen attacks
        foreach (var pathogen in activePathogens.ToList()) // ToList to avoid modification during iteration
        {
            if (pathogen != null && player != null)
            {
                pathogen.AttackPlayer(player);
                
                // Check if player is defeated
                if (player.HP <= 0)
                {
                    OnGameOver?.Invoke();
                    return; // Stop processing if game is over
                }
            }
        }
        
        // Automatically start next player turn after a delay
        Invoke(nameof(StartPlayerTurn), 2f);
    }
    
    #endregion
    
    #region Game Events
    
    private void OnCardPlayedHandler(CardSO card)
    {
        Debug.Log($"Card played: {card.cardName}");
        
        // Update pathogen states if needed
        foreach (var pathogen in activePathogens)
        {
            if (pathogen != null)
            {
                // You can add specific card reaction logic here
            }
        }
    }
    
    #endregion
    
    #region Game State Queries
    
    public bool IsPlayerTurn() => isPlayerTurn;
    public int GetCurrentTurn() => currentTurn;
    public Player GetPlayer() => player;
    
    public bool IsGameOver()
    {
        return player != null && player.HP <= 0;
    }
    
    public bool IsGameWon()
    {
        return activePathogens.Count == 0;
    }
    
    #endregion
    
    #region Public Methods
    
    // Method to manually trigger turn end (for UI button)
    public void EndTurnButtonPressed()
    {
        if (isPlayerTurn)
        {
            EndPlayerTurn();
        }
    }
    
    // Method to add a pathogen manually (for testing or events)
    public void SpawnPathogen(PathogenSO pathogenPrefab)
    {
        if (pathogenPrefab != null)
        {
            // Create instance of the pathogen
            PathogenSO newPathogen = Instantiate(pathogenPrefab);
            AddPathogen(newPathogen);
        }
    }
    
    #endregion
}
