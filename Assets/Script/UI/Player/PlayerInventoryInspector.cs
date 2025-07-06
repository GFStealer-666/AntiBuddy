using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inspector component to view player inventory in the Unity Inspector for debugging.
/// Attach this to any GameObject to see the current player's inventory state.
/// </summary>
public class PlayerInventoryInspector : MonoBehaviour
{
    [Header("Player Inventory Inspector")]
    [SerializeField] private bool autoRefresh = true;
    [SerializeField] private float refreshRate = 1f;
    
    [Header("Current Player Info")]
    [SerializeField] private int currentHP;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentTokens;
    [SerializeField] private int currentDefense;
    
    [Header("Inventory Contents")]
    [SerializeField] private List<InventoryDisplayItem> inventoryItems = new List<InventoryDisplayItem>();
    
    [Header("Hand Contents")]
    [SerializeField] private List<string> handCards = new List<string>();
    
    private Player player;
    private float lastRefreshTime;
    
    void Start()
    {
        RefreshPlayerReference();
    }
    
    void Update()
    {
        if (autoRefresh && Time.time - lastRefreshTime > refreshRate)
        {
            RefreshInventoryDisplay();
            lastRefreshTime = Time.time;
        }
    }
    
    [ContextMenu("Refresh Player Reference")]
    public void RefreshPlayerReference()
    {
        // Try to find player from PlayerManager first
        var playerManager = FindFirstObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            player = playerManager.GetPlayer();
            Debug.Log("PlayerInventoryInspector: Found player via PlayerManager");
        }
        else
        {
            // Fallback to GameManager
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                player = gameManager.GetPlayer();
                Debug.Log("PlayerInventoryInspector: Found player via GameManager");
            }
        }
        
        if (player == null)
        {
            Debug.LogWarning("PlayerInventoryInspector: Could not find player reference");
        }
        else
        {
            RefreshInventoryDisplay();
        }
    }
    
    [ContextMenu("Refresh Inventory Display")]
    public void RefreshInventoryDisplay()
    {
        if (player == null)
        {
            RefreshPlayerReference();
            if (player == null) return;
        }
        
        // Update player stats
        currentHP = player.HP;
        maxHP = player.MaxHP;
        currentTokens = player.Tokens;
        currentDefense = player.Defense;
        
        // Update inventory display
        inventoryItems.Clear();
        foreach (var slot in player.PlayerInventory.Items)
        {
            inventoryItems.Add(new InventoryDisplayItem
            {
                itemName = slot.item.cardName,
                quantity = slot.quantity,
                tokenCost = slot.item.tokenCost,
                hpCost = slot.item.hpCost,
                isConsumable = slot.item.isConsumable,
                maxStack = slot.item.maxStack
            });
        }
        
        // Update hand display
        handCards.Clear();
        foreach (var card in player.Hand)
        {
            // handCards.Add($"{card.cardName} (Cost: {card.}t/{card.hpCost}hp)");
        }
        
        Debug.Log($"PlayerInventoryInspector: Refreshed - HP: {currentHP}/{maxHP}, Tokens: {currentTokens}, Inventory: {inventoryItems.Count} items, Hand: {handCards.Count} cards");
    }
    
    public Player GetPlayer()
    {
        return player;
    }
    
    public int GetInventoryItemCount()
    {
        return inventoryItems.Count;
    }
    
    public int GetHandCardCount()
    {
        return handCards.Count;
    }
    
    [ContextMenu("Add Test Item (Health Potion)")]
    public void AddTestHealthPotion()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerInventoryInspector: No player reference to add test item");
            return;
        }
        
        // Try to find a health potion item in the project
        var healthPotions = Resources.LoadAll<ItemSO>("Items");
        ItemSO testItem = null;
        
        foreach (var item in healthPotions)
        {
            if (item.itemType == ItemType.Healing || item.cardName.ToLower().Contains("health") || item.cardName.ToLower().Contains("potion"))
            {
                testItem = item;
                break;
            }
        }
        
        if (testItem != null)
        {
            player.PlayerInventory.AddItem(testItem);
            Debug.Log($"PlayerInventoryInspector: Added test item {testItem.cardName} to inventory");
            RefreshInventoryDisplay();
        }
        else
        {
            Debug.LogWarning("PlayerInventoryInspector: No suitable test item found in Resources/Items");
        }
    }
    
    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerInventoryInspector: No player reference to clear inventory");
            return;
        }
        
        var itemsToRemove = new List<ItemSO>();
        foreach (var slot in player.PlayerInventory.Items)
        {
            itemsToRemove.Add(slot.item);
        }
        
        foreach (var item in itemsToRemove)
        {
            player.PlayerInventory.RemoveItem(item, player.PlayerInventory.GetItemCount(item));
        }
        
        Debug.Log("PlayerInventoryInspector: Cleared all items from inventory");
        RefreshInventoryDisplay();
    }
}

/// <summary>
/// Serializable class to display inventory items in the inspector
/// </summary>
[System.Serializable]
public class InventoryDisplayItem
{
    public string itemName;
    public int quantity;
    public int tokenCost;
    public int hpCost;
    public bool isConsumable;
    public int maxStack;
}
