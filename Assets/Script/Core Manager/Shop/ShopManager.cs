using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    public List<ItemSO> availableItems = new List<ItemSO>(); // Set in inspector with all possible items
    public int itemsPerShop = 1; // How many items to show each round
    
    [Header("Shop Timing")]
    public bool showShopAtTurnStart = true; // Show at start vs end of turn
    
    private List<ItemSO> currentShopItems = new List<ItemSO>();
    private bool shopIsOpen = false;
    
    // Events
    public static System.Action<List<ItemSO>> OnShopOpened;
    public static System.Action OnShopClosed;
    
    void Start()
    {
        OpenShop();
        TurnManager.OnTurnPhaseChanged += OnTurnPhaseChanged;
    }
    
    void OnDestroy()
    {
        TurnManager.OnTurnPhaseChanged -= OnTurnPhaseChanged;
    }
    
    private void OnTurnPhaseChanged(TurnPhase phase)
    {
        if (showShopAtTurnStart && phase == TurnPhase.PlayerTurn)
        {
            // Refresh shop items at start of each player turn
            Debug.Log("ShopManager: Player turn started - refreshing shop items");
            RefreshShop();
        }
    }
    
    public void OpenShop()
    {
        if (shopIsOpen) return;
        
        GenerateRandomItems();
        shopIsOpen = true;
        OnShopOpened?.Invoke(currentShopItems);
    }

    public void RefreshShop()
    {
        Debug.Log("ShopManager: Refreshing shop with new items");
        GenerateRandomItems();
        
        if (!shopIsOpen)
        {
            shopIsOpen = true;
        }
        
        OnShopOpened?.Invoke(currentShopItems);
        Debug.Log($"ShopManager: Shop refreshed with {currentShopItems.Count} new items");
    }
    
    private void GenerateRandomItems()
    {
        Debug.Log("ShopManager: Generating new random items for shop");
        currentShopItems.Clear();
        
        if (availableItems.Count == 0)
        {
            Debug.LogWarning("No items available in shop! Add items to availableItems list.");
            return;
        }
        
        // Create a copy of available items to avoid duplicates
        List<ItemSO> itemPool = new List<ItemSO>(availableItems);
        
        // Pick random items
        for (int i = 0; i < itemsPerShop && itemPool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, itemPool.Count);
            ItemSO selectedItem = itemPool[randomIndex];
            
            // Randomize costs for items that have useRandomCost enabled
            selectedItem.RandomizeCosts();
            
            currentShopItems.Add(selectedItem);
            itemPool.RemoveAt(randomIndex); // Remove to avoid duplicates
            
            Debug.Log($"ShopManager: Added {selectedItem.cardName} to shop");
        }
        
        Debug.Log($"ShopManager: Generated {currentShopItems.Count} items for shop");
    }
    
    public bool PurchaseItem(ItemSO item, Player player, bool useHealth = false)
    {
        if (!shopIsOpen || !currentShopItems.Contains(item))
        {
            Debug.LogWarning("Cannot purchase item - shop is closed or item not available");
            return false;
        }
        
        // Check if player can afford the item
        bool canAfford = false;
        string costDescription = "";
        
        switch (item.costType)
        {
            case ItemCostType.Tokens:
                canAfford = player.Tokens >= item.tokenCost;
                costDescription = $"{item.tokenCost} tokens";
                break;
                
            case ItemCostType.Health:
                canAfford = player.HP > item.hpCost; // Must survive the cost
                costDescription = $"{item.hpCost} HP";
                break;
                
            case ItemCostType.Either:
                if (useHealth)
                {
                    canAfford = player.HP > item.hpCost;
                    costDescription = $"{item.hpCost} HP";
                }
                else
                {
                    canAfford = player.Tokens >= item.tokenCost;
                    costDescription = $"{item.tokenCost} tokens";
                }
                break;
        }
        
        if (!canAfford)
        {
            Debug.LogWarning($"Cannot afford {item.cardName}. Cost: {costDescription}");
            return false;
        }
        
        // Check if player can add item to inventory
        if (!player.PlayerInventory.CanAddItem(item))
        {
            Debug.LogWarning($"Cannot add {item.cardName} to inventory - full or item limit reached");
            return false;
        }
        
        // Pay the cost
        switch (item.costType)
        {
            case ItemCostType.Tokens:
                player.PlayerTokens.SpendTokens(item.tokenCost);
                break;
                
            case ItemCostType.Health:
                player.PlayerHealth.TakeDamage(item.hpCost);
                break;
                
            case ItemCostType.Either:
                if (useHealth)
                    player.PlayerHealth.TakeDamage(item.hpCost);
                else
                    player.PlayerTokens.SpendTokens(item.tokenCost);
                break;
        }
        
        // Add item to inventory
        player.PlayerInventory.AddItem(item);
        
        // Force immediate PlayerHandUI refresh to ensure purchased items appear immediately
        var playerHandUI = FindFirstObjectByType<PlayerHandUI>();
        if (playerHandUI != null)
        {
            playerHandUI.RefreshHand();
            Debug.Log("ShopManager: Forced PlayerHandUI refresh after item purchase");
        }
        
        // Remove from shop (optional - items could be unlimited)
        currentShopItems.Remove(item);
        
        Debug.Log($"Purchased {item.cardName} for {costDescription}!");
        
        // Log the purchase via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogItemPurchase($"{item.cardName} for {costDescription}");
        }
        
        return true;
    }
    
    
    public List<ItemSO> GetCurrentShopItems()
    {
        return new List<ItemSO>(currentShopItems);
    }
    
    public bool IsShopOpen()
    {
        return shopIsOpen;
    }
    
    #region Debug Methods
    
    [ContextMenu("Force Open Shop")]
    public void DebugOpenShop()
    {
        OpenShop();
    }

    
    [ContextMenu("Debug Shop Status")]
    public void DebugShopStatus()
    {
        Debug.Log($"=== Shop Status ===");
        Debug.Log($"Shop Open: {shopIsOpen}");
        Debug.Log($"Available Items: {availableItems.Count}");
        Debug.Log($"Current Shop Items: {currentShopItems.Count}");
        Debug.Log($"Items per shop: {itemsPerShop}");
    }

    [ContextMenu("Force Refresh Shop")]
    public void ForceRefreshShop()
    {
        Debug.Log("ShopManager: Manual shop refresh requested");
        RefreshShop();
    }
    
    #endregion
}
