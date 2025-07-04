using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemCard item;
    public int quantity;

    public InventorySlot(ItemCard item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class PlayerInventory
{
    public List<InventorySlot> Items { get; private set; }
    public int MaxSlots { get; private set; }

    public event Action<ItemCard, int> OnItemAdded;
    public event Action<ItemCard, int> OnItemRemoved;
    public event Action<ItemCard> OnItemUsed;

    public PlayerInventory(int maxSlots = 20)
    {
        Items = new List<InventorySlot>();
        MaxSlots = maxSlots;
    }

    public bool PurchaseItem(ItemCard item, PlayerTokens playerTokens)
    {
        if (!playerTokens.CanAfford(item.cost))
        {
            Console.WriteLine($"Cannot afford {item.itemName}. Cost: {item.cost}, Available: {playerTokens.Tokens}");
            return false;
        }

        if (!CanAddItem(item))
        {
            Console.WriteLine($"Cannot add {item.itemName} to inventory. Inventory full or item limit reached.");
            return false;
        }

        if (playerTokens.SpendTokens(item.cost))
        {
            AddItem(item);
            Console.WriteLine($"Purchased {item.itemName} for {item.cost} tokens");
            return true;
        }

        return false;
    }

    public bool AddItem(ItemCard item, int quantity = 1)
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

    public bool UseItem(ItemCard item, Player player)
    {
        var slot = Items.FirstOrDefault(s => s.item == item);
        if (slot == null)
        {
            Console.WriteLine($"Item {item.itemName} not found in inventory");
            return false;
        }

        if (!item.CanUse(player))
        {
            Console.WriteLine($"Cannot use {item.itemName} right now");
            return false;
        }

        item.UseItem(player);
        OnItemUsed?.Invoke(item);

        if (item.isConsumable)
        {
            RemoveItem(item, 1);
        }

        return true;
    }

    public bool RemoveItem(ItemCard item, int quantity = 1)
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

    public bool CanAddItem(ItemCard item, int quantity = 1)
    {
        var existingSlot = Items.FirstOrDefault(slot => slot.item == item);
        if (existingSlot != null)
        {
            return existingSlot.quantity + quantity <= item.maxStack;
        }
        return Items.Count < MaxSlots;
    }

    public int GetItemCount(ItemCard item)
    {
        var slot = Items.FirstOrDefault(s => s.item == item);
        return slot?.quantity ?? 0;
    }

    public bool HasItem(ItemCard item)
    {
        return GetItemCount(item) > 0;
    }
}
