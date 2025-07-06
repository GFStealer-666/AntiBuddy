using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// UI component for displaying the shop with items for purchase
/// Uses CardUI to display items and ShopItemActionPanel for purchase actions
/// </summary>
public class ShopDisplayUI : MonoBehaviour, IShopItemActionHandler
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject cardUIPrefab; // Prefab with CardUI component
    [SerializeField] private ShopItemActionPanel itemActionPanel;
    [Header("Player Reference")]
    [SerializeField] private Player player; // Assign in inspector or find dynamically
    
    private List<CardUI> currentItemUIs = new List<CardUI>();
    private List<ItemSO> currentShopItems = new List<ItemSO>();
    
    #region Unity Lifecycle
    
    void Awake()
    {

        // Subscribe to shop events
        ShopManager.OnShopOpened += OnShopOpened;
        ShopManager.OnShopClosed += OnShopClosed;
        
    }
    
    void OnDestroy()
    {
        // Unsubscribe from shop events
        ShopManager.OnShopOpened -= OnShopOpened;
        ShopManager.OnShopClosed -= OnShopClosed;
    }
    
    #endregion
    
    #region Shop Events
    
    private void OnShopOpened(List<ItemSO> items)
    {
        DisplayShopItems(items);
        ShowShop();
    }
    
    private void OnShopClosed()
    {
        CloseShop();
    }
    
    #endregion
    
    #region Shop Display
    
    private void ShowShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

    }
    
    private void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
            
        // Hide action panel if showing
        if (itemActionPanel != null)
            itemActionPanel.Hide();
            
        // Clear current items
        ClearItemDisplay();
    }
    
    private void DisplayShopItems(List<ItemSO> items)
    {
        // Clear existing items
        ClearItemDisplay();
        
        currentShopItems = new List<ItemSO>(items);
        
        // Create UI for each item
        foreach (ItemSO item in items)
        {
            CreateItemUI(item);
        }
    }
    
    private void CreateItemUI(ItemSO item)
    {
        if (cardUIPrefab == null || itemsContainer == null) return;
        
        // Instantiate card UI prefab
        GameObject cardUIObj = Instantiate(cardUIPrefab, itemsContainer);
        CardUI cardUI = cardUIObj.GetComponent<CardUI>();
        
        if (cardUI != null)
        {
            // Initialize with item data (works because ItemSO inherits from CardSO)
            cardUI.Initialize(item);
            
            // Set up click handler to show purchase options
            cardUI.OnCardClicked += OnItemClicked;
            
            currentItemUIs.Add(cardUI);
        }
    }
    
    private void ClearItemDisplay()
    {
        // Clean up existing item UIs
        foreach (CardUI cardUI in currentItemUIs)
        {
            if (cardUI != null)
            {
                cardUI.OnCardClicked -= OnItemClicked;
                Destroy(cardUI.gameObject);
            }
        }
        
        currentItemUIs.Clear();
        currentShopItems.Clear();
    }
    
    #endregion
    
    #region Item Interaction
    
    private void OnItemClicked(CardUI clickedCardUI)
    {
        // Get the item data
        CardSO cardData = clickedCardUI.GetCardData();
        
        // Cast to ItemSO (safe because only items are in shop)
        ItemSO item = cardData as ItemSO;
        
        if (item != null && itemActionPanel != null)
        {
            // Show purchase options panel
            itemActionPanel.ShowForItem(item, this);
        }
    }
    
    #endregion
    
    #region IShopItemActionHandler Implementation
    
    public void PurchaseItem(ItemSO item, bool useHealth = false)
    {
        if (player == null)
        {
            Debug.LogError("No player assigned to ShopDisplayUI");
            return;
        }
        
        // Try to purchase through ShopManager
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            bool success = shopManager.PurchaseItem(item, player, useHealth);
            
            if (success)
            {
                Debug.Log($"Successfully purchased {item.cardName}");
                
                // Remove from shop display
                RemoveItemFromShop(item);
            }
            else
            {
                Debug.LogWarning($"Failed to purchase {item.cardName}");
            }
        }
    }
    
    public Player GetPlayer()
    {
        return player;
    }
    
    private void RemoveItemFromShop(ItemSO purchasedItem)
    {
        // Find and remove the UI for this item
        for (int i = currentItemUIs.Count - 1; i >= 0; i--)
        {
            CardUI cardUI = currentItemUIs[i];
            if (cardUI != null && cardUI.GetCardData() == purchasedItem)
            {
                cardUI.OnCardClicked -= OnItemClicked;
                Destroy(cardUI.gameObject);
                currentItemUIs.RemoveAt(i);
                break;
            }
        }
        
        // Remove from current shop items list
        currentShopItems.Remove(purchasedItem);
    }
    
    #endregion
    
    #region Public Properties
    
    public bool IsShopOpen => shopPanel != null && shopPanel.activeSelf;
    
    #endregion
}
