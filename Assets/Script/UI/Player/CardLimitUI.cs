using UnityEngine;
using TMPro;

public class CardLimitUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardLimitText;

    private TurnManager turnManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();

        // Subscribe to card limit changes
        TurnManager.OnCardLimitChanged += OnCardLimitChanged;
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;

        // Initialize display
        UpdateCardLimitDisplay();
    }

    void OnDestroy()
    {
        TurnManager.OnCardLimitChanged -= OnCardLimitChanged;
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
    }

    private void OnCardLimitChanged(int cardsPlayed, int cardLimit)
    {
        UpdateCardLimitDisplay(cardsPlayed, cardLimit);
    }

    private void OnTurnPhaseChanged(TurnPhase phase)
    {
        // Update display when turn phase changes
        UpdateCardLimitDisplay();

        // Hide during pathogen turn, show during player turn
    }

    private void UpdateCardLimitDisplay(int? cardsPlayed = null, int? cardLimit = null)
    {
        if (turnManager == null) return;

        int played = cardsPlayed ?? turnManager.GetCardsPlayedThisTurn();
        int limit = cardLimit ?? turnManager.GetCardLimit();

        // Update text
        if (cardLimitText != null)
        {
            cardLimitText.text = $"{played}/{limit}";
        }
    }

    /// <summary>
    /// Called from UI button to end turn manually
    /// </summary>
    public void EndTurnButton()
    {
        if (turnManager != null && turnManager.IsPlayerTurn())
        {
            turnManager.EndPlayerTurn("player ended turn manually");
        }
    }
}
