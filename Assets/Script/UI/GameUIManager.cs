using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI tokensText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI handSizeText;
    public Button endTurnButton;
    public Transform cardContainer;
    public GameObject cardButtonPrefab;

    [Header("Game References")]
    public GameManager gameManager;
    public TurnManager turnManager;
    public PlayerManager playerManager;
    public PathogenManager pathogenManager;

    private List<Button> cardButtons = new List<Button>();

    void Start()
    {
        // Find managers if not assigned
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();
        if (playerManager == null)
            playerManager = FindFirstObjectByType<PlayerManager>();
        if (pathogenManager == null)
            pathogenManager = FindFirstObjectByType<PathogenManager>();

        // Subscribe to events
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
        TurnManager.OnTurnNumberChanged += OnTurnNumberChanged;
        
        if (playerManager != null)
            playerManager.OnPlayerStatsChanged += OnPlayerStatsChanged;

        // Setup button listeners
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(() => gameManager?.EndTurnButtonPressed());

        UpdateUI();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
        TurnManager.OnTurnNumberChanged -= OnTurnNumberChanged;
        
        if (playerManager != null)
            playerManager.OnPlayerStatsChanged -= OnPlayerStatsChanged;
    }

    void OnTurnPhaseChanged(TurnPhase phase)
    {
        if (phaseText != null)
            phaseText.text = $"Phase: {phase}";

        // Enable/disable buttons based on phase
        bool isPlayerTurn = phase == TurnPhase.PlayerTurn;
        if (endTurnButton != null)
            endTurnButton.interactable = isPlayerTurn;
        UpdateCardButtons(isPlayerTurn);
    }

    void OnTurnNumberChanged(int turnNumber)
    {
        if (turnText != null)
            turnText.text = $"Turn: {turnNumber}";
    }

    void OnPlayerStatsChanged(PlayerStats stats)
    {
        if (healthText != null)
            healthText.text = $"HP: {stats.HP}/{stats.MaxHP}";
            
        if (defenseText != null)
            defenseText.text = $"Defense: {stats.Defense}";
            
        if (tokensText != null)
            tokensText.text = $"Tokens: {stats.Tokens}";
            
        if (handSizeText != null)
            handSizeText.text = $"Hand: {stats.HandSize}";

        UpdateCardDisplay();
    }

    void UpdateUI()
    {
        if (turnManager == null) return;

        OnTurnPhaseChanged(turnManager.GetCurrentPhase());
        OnTurnNumberChanged(turnManager.GetTurnNumber());
        
        if (playerManager != null)
        {
            OnPlayerStatsChanged(playerManager.GetPlayerStats());
        }
    }

    void UpdateCardDisplay()
    {
        if (gameManager == null || cardContainer == null || cardButtonPrefab == null) return;

        // Clear existing card buttons
        foreach (var button in cardButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        cardButtons.Clear();

        // Create buttons for cards in hand
        var playerHand = gameManager.GetPlayerHand();
        foreach (var card in playerHand)
        {
            CreateCardButton(card);
        }
    }

    void CreateCardButton(CardSO card)
    {
        GameObject buttonObj = Instantiate(cardButtonPrefab, cardContainer);
        Button button = buttonObj.GetComponent<Button>();
        
        if (button != null)
        {
            cardButtons.Add(button);
            
            // Set card text
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = card.cardName;
            
            // Set button action
            button.onClick.AddListener(() => PlayCard(card));
        }
    }

    void UpdateCardButtons(bool interactable)
    {
        foreach (var button in cardButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }

    void PlayCard(CardSO card)
    {
        if (gameManager == null) return;

        // Get current targeted pathogen
        Pathogen target = gameManager.GetCurrentTargetedPathogen();

        bool success = gameManager.PlayCard(card, target);
        if (success)
        {
            Debug.Log($"Successfully played {card.cardName}");
            UpdateCardDisplay(); // Refresh UI after playing card
        }
        else
        {
            Debug.LogWarning($"Failed to play {card.cardName}");
        }
    }

    // Public method for external UI elements to end turn
    public void EndTurn()
    {
        if (gameManager != null)
            gameManager.EndTurnButtonPressed();
    }
}
