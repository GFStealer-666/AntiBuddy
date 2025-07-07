using UnityEngine;
using System.Collections.Generic;

public class PlayerHandUI : MonoBehaviour, ICardActionHandler
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private CardActionPanel actionPanel;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PathogenManager pathogenManager;
    [SerializeField] private TurnManager turnManager;
    
    
    [Header("Settings")]
    [SerializeField] private float refreshRate = 0.1f;
    
    private List<GameObject> currentCardObjects = new List<GameObject>();
    private int lastHandCount = -1;
    private float lastRefreshTime = 0f;

    #region Unity Lifecycle
    void Awake()
    {
        InitializeComponents();
    }
    void Start()
    {
        RefreshHand();
        
        // Subscribe to card limit changes
        TurnManager.OnCardLimitChanged += OnCardLimitChanged;
    }
    
    void Update()
    {
        // Reduced polling frequency since we now have event-driven updates
        if (Time.time - lastRefreshTime > refreshRate * 5f) // 5x slower polling as backup
        {
            CheckForHandChanges();
            RefreshHand();
            lastRefreshTime = Time.time;
        }
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        // Unsubscribe from card limit changes
        TurnManager.OnCardLimitChanged -= OnCardLimitChanged;
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeComponents()
    {
        // Find turn manager if not assigned
        if (turnManager == null)
        {
            turnManager = FindFirstObjectByType<TurnManager>();
        }
        
        // Find pathogen manager if not assigned
        if (pathogenManager == null)
        {
            pathogenManager = FindFirstObjectByType<PathogenManager>();
        }
        
        // Subscribe to pathogen events
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned += OnPathogenChanged;
            pathogenManager.OnPathogenDefeated += OnPathogenChanged;
        }
        
        // Find player if not assigned
        if (player == null)
        {
            // Try to get player from PlayerManager first
            if (playerManager != null)
            {
                player = playerManager.GetPlayer();
            }
            else
            {
                // Fallback to GameManager
                var gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    player = gameManager.GetPlayer();
                }
            }
        }
        
        // Subscribe to player card events for immediate updates
        if (player?.PlayerCards != null)
        {
            player.PlayerCards.OnCardAddedToHand += OnCardAddedToHand;
            player.PlayerCards.OnCardPlayed += OnCardPlayed;
            Debug.Log("PlayerHandUI: Successfully subscribed to PlayerCards events");
        }
        else
        {
            Debug.LogWarning("PlayerHandUI: Could not subscribe to PlayerCards events - player or PlayerCards is null");
        }
        
        // Subscribe to inventory events for immediate updates when items are purchased
        if (player?.PlayerInventory != null)
        {
            player.PlayerInventory.OnItemAdded += OnItemAddedToInventory;
            player.PlayerInventory.OnItemRemoved += OnItemRemovedFromInventory;
            player.PlayerInventory.OnItemUsed += OnItemUsedFromInventory;
            Debug.Log($"PlayerHandUI: Successfully subscribed to PlayerInventory events. Current inventory count: {player.PlayerInventory.Items?.Count ?? 0}");
        }
        else
        {
            Debug.LogWarning("PlayerHandUI: Could not subscribe to PlayerInventory events - player or PlayerInventory is null");
            if (player == null) Debug.LogWarning("PlayerHandUI: player is null");
            if (player?.PlayerInventory == null) Debug.LogWarning("PlayerHandUI: player.PlayerInventory is null");
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= OnPathogenChanged;
            pathogenManager.OnPathogenDefeated -= OnPathogenChanged;
        }
        
        // Unsubscribe from player card events
        if (player?.PlayerCards != null)
        {
            player.PlayerCards.OnCardAddedToHand -= OnCardAddedToHand;
            player.PlayerCards.OnCardPlayed -= OnCardPlayed;
        }
        
        // Unsubscribe from inventory events
        if (player?.PlayerInventory != null)
        {
            player.PlayerInventory.OnItemAdded -= OnItemAddedToInventory;
            player.PlayerInventory.OnItemRemoved -= OnItemRemovedFromInventory;
            player.PlayerInventory.OnItemUsed -= OnItemUsedFromInventory;
        }
    }
    
    #endregion
    
    #region Hand Management
    
    private void CheckForHandChanges()
    {
        if (player == null) return;
        
        if (player.Hand.Count != lastHandCount)
        {
            RefreshHand();
            lastHandCount = player.Hand.Count;
        }
    }
    
    public void RefreshHand()
    {
        if (player == null || cardContainer == null) return;
        
        ClearCardUIs();
        
        int cardsAdded = 0;
        int itemsAdded = 0;
        
        // Add regular cards
        foreach (CardSO card in player.Hand)
        {
            CreateCardUI(card);
            cardsAdded++;
        }
        
        // Add items from inventory (items don't count toward hand limit)
        foreach (var inventorySlot in player.PlayerInventory.Items)
        {
            if (inventorySlot.item != null)
            {
                // ItemSO inherits from CardSO, so this should work
                CreateCardUI(inventorySlot.item);
                itemsAdded++;
            }
        }
        
        // Debug.Log($"PlayerHandUI: Refreshed hand - {cardsAdded} cards, {itemsAdded} items from inventory");
        
        // Update blocking status after creating all cards
        UpdateCardBlocking();
        
        // Update interactability based on card limits
        UpdateCardInteractability();
    }
    
    private void ClearCardUIs()
    {
        foreach (GameObject cardObj in currentCardObjects)
        {
            if (cardObj != null)
            {
                var cardUI = cardObj.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    cardUI.OnCardClicked -= OnCardClicked;
                }
                Destroy(cardObj);
            }
        }
        currentCardObjects.Clear();
    }
    private void CreateCardUI(CardSO card)
    {
        if (cardPrefab == null) return;

        GameObject cardObject = Instantiate(cardPrefab, cardContainer);
        CardUI cardUI = cardObject.GetComponent<CardUI>();

        if (cardUI != null)
        {
            cardUI.Initialize(card);
            cardUI.OnCardClicked += OnCardClicked;
            currentCardObjects.Add(cardObject);
        }
        else
        {
            Debug.LogError("Card prefab must have CardUI component!");
            Destroy(cardObject);
        }
    }
    
    #endregion
    
    #region Card Blocking
    
    /// <summary>
    /// Update which cards are blocked based on active pathogen abilities
    /// </summary>
    public void UpdateCardBlocking()
    {
        if (pathogenManager == null) return;
        
        var activePathogens = pathogenManager.GetActivePathogens();
        
        // Check each card UI for blocking
        foreach (GameObject cardObj in currentCardObjects)
        {
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI == null || cardUI.GetCardData() == null) continue;
            
            bool isBlocked = IsCardBlockedByAnyPathogen(cardUI.GetCardData(), activePathogens);
            cardUI.SetBlocked(isBlocked);
            
            if (isBlocked)
            {
                Debug.Log($"Card {cardUI.GetCardData().cardName} is BLOCKED by active pathogen!");
            }
        }
    }
    
    /// <summary>
    /// Update card interactability based on card limit and other conditions
    /// </summary>
    private void UpdateCardInteractability()
    {
        if (turnManager == null) return;
        
        bool canPlayMoreCards = turnManager.CanPlayMoreCards();
        
        // Update each card UI for interactability
        foreach (GameObject cardObj in currentCardObjects)
        {
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI == null || cardUI.GetCardData() == null) continue;
            
            bool isItem = cardUI.GetCardData() is ItemSO;
            bool canPlay = isItem || canPlayMoreCards; // Items can always be played
            
            // Use the button component to control interactability
            var button = cardObj.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.interactable = canPlay && !cardUI.IsBlocked();
            }
        }
    }

    private bool IsCardBlockedByAnyPathogen(CardSO card, List<Pathogen> pathogens)
    {
        System.Type cardType = card.GetType();
        
        foreach (var pathogen in pathogens)
        {
            if (pathogen.IsCardBlocked(cardType))
            {
                return true;
            }
        }
        
        return false;
    }
    
    #endregion
    
    #region Card Selection
    
    private void OnCardClicked(CardUI clickedCard)
    {
        // Don't allow playing blocked cards
        if (clickedCard.IsBlocked())
        {
            Debug.Log($"Cannot play {clickedCard.GetCardData().cardName} - it's blocked by a pathogen!");
            return;
        }
        
        if (actionPanel != null)
        {
            actionPanel.ShowForCard(clickedCard.GetCardData(), this);
        }
        else
        {
            PlayCard(clickedCard.GetCardData());
        }
    }
    
    #endregion
    
    #region ICardActionHandler Implementation
    
    public void PlayCard(CardSO card)
    {
        // Delegate to TurnManager which handles all turn logic, card limits, and validation
        if (turnManager != null)
        {
            bool success = turnManager.PlayCard(card, pathogenManager?.GetCurrentPathogen());
            if (success)
            {
                // Update blocking after playing a card (in case it affects blocking)
                UpdateCardBlocking();
            }
        } 
        else
        {
            Debug.LogError("PlayerHandUI: Cannot find TurnManager to play card!");
        }
    }
    
    public void DiscardCard(CardSO card)
    {
        if (player == null) return;
        
        if (player.Hand.Contains(card))
        {
            player.Hand.Remove(card);
            Debug.Log($"Discarded card: {card.cardName}");
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Force refresh the hand UI immediately - useful for debugging
    /// </summary>
    [ContextMenu("Force Refresh Hand")]
    public void ForceRefreshHand()
    {
        Debug.Log($"PlayerHandUI: Manual force refresh requested. Hand size: {player?.PlayerCards?.Hand?.Count ?? 0}");
        RefreshHand();
        
        if (player?.PlayerCards?.Hand != null)
        {
            lastHandCount = player.PlayerCards.Hand.Count;
            Debug.Log($"PlayerHandUI: Updated lastHandCount to {lastHandCount}");
        }
    }

    [ContextMenu("Debug Inventory State")]
    public void DebugInventoryState()
    {
        if (player?.PlayerInventory != null)
        {
            Debug.Log($"PlayerHandUI: Inventory has {player.PlayerInventory.Items.Count} items:");
            foreach (var slot in player.PlayerInventory.Items)
            {
                Debug.Log($"  - {slot.item?.cardName ?? "null"} x{slot.quantity}");
            }
        }
        else
        {
            Debug.LogWarning("PlayerHandUI: Cannot debug inventory - player or inventory is null");
        }
    }
    
    public int GetHandCount()
    {
        return player?.Hand.Count ?? 0;
    }
    
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        RefreshHand();
    }
    
    public void SetCardsInteractable(bool interactable)
    {
        foreach (var cardObject in currentCardObjects)
        {
            var cardUI = cardObject.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetInteractable(interactable);
            }
        }
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnCardAddedToHand(CardSO card)
    {
        Debug.Log($"PlayerHandUI: Card {card.cardName} added to hand - refreshing UI immediately");
        Debug.Log($"PlayerHandUI: Hand size before refresh: {player?.PlayerCards?.Hand?.Count ?? 0}");
        
        // Force immediate UI refresh with a slight delay to ensure data is updated
        StartCoroutine(ForceRefreshHandCoroutine());
    }
    
    private void OnCardPlayed(CardSO card)
    {
        Debug.Log($"PlayerHandUI: Card {card.cardName} played - refreshing UI immediately");
        Debug.Log($"PlayerHandUI: Hand size before refresh: {player?.PlayerCards?.Hand?.Count ?? 0}");
        
        // Force immediate UI refresh with a slight delay to ensure data is updated
        StartCoroutine(ForceRefreshHandCoroutine());
    }
    
    private System.Collections.IEnumerator ForceRefreshHandCoroutine()
    {
        // Wait one frame to ensure all events have processed
        yield return null;
        
        Debug.Log($"PlayerHandUI: Force refreshing hand. Current hand size: {player?.PlayerCards?.Hand?.Count ?? 0}");
        RefreshHand();
        
        // Update the last hand count to prevent polling conflicts
        if (player?.PlayerCards?.Hand != null)
        {
            lastHandCount = player.PlayerCards.Hand.Count;
        }
    }
    
    private void OnPathogenChanged(Pathogen pathogen)
    {
        UpdateCardBlocking();
    }
    
    private void OnCardLimitChanged(int cardsPlayed, int cardLimit)
    {
        // Update card interactability based on card limit
        UpdateCardInteractability();
        Debug.Log($"PlayerHandUI: Card limit status - {cardsPlayed}/{cardLimit}");
    }
    
    // Placeholder inventory event handlers (to be implemented when inventory events are available)
    private void OnItemAddedToInventory(ItemSO item, int quantity) { RefreshHand(); }
    private void OnItemRemovedFromInventory(ItemSO item, int quantity) { RefreshHand(); }
    private void OnItemUsedFromInventory(ItemSO item) { RefreshHand(); }
    
    #endregion
}