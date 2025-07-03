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
    public TurnManager turnManager;

    private List<Button> cardButtons = new List<Button>();

    void Start()
    {
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();

        // Subscribe to events
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
        TurnManager.OnTurnNumberChanged += OnTurnNumberChanged;
        TurnManager.OnPlayerStatsChanged += OnPlayerStatsChanged;

        // Setup button listeners
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(() => turnManager.EndPlayerTurn());

        UpdateUI();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
        TurnManager.OnTurnNumberChanged -= OnTurnNumberChanged;
        TurnManager.OnPlayerStatsChanged -= OnPlayerStatsChanged;
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
        
        var playerManager = turnManager.GetPlayerManager();
        if (playerManager != null)
        {
            OnPlayerStatsChanged(playerManager.GetPlayerStats());
        }
    }

    void UpdateCardDisplay()
    {
        if (turnManager == null || cardContainer == null || cardButtonPrefab == null) return;

        // Clear existing card buttons
        foreach (var button in cardButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        cardButtons.Clear();

        // Create buttons for cards in hand
        var playerHand = turnManager.GetPlayerHand();
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
            {
                buttonText.text = $"{card.cardName}\nPower: {card.power}";
            }
            
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
        if (turnManager == null) return;

        // For now, play without target (you can enhance this to include target selection)
        PathogenSO target = null;
        var pathogenManager = turnManager.GetPathogenManager();
        
        if (pathogenManager != null && pathogenManager.pathogens.Count > 0)
        {
            // Auto-target first living pathogen
            foreach (var pathogen in pathogenManager.pathogens)
            {
                if (pathogen.health > 0)
                {
                    target = pathogen;
                    break;
                }
            }
        }

        bool success = turnManager.PlayCard(card, target);
        if (success)
        {
            Debug.Log($"Successfully played {card.cardName}");
        }
        else
        {
            Debug.LogWarning($"Failed to play {card.cardName}");
        }
    }

    // Public method for external UI elements to end turn
    public void EndTurn()
    {
        if (turnManager != null)
            turnManager.EndPlayerTurn();
    }

}
