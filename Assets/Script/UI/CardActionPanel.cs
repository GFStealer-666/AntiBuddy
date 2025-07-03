using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interface for objects that can handle card actions
/// </summary>
public interface ICardActionHandler
{
    void PlayCard(CardSO card);
    void DiscardCard(CardSO card);
}

/// <summary>
/// Panel that appears when a card is clicked, showing Use/Discard options
/// Simple popup panel for card actions
/// </summary>
public class CardActionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelObject;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private Image cardIcon;
    [SerializeField] private Button useButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Settings")]
    [SerializeField] private bool hideOnUse = true;
    [SerializeField] private bool hideOnDiscard = true;
    
    private CardSO currentCard;
    private ICardActionHandler handUI;
    
    #region Unity Lifecycle
    
    void Awake()
    {
        // Setup button listeners
        if (useButton != null)
            useButton.onClick.AddListener(OnUseClicked);
            
        if (discardButton != null)
            discardButton.onClick.AddListener(OnDiscardClicked);
            
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);
        
        // Start hidden
        if (panelObject != null)
            panelObject.SetActive(false);
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (useButton != null)
            useButton.onClick.RemoveListener(OnUseClicked);
            
        if (discardButton != null)
            discardButton.onClick.RemoveListener(OnDiscardClicked);
            
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(OnCancelClicked);
    }
    
    #endregion
    
    #region Panel Management
    
    public void ShowForCard(CardSO card, ICardActionHandler actionHandler)
    {
        currentCard = card;
        handUI = actionHandler;
        
        UpdateDisplay();
        
        if (panelObject != null)
            panelObject.SetActive(true);
    }
    
    public void Hide()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
            
        currentCard = null;
        handUI = null;
    }
    
    private void UpdateDisplay()
    {
        if (currentCard == null) return;
        
        if (cardNameText != null)
            cardNameText.text = currentCard.cardName;
            
        if (cardDescriptionText != null)
            cardDescriptionText.text = currentCard.description;
            
        if (cardIcon != null && currentCard.cardIcon != null)
            cardIcon.sprite = currentCard.cardIcon;
    }
    
    #endregion
    
    #region Button Handlers
    
    private void OnUseClicked()
    {
        if (currentCard != null && handUI != null)
        {
            handUI.PlayCard(currentCard);
            
            if (hideOnUse)
                Hide();
        }
    }
    
    private void OnDiscardClicked()
    {
        if (currentCard != null && handUI != null)
        {
            handUI.DiscardCard(currentCard);
            
            if (hideOnDiscard)
                Hide();
        }
    }
    
    private void OnCancelClicked()
    {
        Hide();
    }
    
    #endregion
    
    #region Public Properties
    
    public bool IsShowing => panelObject != null && panelObject.activeSelf;
    public CardSO GetCurrentCard() => currentCard;
    
    #endregion
}
