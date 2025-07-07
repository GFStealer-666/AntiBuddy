using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages Player, Deck, and Card operations
/// Handles all player-related actions and stats
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private TurnManager turnManager;
    private readonly int MAX_HAND_SIZE = 5;

    [SerializeField] private int defaultPlayerHealth = 100;

    // Events for other systems
    public event Action<PlayerStats> OnPlayerStatsChanged;
    public event Action<CardSO> OnCardPlayed;
    public event Action<int> OnPlayerHealed;
    
    [Header("Debug")]
    [SerializeField] private bool enableDrawDebug = true;
    [SerializeField] private bool showHandDebug = true;
    [SerializeField] private List<CardSO> currentHandDebug = new List<CardSO>();
    [SerializeField] private int handSizeDebug = 0;
    private static int totalCardDraws = 0;
    
    void Awake()
    {
        // Create default player if none exists - no dependencies needed
        if (player == null)
        {
            player = new Player(defaultPlayerHealth);
            Debug.Log("PlayerManager: Created new player with 100 HP");
        }
    }
    
    void Start()
    {
        // Find other managers
        deckManager = FindFirstObjectByType<DeckManager>();
        turnManager = FindFirstObjectByType<TurnManager>();
        
        // Subscribe to card events for debug updates
        if (player != null && player.PlayerCards != null)
        {
            player.PlayerCards.OnCardAddedToHand += OnCardAddedToHandDebug;
            player.PlayerCards.OnCardPlayed += OnCardPlayedDebug;
        }
        
        // Draw initial cards after ensuring DeckManager is ready
        StartCoroutine(DrawInitialCardsCoroutine());
    }
    
    void OnDestroy()
    {
        // Clean up event subscriptions
        if (player != null && player.PlayerCards != null)
        {
            player.PlayerCards.OnCardAddedToHand -= OnCardAddedToHandDebug;
            player.PlayerCards.OnCardPlayed -= OnCardPlayedDebug;
        }
    }
    
    void Update()
    {
        // Update debug info every frame if enabled
        if (showHandDebug && player != null)
        {
            UpdateHandDebugInfo();
        }
    }
    
    private System.Collections.IEnumerator DrawInitialCardsCoroutine()
    {
        // Wait one frame to ensure other managers are initialized
        yield return null;
        
        // Validate deck manager is available and ready
        if (deckManager == null)
        {
            deckManager = FindFirstObjectByType<DeckManager>();
        }
        
        if (deckManager != null)
        {
            Debug.Log("PlayerManager: Drawing initial 5 cards");
            DrawCards(5);
        }
        else
        {
            Debug.LogError("PlayerManager: DeckManager still not found after initialization delay!");
        }
    }

    #region Player Setup
    
    public void SetPlayer(Player p)
    {
        player = p;
        NotifyStatsChanged();
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetDeckManager(DeckManager deck)
    {
        deckManager = deck;
    }
    
    #endregion

    #region Turn Management
    
    public void StartTurn(int cardsToDraw)
    {
        // Use TurnManager's setting if not specified
            
        player.ResetTurnStats();
        DrawCards(cardsToDraw);
        
        Debug.Log($"PlayerManager: Turn started - Drew {cardsToDraw} cards");
        NotifyStatsChanged();
    }
    
    #endregion

    #region Player Actions
    
    public void HealPlayer(int amount = 10)
    {
        player.PlayerHealth.Heal(amount);
        Debug.Log($"PlayerManager: Player healed for {amount} HP");
        
        OnPlayerHealed?.Invoke(amount);
        NotifyStatsChanged();
    }

    public bool AttackPathogen(Pathogen pathogen, int damage)
    {
        if (pathogen != null && pathogen.IsAlive())
        {
            pathogen.TakeDamage(damage);
            Debug.Log($"PlayerManager: Attacked {pathogen.GetPathogenName()} for {damage} damage");
            return true;
        }
        return false;
    }
    
    #endregion

    #region Card Management
    
    public bool PlayCard(CardSO card, Pathogen target = null)
    {
        if (!player.Hand.Contains(card))
        {
            Debug.LogWarning("PlayerManager: Card not in player hand");
            return false;
        }
        
        // Play the card
        if (player.PlayCard(card, target))
        {
            OnCardPlayed?.Invoke(card);
            NotifyStatsChanged();
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Use an item from the player's inventory (bypasses hand check)
    /// </summary>
    public bool UseInventoryItem(ItemSO item, Pathogen target = null)
    {
        // Check if item is in inventory
        if (!player.PlayerInventory.HasItem(item))
        {
            Debug.LogWarning($"PlayerManager: Item {item.cardName} not in player inventory");
            return false;
        }
        
        Debug.Log($"PlayerManager: Using inventory item {item.cardName}");
        
        // Apply the item effect directly
        item.ApplyEffect(player, player.PlayedCards, target);
        
        // Consume the item from inventory (handles events and removal)
        bool consumed = player.PlayerInventory.ConsumeItem(item);
        if (!consumed)
        {
            Debug.LogWarning($"PlayerManager: Failed to consume {item.cardName}");
            return false;
        }
        
        // Log the item use via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogItemUse(item.cardName);
        }
        
        Debug.Log($"PlayerManager: Used {item.cardName}");
        
        OnCardPlayed?.Invoke(item);
        NotifyStatsChanged();
        return true;
    }

    public void DrawCards(int count)
    {
        if (deckManager == null)
        {
            Debug.LogWarning("PlayerManager: No DeckManager assigned");
            return;
        }

        int cardsDrawn = 0;
        for (int i = 0; i < count; i++)
        {
            if (player.Hand.Count >= MAX_HAND_SIZE)
            {
                Debug.Log("PlayerManager: Hand is full, cannot draw more cards");
                break;
            }

            CardSO drawnCard = deckManager.DrawCard();
            if (drawnCard != null)
            {
                player.PlayerCards.AddCardToHand(drawnCard);
                cardsDrawn++;
            }
            else
            {
                Debug.Log("PlayerManager: No more cards to draw");
                break;
            }
        }
        
        Debug.Log($"PlayerManager: Drew {cardsDrawn} cards");
        
        totalCardDraws += cardsDrawn;
        if (enableDrawDebug)
        {
            Debug.Log($"PlayerManager: Total cards drawn this session: {totalCardDraws}");
            
            // Warning if too many draws
            if (totalCardDraws > 20)
            {
                Debug.LogError($"PlayerManager: Excessive card draws detected! Total: {totalCardDraws}");
            }
        }
        
        NotifyStatsChanged();
    }

    public bool CanPlayCard(CardSO card)
    {
        return player.Hand.Contains(card);
    }

    public void ApplyCardEffect(CardSO card, Pathogen target = null)
    {
        if (CanPlayCard(card))
        {
            card.ApplyEffect(player, player.PlayedCards, target);
            NotifyStatsChanged();
        }
    }
    
    #endregion

    #region Getters
    
    public List<CardSO> GetPlayerHand()
    {
        return new List<CardSO>(player.Hand);
    }

    public List<CardSO> GetPlayedCards()
    {
        return new List<CardSO>(player.PlayedCards);
    }

    public int GetHandSize()
    {
        return player.Hand.Count;
    }
    
    public int GetMaxHandSize()
    {
        return MAX_HAND_SIZE;
    }

    public PlayerStats GetPlayerStats()
    {
        return new PlayerStats
        {
            HP = player.HP,
            MaxHP = player.MaxHP,
            Defense = player.Defense,
            Tokens = player.Tokens,
            HandSize = player.Hand.Count,
            PlayedCardsCount = player.PlayedCards.Count
        };
    }
    
    #endregion
    
    #region Helper Methods
    
    private void NotifyStatsChanged()
    {
        OnPlayerStatsChanged?.Invoke(GetPlayerStats());
    }
    
    private void OnCardAddedToHandDebug(CardSO card)
    {
        if (showHandDebug)
        {
            UpdateHandDebugInfo();
            Debug.Log($"[Hand Debug] Added {card.cardName} to hand. Hand size: {player.PlayerCards.Hand.Count}");
        }
    }
    
    private void OnCardPlayedDebug(CardSO card)
    {
        if (showHandDebug)
        {
            UpdateHandDebugInfo();
            Debug.Log($"[Hand Debug] Played {card.cardName}. Hand size: {player.PlayerCards.Hand.Count}");
        }
    }
    
    private void UpdateHandDebugInfo()
    {
        if (player?.PlayerCards?.Hand != null)
        {
            currentHandDebug.Clear();
            currentHandDebug.AddRange(player.PlayerCards.Hand);
            handSizeDebug = player.PlayerCards.Hand.Count;
        }
        else
        {
            currentHandDebug.Clear();
            handSizeDebug = 0;
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Update Hand Debug Info")]
    public void ForceUpdateHandDebug()
    {
        UpdateHandDebugInfo();
        Debug.Log($"[Hand Debug] Forced update. Hand contains {handSizeDebug} cards");
    }
    
    [ContextMenu("Print Current Hand")]
    public void PrintCurrentHand()
    {
        if (player?.PlayerCards?.Hand != null)
        {
            Debug.Log($"=== CURRENT HAND ({player.PlayerCards.Hand.Count} cards) ===");
            for (int i = 0; i < player.PlayerCards.Hand.Count; i++)
            {
                var card = player.PlayerCards.Hand[i];
                Debug.Log($"{i + 1}. {card.cardName} ({card.GetType().Name})");
            }
        }
        else
        {
            Debug.Log("No hand data available");
        }
    }
    
    [ContextMenu("Toggle Hand Debug")]
    public void ToggleHandDebug()
    {
        showHandDebug = !showHandDebug;
        Debug.Log($"Hand debug {(showHandDebug ? "enabled" : "disabled")}");
        
        if (showHandDebug)
        {
            UpdateHandDebugInfo();
        }
    }
    
    #endregion

}

[System.Serializable]
public struct PlayerStats
{
    public int HP;
    public int MaxHP;
    public int Defense;
    public int Tokens;
    public int HandSize;
    public int PlayedCardsCount;
}


