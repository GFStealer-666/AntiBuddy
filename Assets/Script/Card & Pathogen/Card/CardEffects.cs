using UnityEngine;
using System.Collections.Generic;

public static class CardEffects
{
    // Common card effects that can be used by multiple cards
    
    public static void DealDamage(PathogenSO target, int damage)
    {
        if (target != null && target.health > 0)
        {
            target.TakeDamage(damage);
        }
    }

    public static void HealPlayer(Player player, int amount)
    {
        player.Heal(amount);
    }

    public static void AddDefense(Player player, int amount)
    {
        player.AddDefense(amount);
    }

    public static void AddPercentageDefense(Player player, int percentage)
    {
        player.AddPercentageDefense(percentage);
    }

    public static void AddTokens(Player player, int amount)
    {
        player.AddTokens(amount);
    }

    public static void DrawCards(Player player, DeckManager deckManager, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardSO card = deckManager?.DrawCard();
            if (card != null)
            {
                player.AddCardToHand(card);
            }
            else
            {
                break; // No more cards in deck
            }
        }
    }

    public static void AddSpecificCardToHand(Player player, CardSO cardToAdd)
    {
        if (cardToAdd != null)
        {
            player.AddCardToHand(cardToAdd);
        }
    }

    public static int CountCardType<T>(List<CardSO> cards) where T : CardSO
    {
        int count = 0;
        foreach (var card in cards)
        {
            if (card is T)
                count++;
        }
        return count;
    }

    public static bool HasCardType<T>(List<CardSO> cards) where T : CardSO
    {
        return CountCardType<T>(cards) > 0;
    }

    // Advanced effects for specific card combinations
    public static void ApplyComboEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Check for specific combinations and apply bonus effects
        bool hasBCell = HasCardType<BCellCardSO>(playedCards);
        bool hasHelperT = HasCardType<HelperTCellCardSO>(playedCards);
        bool hasCytotoxic = HasCardType<CytotoxicCellCardSO>(playedCards);
        bool hasMacrophage = HasCardType<MacrophageCardSO>(playedCards);

        // Immune system coordination bonus
        if (hasBCell && hasHelperT && hasCytotoxic)
        {
            // Full immune response combo
            player.AddDefense(5);
            player.AddTokens(3);
            if (target != null)
            {
                target.TakeDamage(10); // Bonus damage from coordinated attack
            }
            Debug.Log("FULL IMMUNE RESPONSE! Coordinated attack bonus activated!");
        }
        else if ((hasBCell && hasHelperT) || (hasCytotoxic && hasHelperT))
        {
            // Partial combo bonus
            player.AddTokens(1);
            Debug.Log("Immune cell coordination bonus!");
        }

        // Innate + Adaptive immunity bonus
        if (hasMacrophage && (hasBCell || hasCytotoxic))
        {
            player.AddDefense(3);
            Debug.Log("Innate and adaptive immunity working together!");
        }
    }
    
    // Specialized immune system effects
    
    public static void ActivateAntibodyResponse(Player player, List<CardSO> playedCards, PathogenSO target, int baseHealing = 10)
    {
        // B-Cells create antibodies that provide sustained healing and protection
        if (HasCardType<BCellCardSO>(playedCards))
        {
            int antibodyPower = CountCardType<BCellCardSO>(playedCards) * baseHealing;
            HealPlayer(player, antibodyPower);
            AddDefense(player, antibodyPower / 2);
            Debug.Log($"Antibody response activated: {antibodyPower} healing, {antibodyPower/2} defense");
        }
    }
    
    public static void CytokineStorm(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // When too many immune cells are active, can cause damage to player
        int immuneCellCount = CountCardType<BCellCardSO>(playedCards) +
                             CountCardType<CytotoxicCellCardSO>(playedCards) +
                             CountCardType<MacrophageCardSO>(playedCards) +
                             CountCardType<HelperTCellCardSO>(playedCards);
        
        if (immuneCellCount >= 4)
        {
            // Cytokine storm - powerful but damages player
            DealDamage(target, immuneCellCount * 8);
            player.TakeDamage(immuneCellCount * 2);
            Debug.Log($"CYTOKINE STORM! Massive damage to pathogen but {immuneCellCount * 2} self-damage!");
        }
    }
    
    public static void MemoryResponse(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Memory cells provide bonus when same card types are played repeatedly
        bool hasBCellMemory = CountCardType<BCellCardSO>(playedCards) >= 2;
        bool hasTCellMemory = CountCardType<CytotoxicCellCardSO>(playedCards) >= 2;
        
        if (hasBCellMemory)
        {
            AddTokens(player, 2);
            Debug.Log("B-Cell memory response: +2 tokens");
        }
        
        if (hasTCellMemory)
        {
            DealDamage(target, 10);
            Debug.Log("T-Cell memory response: +10 damage");
        }
    }
    
    public static void InflammatoryResponse(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Macrophages and other innate cells create inflammation
        if (HasCardType<MacrophageCardSO>(playedCards))
        {
            int macrophageCount = CountCardType<MacrophageCardSO>(playedCards);
            
            // Inflammation helps fight pathogens but can damage player if excessive
            DealDamage(target, macrophageCount * 5);
            
            if (macrophageCount >= 3)
            {
                player.TakeDamage(macrophageCount);
                Debug.Log($"Excessive inflammation: {macrophageCount * 5} pathogen damage, {macrophageCount} self-damage");
            }
            else
            {
                Debug.Log($"Healthy inflammation: {macrophageCount * 5} pathogen damage");
            }
        }
    }
}
