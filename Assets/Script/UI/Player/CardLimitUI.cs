using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardLimitUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardLimitText;
    [SerializeField] private Slider cardLimitSlider;
    [SerializeField] private Image cardLimitFill;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color limitReachedColor = Color.red;
    
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
        gameObject.SetActive(phase == TurnPhase.PlayerTurn);
    }
    
    private void UpdateCardLimitDisplay(int? cardsPlayed = null, int? cardLimit = null)
    {
        if (turnManager == null) return;
        
        int played = cardsPlayed ?? turnManager.GetCardsPlayedThisTurn();
        int limit = cardLimit ?? turnManager.GetCardLimit();
        int remaining = limit - played;
        
        // Update text
        if (cardLimitText != null)
        {
            cardLimitText.text = $"Cards: {played}/{limit}";
            
            if (remaining > 0)
            {
                cardLimitText.text += $" ({remaining} left)";
            }
            else
            {
                cardLimitText.text += " (Max reached)";
            }
        }
        
        // Update slider
        if (cardLimitSlider != null)
        {
            cardLimitSlider.maxValue = limit;
            cardLimitSlider.value = played;
        }
        
        // Update color based on remaining cards
        Color targetColor = normalColor;
        if (remaining == 0)
        {
            targetColor = limitReachedColor;
        }
        else if (remaining == 1)
        {
            targetColor = warningColor;
        }
        
        // Apply color to fill image
        if (cardLimitFill != null)
        {
            cardLimitFill.color = targetColor;
        }
        
        // Apply color to text
        if (cardLimitText != null)
        {
            cardLimitText.color = targetColor;
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
