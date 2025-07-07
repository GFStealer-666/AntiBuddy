using UnityEngine;
using System.Collections.Generic;

public static class CardEffects
{
    // Common card effects that can be used by multiple cards
    
    #region Vaccine Boost Helpers
    
    /// <summary>
    /// Apply a numerical effect with vaccine boost consideration
    /// </summary>
    private static int ApplyVaccineBoost(Player player, int baseValue)
    {
        if (player.IsVaccineBoostActive())
        {
            Debug.Log($"Vaccine Boost: Doubling effect from {baseValue} to {baseValue * 2}");
            return baseValue * 2;
        }
        return baseValue;
    }
    
    #endregion
    
    #region Enhanced Card Effects (Vaccine Boost Compatible)
    
    public static void DealDamageWithBoost(Player player, Pathogen target, int damage)
    {
        int finalDamage = ApplyVaccineBoost(player, damage);
        DealDamage(target, finalDamage);
    }
    
    public static void HealPlayerWithBoost(Player player, int amount)
    {
        int finalAmount = ApplyVaccineBoost(player, amount);
        HealPlayer(player, finalAmount);
    }
    
    public static void AddDefenseWithBoost(Player player, int amount)
    {
        int finalAmount = ApplyVaccineBoost(player, amount);
        AddDefense(player, finalAmount);
    }
    
    public static void AddPercentageDefenseWithBoost(Player player, int percentage)
    {
        int finalPercentage = ApplyVaccineBoost(player, percentage);
        AddPercentageDefense(player, finalPercentage);
    }
    
    public static void AddTokensWithBoost(Player player, int amount)
    {
        int finalAmount = ApplyVaccineBoost(player, amount);
        AddTokens(player, finalAmount);
    }
    
    #endregion
    
    #region Original Effects (For Backwards Compatibility)
    
    public static void DealDamage(Pathogen target, int damage)
    {
        if (target != null && target.GetMaxHealth() > 0)
        {
            target.TakeDamage(damage);
        }
    }

    public static void HealPlayer(Player player, int amount)
    {
        player.PlayerHealth.Heal(amount);
    }

    public static void AddDefense(Player player, int amount)
    {
        player.PlayerDefense.AddDefense(amount);
    }

    public static void AddPercentageDefense(Player player, int percentage)
    {
        player.PlayerDefense.AddPercentageDefense(percentage);
    }

    public static void AddTokens(Player player, int amount)
    {
        player.PlayerTokens.AddTokens(amount);
    }

    public static void DrawCards(Player player, DeckManager deckManager, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardSO card = deckManager?.DrawCard();
            if (card != null)
            {
                player.PlayerCards.AddCardToHand(card);
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
            player.PlayerCards.AddCardToHand(cardToAdd);
            
            // Log card addition via GameManager
            var gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.LogCardAddedToHand(cardToAdd.cardName);
            }
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
    #endregion
}
