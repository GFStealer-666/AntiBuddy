using UnityEngine;
using System.Collections.Generic;

public static class CardEffects
{
    // Common card effects that can be used by multiple cards
    
    public static void DealDamage(PathogenSO target, int damage)
    {
        if (target != null && target.maxHitPoints > 0)
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
}
