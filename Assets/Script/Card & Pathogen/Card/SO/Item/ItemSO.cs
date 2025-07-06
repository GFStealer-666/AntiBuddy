using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Card", menuName = "Cards/Item Card")]
public class ItemSO : CardSO  // Inherit from CardSO instead of ScriptableObject
{
    /// <summary>
    /// Items work exactly like cards now - they inherit from CardSO
    /// Effects are applied immediately when played, but some effects (like defense boosts)
    /// naturally last until the next player turn starts due to game mechanics.
    /// 
    /// Benefits of this simplified system:
    /// - Items appear in hand exactly like cards
    /// - Can be played using the same card system  
    /// - No complex cost management needed
    /// - Effects are clear and immediate
    /// </summary>

    [Header("Item-Specific Properties")]
    public ItemType itemType;
    public bool isConsumable = true;
    public int maxStack = 1;

    [Header("Item Effects (Active until next player turn)")]
    public int healthBoost;
    public int defenseBoost;
    public int percentageDefenseBoost;
    public int tokenGeneration;

    [Header("Item Cost")]
    public ItemCostType costType = ItemCostType.Tokens;
    public int tokenCost = 5;
    public int hpCost = 5;
    
    [Header("Random Cost Settings")]
    public bool useRandomCost = false;
    public int minRandomTokenCost = 2;
    public int maxRandomTokenCost = 6;
    public int minRandomHpCost = 1;
    public int maxRandomHpCost = 3;

    // Override the card effect method from CardSO
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Apply immediate effects
        switch (itemType)
        {
            case ItemType.Healing:
                if (healthBoost > 0)
                    CardEffects.HealPlayer(player, healthBoost);
                break;
                
            case ItemType.Defense:
                if (defenseBoost > 0)
                    CardEffects.AddDefense(player, defenseBoost);
                if (percentageDefenseBoost > 0)
                    CardEffects.AddPercentageDefense(player, percentageDefenseBoost);
                break;
                
            case ItemType.TokenGenerator:
                if (tokenGeneration > 0)
                    CardEffects.AddTokens(player, tokenGeneration);
                break;
                
            case ItemType.Utility:
                ApplyUtilityEffect(player);
                break;
        }

        Debug.Log($"Used item: {cardName} - Effect lasts until next player turn");
    }

    /// <summary>
    /// Randomize costs if useRandomCost is enabled
    /// </summary>
    public virtual void RandomizeCosts()
    {
        if (!useRandomCost) return;
        
        tokenCost = UnityEngine.Random.Range(minRandomTokenCost, maxRandomTokenCost + 1);
        hpCost = UnityEngine.Random.Range(minRandomHpCost, maxRandomHpCost + 1);
        
        Debug.Log($"Randomized costs for {cardName}: {tokenCost} tokens, {hpCost} HP");
    }

    protected virtual void ApplyUtilityEffect(Player player)
    {
        // Override in derived classes for custom utility effects
        // For example: immunostimulant, vaccine, etc.
    }
}

public enum ItemType
{
    Healing,
    Defense,
    TokenGenerator,
    Utility
}

public enum ItemCostType
{
    Tokens,
    Health,
    Either // Player can choose tokens OR health
}
