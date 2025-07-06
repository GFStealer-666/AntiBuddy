using System.Collections.Generic;
using System;
using UnityEngine;



public class PlayerCards
{
    public List<CardSO> Deck { get; private set; }
    public List<CardSO> Hand { get; private set; }
    public List<CardSO> PlayedCards { get; private set; }

    public event Action<CardSO> OnCardAddedToHand;
    public event Action<CardSO> OnCardPlayed;

    public PlayerCards()
    {
        Deck = new List<CardSO>();
        Hand = new List<CardSO>();
        PlayedCards = new List<CardSO>();
    }

    public void AddCardToHand(CardSO card)
    {
        if (card != null)
        {
            Hand.Add(card);
            OnCardAddedToHand?.Invoke(card);
            Console.WriteLine($"Added {card.cardName} to hand. Hand size: {Hand.Count}");
        }
    }

    public bool PlayCard(CardSO card, Player player, Pathogen target = null)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            PlayedCards.Add(card);
            
            // Check if vaccine boost is active (double effect)
            bool hasVaccineBoost = player.HasVaccineBoost;
            
            // Apply card effect
            card.ApplyEffect(player, PlayedCards, target);
            
            // If vaccine boost was active, apply effect again
            if (hasVaccineBoost && !(card is ItemSO))
            {
                Debug.Log($"Vaccine Boost: Doubling effect of {card.cardName}!");
                card.ApplyEffect(player, PlayedCards, target);
                player.ConsumeVaccineBoost(); // Remove boost after use
            }
            
            OnCardPlayed?.Invoke(card);
            Console.WriteLine($"Played {card.cardName}");
            return true;
        }
        Console.WriteLine($"Card {card.cardName} not found in hand!");
        return false;
    }

    public bool CanPlayCard(CardSO card)
    {
        return Hand.Contains(card);
    }

    public void ResetTurnStats()
    {
        PlayedCards.Clear();
    }
}
