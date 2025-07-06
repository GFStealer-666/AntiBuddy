using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interface for objects that can handle shop item actions
/// </summary>
public interface IShopItemActionHandler
{
    void PurchaseItem(ItemSO item, bool useHealth = false);
    Player GetPlayer(); // Add method to get player for affordability checks
}

/// <summary>
/// Panel that appears when a shop item is clicked, showing purchase options
/// Different from CardActionPanel since shop items have different actions
/// </summary>
public class ShopItemActionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelObject;
    [SerializeField] private Image itemIcon;

    [Header("Purchase Buttons")]
    [SerializeField] private Button buyWithTokensButton;
    [SerializeField] private Button buyWithHealthButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Cost Display")]
    [SerializeField] private TextMeshProUGUI tokenCostText;
    [SerializeField] private TextMeshProUGUI healthCostText;
    
    private ItemSO currentItem;
    private IShopItemActionHandler shopHandler;
    
    #region Unity Lifecycle
    
    void Awake()
    {
        // Setup button listeners
        if (buyWithTokensButton != null)
            buyWithTokensButton.onClick.AddListener(() => OnPurchaseClicked(false));
            
        if (buyWithHealthButton != null)
            buyWithHealthButton.onClick.AddListener(() => OnPurchaseClicked(true));
            
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);
        
        // Start hidden
        if (panelObject != null)
            panelObject.SetActive(false);
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (buyWithTokensButton != null)
            buyWithTokensButton.onClick.RemoveAllListeners();
            
        if (buyWithHealthButton != null)
            buyWithHealthButton.onClick.RemoveAllListeners();
            
        if (cancelButton != null)
            cancelButton.onClick.RemoveAllListeners();
    }
    
    #endregion
    
    #region Panel Management
    
    public void ShowForItem(ItemSO item, IShopItemActionHandler actionHandler)
    {
        currentItem = item;
        shopHandler = actionHandler;
        
        UpdateDisplay();
        
        if (panelObject != null)
            panelObject.SetActive(true);
    }
    
    public void Hide()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
            
        currentItem = null;
        shopHandler = null;
    }

    private void UpdateDisplay()
    {
        if (currentItem == null) return;

        // Update item icon
        if (itemIcon != null && currentItem.frontCardImage != null)
            itemIcon.sprite = currentItem.frontCardImage;
        // Update button availability and cost display based on item cost type
        UpdatePurchaseOptions();
    }
    
    private void UpdatePurchaseOptions()
    {
        if (currentItem == null) return;
        
        // Get player for affordability checks
        Player player = shopHandler?.GetPlayer();
        bool hasPlayer = player != null;
        
        // Debug logging
        if (!hasPlayer)
        {
            Debug.LogWarning("ShopItemActionPanel: Player is null, cannot check affordability");
        }
        else
        {
            Debug.Log($"ShopItemActionPanel: Player found - HP: {player.HP}, Tokens: {player.Tokens}");
            Debug.Log($"ShopItemActionPanel: Item {currentItem.cardName} costs - HP: {currentItem.hpCost}, Tokens: {currentItem.tokenCost}, Type: {currentItem.costType}");
        }
        
        // Check what purchase options are available and affordable
        bool canBuyWithTokens = false;
        bool canAffordTokens = false;
        bool canBuyWithHealth = false;
        bool canAffordHealth = false;
        
        switch (currentItem.costType)
        {
            case ItemCostType.Tokens:
                canBuyWithTokens = true;
                canAffordTokens = hasPlayer && player.Tokens >= currentItem.tokenCost;
                break;
                
            case ItemCostType.Health:
                canBuyWithHealth = true;
                canAffordHealth = hasPlayer && player.HP > currentItem.hpCost; // Must survive the cost
                break;
                
            case ItemCostType.Either:
                canBuyWithTokens = true;
                canBuyWithHealth = true;
                canAffordTokens = hasPlayer && player.Tokens >= currentItem.tokenCost;
                canAffordHealth = hasPlayer && player.HP > currentItem.hpCost;
                break;
        }
        
        // Update buttons
        if (buyWithTokensButton != null)
        {
            buyWithTokensButton.gameObject.SetActive(canBuyWithTokens);
            if (canBuyWithTokens)
            {
                buyWithTokensButton.interactable = canAffordTokens;
                if (tokenCostText != null)
                {
                    tokenCostText.text = $"{currentItem.tokenCost} Tokens";
                }
            }
        }
        
        if (buyWithHealthButton != null)
        {
            buyWithHealthButton.gameObject.SetActive(canBuyWithHealth);
            if (canBuyWithHealth)
            {
                buyWithHealthButton.interactable = canAffordHealth;
                if (healthCostText != null)
                {
                    healthCostText.text = $"{currentItem.hpCost} HP";
                }
            }
        }
    }
    
    #endregion
    
    #region Button Handlers
    
    private void OnPurchaseClicked(bool useHealth)
    {
        if (currentItem != null && shopHandler != null)
        {
            shopHandler.PurchaseItem(currentItem, useHealth);
            Hide(); // Always hide after purchase attempt
        }
    }
    
    private void OnCancelClicked()
    {
        Hide();
    }
    
    #endregion
    
    #region Public Properties
    
    public bool IsShowing => panelObject != null && panelObject.activeSelf;
    public ItemSO GetCurrentItem() => currentItem;
    
    #endregion
}
