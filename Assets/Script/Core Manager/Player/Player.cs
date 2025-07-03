using System;
using System.Collections.Generic;
using UnityEngine;
public class Player
{
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Tokens { get; set; }
    public int Defense { get; set; } // Flat defense amount
    public int PercentageDefense { get; set; } // Percentage defense (0-100)
    public List<CardSO> Deck { get; set; } // Personal deck of cards
    public List<CardSO> Hand { get; set; } // Cards in player's hand
    public List<CardSO> PlayedCards { get; set; } // Cards played this turn

    public Player(int startingHP)
    {
        HP = startingHP;
        MaxHP = startingHP;
        Tokens = 0;
        Defense = 0;
        PercentageDefense = 0;
        Deck = new List<CardSO>();
        Hand = new List<CardSO>();
        PlayedCards = new List<CardSO>();
    }

    public void TakeDamage(int damage)
    {
        // Apply percentage defense first
        int damageAfterPercentage = damage;
        if (PercentageDefense > 0)
        {
            damageAfterPercentage = Mathf.RoundToInt(damage * (100 - PercentageDefense) / 100f);
        }
        
        // Then apply flat defense
        int actualDamage = Mathf.Max(0, damageAfterPercentage - Defense);
        HP -= actualDamage;
        if (HP < 0) HP = 0;
        
        Console.WriteLine($"Player took {actualDamage} damage (original: {damage}, after {PercentageDefense}% defense: {damageAfterPercentage}, blocked {damageAfterPercentage - actualDamage}). Current HP: {HP}");
    }

    public void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        Console.WriteLine($"Player healed {amount} HP. Current HP: {HP}");
    }

    public void AddTokens(int amount)
    {
        Tokens += amount;
        Console.WriteLine($"Player received {amount} tokens. Current tokens: {Tokens}");
    }

    public void AddDefense(int amount)
    {
        Defense += amount;
        Console.WriteLine($"Player gained {amount} defense. Current defense: {Defense}");
    }

    public void AddPercentageDefense(int percentage)
    {
        PercentageDefense = Mathf.Max(PercentageDefense, percentage); // Take the highest percentage
        Console.WriteLine($"Player gained {percentage}% defense. Current percentage defense: {PercentageDefense}%");
    }

    public void ResetDefense()
    {
        Defense = 0;
        PercentageDefense = 0;
    }

    public void AddCardToHand(CardSO card)
    {
        if (card != null)
        {
            Hand.Add(card);
            Console.WriteLine($"Added {card.cardName} to hand. Hand size: {Hand.Count}");
        }
    }

    public bool PlayCard(CardSO card, PathogenSO target = null)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            PlayedCards.Add(card);
            card.ApplyEffect(this, PlayedCards, target);
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
        ResetDefense();
        PlayedCards.Clear();
    }
}
