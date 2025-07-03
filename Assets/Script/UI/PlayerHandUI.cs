using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple UI for displaying player's hand as vertical list of cards
/// Follows SRP - only handles displaying cards from Player.Hand
/// </summary>
public class PlayerHandUI : MonoBehaviour, ICardActionHandler
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform cardContainer; // Vertical Layout Group container
    [SerializeField] private GameObject cardPrefab; // Should have CardUI component
    [SerializeField] private CardActionPanel actionPanel; // Panel that appears when card is clicked
    
    [Header("Settings")]
    [SerializeField] private float refreshRate = 0.1f; // How often to check for hand changes
    
    private List<GameObject> currentCardObjects = new List<GameObject>();
    private int lastHandCount = -1;
    private float lastRefreshTime = 0f;
    
    #region Unity Lifecycle
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            // Since Player is not a MonoBehaviour, we need to get it from GameManager
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                player = gameManager.GetPlayer();
            }
        }
            
        RefreshHand();
    }
    
    void Update()
    {
        // Periodically check if hand has changed
        if (Time.time - lastRefreshTime > refreshRate)
        {
            CheckForHandChanges();
            lastRefreshTime = Time.time;
        }
    }
    
    #endregion
    
    #region Hand Management
    
    private void CheckForHandChanges()
    {
        if (player == null) return;
        
        // Simple check - if hand count changed, refresh everything
        if (player.Hand.Count != lastHandCount)
        {
            RefreshHand();
            lastHandCount = player.Hand.Count;
        }
    }
    
    public void RefreshHand()
    {
        if (player == null || cardContainer == null) return;
        
        // Clear existing UI cards
        ClearCardUIs();
        
        // Create UI for each card in player's hand
        foreach (CardSO card in player.Hand)
        {
            CreateCardUI(card);
        }
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
    
    #region Card Selection
    
    private void OnCardClicked(CardUI clickedCard)
    {
        // Show action panel for the selected card
        if (actionPanel != null)
        {
            actionPanel.ShowForCard(clickedCard.GetCardData(), this);
        }
        else
        {
            // Fallback: directly play the card if no action panel
            PlayCard(clickedCard.GetCardData());
        }
    }
    
    #endregion
    
    #region ICardActionHandler Implementation
    
    public void PlayCard(CardSO card)
    {
        if (player == null) return;
        
        bool success = player.PlayCard(card);
        if (success)
        {
            Debug.Log($"Played card: {card.cardName}");
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
    
    public void ForceRefresh()
    {
        RefreshHand();
    }
    
    public int GetHandCount()
    {
        return player?.Hand.Count ?? 0;
    }
    
    /// <summary>
    /// Set the player reference (called by GameManager)
    /// </summary>
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        RefreshHand(); // Refresh display when player is set
    }
    
    #endregion
}