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
            Debug.Log($"Added {card.cardName} to hand. Hand size: {Hand.Count}");
        }

    }

    public bool PlayCard(CardSO card, Player player, Pathogen target = null)
    {
        if (Hand.Contains(card))
        {
            Debug.Log($"PlayerCards: Playing {card.cardName}. Hand size before: {Hand.Count}");
            
            Hand.Remove(card);
            PlayedCards.Add(card);
            
            Debug.Log($"PlayerCards: Removed {card.cardName} from hand. Hand size now: {Hand.Count}");
            
            // Check if vaccine boost is active (double effect)
            bool hasVaccineBoost = player.HasVaccineBoost;
            
            if (hasVaccineBoost && !(card is ItemSO))
            {
                Debug.Log($"Vaccine Boost: Activating double effect for {card.cardName}!");
                // Set the boost flag before applying effect so cards can check it
                player.SetVaccineBoostActive(true);
            }
            
            // Apply card effect (cards will check for boost and double their own effects)
            Debug.Log($"PlayerCards: Applying effect for {card.cardName}");
            card.ApplyEffect(player, PlayedCards, target);
            Debug.Log($"PlayerCards: Effect applied. Hand size after effect: {Hand.Count}");
            
            // Consume boost after use
            if (hasVaccineBoost && !(card is ItemSO))
            {
                player.ConsumeVaccineBoost();
                Debug.Log($"Vaccine Boost: Effect doubled and boost consumed for {card.cardName}");
            }
            
            Debug.Log($"PlayerCards: Firing OnCardPlayed event for {card.cardName}");
            OnCardPlayed?.Invoke(card);
            Debug.Log($"Played {card.cardName}");
            return true;
        }
        Debug.Log($"Card {card.cardName} not found in hand!");
        return false;
    }

    public bool CanPlayCard(CardSO card)
    {
        return Hand.Contains(card);
    }

    public void ResetTurnStats()
    {
        // Reset activation flags for combo cards
        foreach (var card in PlayedCards)
        {
            if (card is BCellCardSO bCell)
                bCell.ResetActivation();
            else if (card is CytotoxicCellCardSO cytotoxic)
                cytotoxic.ResetActivation();
        }
        
        PlayedCards.Clear();
        Debug.Log("PlayerCards: Cleared played cards and reset combo activations for new turn");
    }

    /// <summary>
    /// Process all card effects at end of turn with proper combo resolution
    /// Call this after all cards have been played in a turn
    /// </summary>
    public void ProcessAllCardEffects(Player player, Pathogen target)
    {
        if (PlayedCards.Count == 0) return;

        Debug.Log($"PlayerCards: Processing {PlayedCards.Count} card effects");

        // Two-pass system for combo effects
        // Pass 1: Non-combo cards (Helper T-Cell, Macrophage, Natural Killer)
        foreach (var card in PlayedCards)
        {
            if (card is HelperTCellCardSO || card is MacrophageCardSO || card is NaturalKillerCardSO)
            {
                Debug.Log($"PlayerCards: Processing non-combo card: {card.cardName}");
                // These effects were already applied when played, skip re-processing
            }
        }

        // Pass 2: Combo-dependent cards (B-Cell, Cytotoxic) 
        // Re-process these to ensure they can find Helper T-Cell
        foreach (var card in PlayedCards)
        {
            if (card is BCellCardSO || card is CytotoxicCellCardSO)
            {
                Debug.Log($"PlayerCards: Re-processing combo card: {card.cardName}");
                // Re-apply effect now that all cards are in PlayedCards
                card.ApplyEffect(player, PlayedCards, target);
            }
        }

        Debug.Log("PlayerCards: All card effects processed");
    }
}
