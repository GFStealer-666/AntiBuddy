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
    
    [Header("Settings")]
    [SerializeField] private float refreshRate = 0.1f;
    
    private List<GameObject> currentCardObjects = new List<GameObject>();
    private int lastHandCount = -1;
    private float lastRefreshTime = 0f;
    
    #region Unity Lifecycle
    
    void Start()
    {
        InitializeComponents();
        RefreshHand();
    }
    
    void Update()
    {
        if (Time.time - lastRefreshTime > refreshRate)
        {
            CheckForHandChanges();
            lastRefreshTime = Time.time;
        }
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeComponents()
    {
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
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                player = gameManager.GetPlayer();
            }
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (pathogenManager != null)
        {
            pathogenManager.OnPathogenSpawned -= OnPathogenChanged;
            pathogenManager.OnPathogenDefeated -= OnPathogenChanged;
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
        
        foreach (CardSO card in player.Hand)
        {
            CreateCardUI(card);
        }
        
        // Update blocking status after creating all cards
        UpdateCardBlocking();
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
    
    private void OnPathogenChanged(Pathogen pathogen)
    {
        // Update card blocking when pathogens change
        UpdateCardBlocking();
    }
    
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
        if (player == null) return;
        
        // Double-check that card isn't blocked
        if (pathogenManager != null)
        {
            var activePathogens = pathogenManager.GetActivePathogens();
            if (IsCardBlockedByAnyPathogen(card, activePathogens))
            {
                Debug.Log($"Cannot play {card.cardName} - it's blocked!");
                return;
            }
        }
        
        bool success = player.PlayCard(card);
        if (success)
        {
            Debug.Log($"Played card: {card.cardName}");

            playerManager.ApplyCardEffect(card , pathogenManager.GetCurrentPathogen());
            // Update blocking after playing a card (in case it affects blocking)
            UpdateCardBlocking();
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
    
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        RefreshHand();
    }
    
    #endregion
}