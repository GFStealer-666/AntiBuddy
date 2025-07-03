using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Simple card UI component for displaying card data and handling clicks
/// Follows SRP - only displays card info and notifies when clicked
/// </summary>
public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardIcon;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    
    private CardSO cardData;
    private bool isSelected = false;
    private bool isHovered = false;
    
    // Event for when this card is clicked
    public System.Action<CardUI> OnCardClicked;
    
    #region Initialization
    
    /// <summary>
    /// Initialize this card UI with card data
    /// </summary>
    /// <param name="card">The card data to display</param>
    public void Initialize(CardSO card)
    {
        cardData = card;
        UpdateDisplay();
    }
    
    /// <summary>
    /// Update the visual display based on current card data
    /// </summary>
    private void UpdateDisplay()
    {
        if (cardData == null) return;
        
        // Update icon
        if (cardIcon != null && cardData.cardIcon != null)
            cardIcon.sprite = cardData.cardIcon;
            
        // Set initial color
        UpdateVisualState();
    }
    
    #endregion
    
    #region Visual State
    
    /// <summary>
    /// Update visual appearance based on current state
    /// </summary>
    private void UpdateVisualState()
    {
        if (cardIcon == null) return;
        
        Color targetColor;
        if (isSelected)
            targetColor = selectedColor;
        else if (isHovered)
            targetColor = hoverColor;
        else
            targetColor = normalColor;
            
        cardIcon.color = targetColor;
    }
    
    /// <summary>
    /// Set whether this card appears selected
    /// </summary>
    /// <param name="selected">True if selected</param>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }
    
    #endregion
    
    #region Data Access
    
    /// <summary>
    /// Get the card data this UI represents
    /// </summary>
    /// <returns>The CardSO data</returns>
    public CardSO GetCardData()
    {
        return cardData;
    }
    
    #endregion
    
    #region Event Handling
    
    /// <summary>
    /// Handle pointer click events
    /// </summary>
    /// <param name="eventData">Click event data</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Fire the click event
        OnCardClicked?.Invoke(this);
    }
    
    /// <summary>
    /// Handle mouse enter events
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Handle mouse exit events
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    void OnDestroy()
    {
        // Clear event listeners when destroyed
        OnCardClicked = null;
    }
    
    #endregion
}
