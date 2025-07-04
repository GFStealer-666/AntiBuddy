using System.Collections.Generic;
using System;

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
            card.ApplyEffect(player, PlayedCards, target);
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
