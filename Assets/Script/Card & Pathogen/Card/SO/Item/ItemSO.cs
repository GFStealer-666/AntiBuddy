using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item Card", menuName = "Cards/Item Card")]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public string description;
    public Sprite itemImage;
    public int cost;

    [Header("Cost Settings")]
    public CostType costType = CostType.Tokens;
    public int hpCost;

    [Header("Item Properties")]
    public ItemType itemType;
    public bool isConsumable = true;
    public int maxStack = 1;

    [Header("Effects")]
    public int healthBoost;
    public int defenseBoost;
    public int percentageDefenseBoost;
    public int tokenGeneration;
    public bool grantsPermanentEffect;

    public virtual void UseItem(Player player)
    {
        // Pay the cost first
        if (!PayCost(player))
        {
            Debug.LogWarning($"Cannot use {itemName} - insufficient resources");
            return;
        }

        switch (itemType)
        {
            case ItemType.Healing:
                player.PlayerHealth.Heal(healthBoost);
                break;
            case ItemType.Defense:
                player.PlayerDefense.AddDefense(defenseBoost);
                if (percentageDefenseBoost > 0)
                    player.PlayerDefense.AddPercentageDefense(percentageDefenseBoost);
                break;
            case ItemType.TokenGenerator:
                player.PlayerTokens.AddTokens(tokenGeneration);
                break;
            case ItemType.Utility:
                ApplyUtilityEffect(player);
                break;
        }

        Console.WriteLine($"Used item: {itemName}");
    }

    private bool PayCost(Player player)
    {
        switch (costType)
        {
            case CostType.Tokens:
                if (player.Tokens >= cost)
                {
                    player.PlayerTokens.SpendTokens(cost);
                    return true;
                }
                break;
            case CostType.Health:
                if (player.HP > hpCost) // Ensure player doesn't die from using item
                {
                    player.PlayerHealth.TakeDamage(hpCost);
                    return true;
                }
                break;
            case CostType.Both:
                if (player.Tokens >= cost && player.HP > hpCost)
                {
                    player.PlayerTokens.SpendTokens(cost);
                    player.PlayerHealth.TakeDamage(hpCost);
                    return true;
                }
                break;
        }
        return false;
    }

    protected virtual void ApplyUtilityEffect(Player player)
    {
        // Override in derived classes for custom utility effects
    }

    public virtual bool CanUse(Player player)
    {
        switch (costType)
        {
            case CostType.Tokens:
                return player.Tokens >= cost;
            case CostType.Health:
                return player.HP > hpCost; // Must have more HP than cost to survive
            case CostType.Both:
                return player.Tokens >= cost && player.HP > hpCost;
            default:
                return true;
        }
    }
}

public enum ItemType
{
    Healing,
    Defense,
    TokenGenerator,
    Utility
}

public enum CostType
{
    Tokens,
    Health,
    Both
}
