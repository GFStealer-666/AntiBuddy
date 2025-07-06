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
    [SerializeField] private Button cardButton; // Add button component
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    
    private CardSO cardData;
    private bool isSelected = false;
    private bool isBlocked = false; // Add blocked state
    
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
        
        // Clear any existing button listeners first to prevent duplicates
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.interactable = true;
            cardButton.onClick.AddListener(() => OnCardClicked?.Invoke(this));
        }
        
        UpdateDisplay();
    }
    
    /// <summary>
    /// Update the visual display based on current card data
    /// </summary>
    private void UpdateDisplay()
    {
        if (cardData == null) return;
        
        // Update icon
        if (cardIcon != null && cardData.frontCardImage != null)
            cardIcon.sprite = cardData.frontCardImage;
            
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
        
        Color targetColor = normalColor;
        bool interactable = true;
        
        if (isBlocked)
        {
            interactable = false;
            targetColor = normalColor;
        }
        else
            targetColor = normalColor;
            
        cardIcon.color = targetColor;
        
        // Update button interactability
        if (cardButton != null)
            cardButton.interactable = interactable;
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
    
    /// <summary>
    /// Set whether this card is blocked from being played
    /// </summary>
    /// <param name="blocked">True if blocked</param>
    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
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
    
    /// <summary>
    /// Check if this card is currently blocked
    /// </summary>
    /// <returns>True if blocked</returns>
    public bool IsBlocked()
    {
        return isBlocked;
    }
    
    #endregion
    
    #region Event Handling
    
    /// <summary>
    /// Handle pointer click events
    /// </summary>
    /// <param name="eventData">Click event data</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Don't fire click if blocked
        if (isBlocked) return;
        
        // Fire the click event
        OnCardClicked?.Invoke(this);
    }
    
    /// <summary>
    /// Handle mouse enter events
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isBlocked) return; // No hover effect when blocked
        
        UpdateVisualState();
    }
    
    /// <summary>
    /// Handle mouse exit events
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateVisualState();
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    void OnDestroy()
    {
        // Clear button listeners when destroyed
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
        }
        
        // Clear event listeners when destroyed
        OnCardClicked = null;
    }
    
    #endregion
}