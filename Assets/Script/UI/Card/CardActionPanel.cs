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
    [SerializeField] private Image frontCard;
    [SerializeField] private Image backCard;
    [SerializeField] private Button useButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Settings")]
    [SerializeField] private bool hideOnUse = true;
    [SerializeField] private bool hideOnDiscard = true;
    
    [SerializeField] private CardSO currentCard;
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

        if (frontCard != null && currentCard.frontCardImage != null)
            frontCard.sprite = currentCard.frontCardImage;

        if (backCard != null && currentCard.backCardImage != null)
            backCard.sprite = currentCard.backCardImage;
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
