using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int quantity;

    public InventorySlot(ItemSO item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class PlayerInventory
{
    public List<InventorySlot> Items { get; private set; }
    public int MaxSlots { get; private set; }

    public event Action<ItemSO, int> OnItemAdded;
    public event Action<ItemSO, int> OnItemRemoved;
    public event Action<ItemSO> OnItemUsed;

    public PlayerInventory(int maxSlots = 20)
    {
        Items = new List<InventorySlot>();
        MaxSlots = maxSlots;
    }

    public bool PurchaseItem(ItemSO item, PlayerTokens playerTokens)
    {
        // This method is deprecated - use ShopManager.PurchaseItem instead
        // Keeping for compatibility but shop should handle purchasing now
        Debug.LogWarning("PlayerInventory.PurchaseItem is deprecated - use ShopManager instead");
        
        if (!CanAddItem(item))
        {
            Debug.LogWarning($"Cannot add {item.cardName} to inventory. Inventory full or item limit reached.");
            return false;
        }

        // Simple token cost for compatibility
        int cost = item.tokenCost > 0 ? item.tokenCost : 5; // Default cost
        
        if (playerTokens.Tokens < cost)
        {
            Debug.LogWarning($"Cannot afford {item.cardName}. Cost: {cost}, Available: {playerTokens.Tokens}");
            return false;
        }

        if (playerTokens.SpendTokens(cost))
        {
            AddItem(item);
            Debug.Log($"Purchased {item.cardName} for {cost} tokens");
            return true;
        }

        return false;
    }

    public bool AddItem(ItemSO item, int quantity = 1)
    {
        if (!CanAddItem(item, quantity))
            return false;

        var existingSlot = Items.FirstOrDefault(slot => slot.item == item);
        if (existingSlot != null && item.maxStack > 1)
        {
            int canAdd = Mathf.Min(quantity, item.maxStack - existingSlot.quantity);
            existingSlot.quantity += canAdd;
            OnItemAdded?.Invoke(item, canAdd);
            return canAdd == quantity;
        }
        else
        {
            Items.Add(new InventorySlot(item, quantity));
            OnItemAdded?.Invoke(item, quantity);
            return true;
        }
    }

    public bool UseItem(ItemSO item, Player player)
    {
        var slot = Items.FirstOrDefault(s => s.item == item);
        if (slot == null)
        {
            Debug.LogWarning($"Item {item.cardName} not found in inventory");
            return false;
        }

        // Items are now played through the card system (since ItemSO inherits from CardSO)
        bool success = player.PlayCard(item);
        
        if (success)
        {
            OnItemUsed?.Invoke(item);
            
            if (item.isConsumable)
            {
                RemoveItem(item, 1);
                Debug.Log($"Used and consumed {item.cardName}");
            }
            else
            {
                Debug.Log($"Used {item.cardName} (not consumed)");
            }
        }

        return success;
    }

    public bool RemoveItem(ItemSO item, int quantity = 1)
    {
        var slot = Items.FirstOrDefault(s => s.item == item);
        if (slot == null || slot.quantity < quantity)
            return false;

        slot.quantity -= quantity;
        OnItemRemoved?.Invoke(item, quantity);

        if (slot.quantity <= 0)
        {
            Items.Remove(slot);
        }

        return true;
    }

    public bool CanAddItem(ItemSO item, int quantity = 1)
    {
        var existingSlot = Items.FirstOrDefault(slot => slot.item == item);
        if (existingSlot != null)
        {
            return existingSlot.quantity + quantity <= item.maxStack;
        }
        return Items.Count < MaxSlots;
    }

    public int GetItemCount(ItemSO item)
    {
        var slot = Items.FirstOrDefault(s => s.item == item);
        return slot?.quantity ?? 0;
    }

    public bool HasItem(ItemSO item)
    {
        return GetItemCount(item) > 0;
    }
}
