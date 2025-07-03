using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // reference to the Player instance
    // This class manages player actions such as healing, attacking pathogens, and drawing cards.

    private Player player;
    private DeckManager deckManager;
    private readonly int MAX_HAND_SIZE = 5;
    // Removed CARDS_PER_TURN as it will be determined by cards played in previous turn

    public PlayerManager(Player p, DeckManager deck = null)
    {
        player = p;
        deckManager = deck;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetDeckManager(DeckManager deck)
    {
        deckManager = deck;
    }

    public void StartTurn(int cardsToDraw = 2)
    {
        player.ResetTurnStats();
        DrawCards(cardsToDraw);
    }

    public void HealPlayer(int amount)
    {
        player.Heal(amount);
    }

    public bool AttackPathogen(PathogenSO pathogen, int damage)
    {
        if (pathogen != null && pathogen.health > 0)
        {
            pathogen.TakeDamage(damage);
            Debug.Log($"Attacked {pathogen.pathogenName} for {damage} damage");
            return true;
        }
        return false;
    }

    public bool PlayCard(CardSO card, PathogenSO target = null)
    {
        if (player.Hand.Contains(card))
        {
            return player.PlayCard(card, target);
        }
        return false;
    }

    public void DrawCards(int count)
    {
        if (deckManager == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (player.Hand.Count >= MAX_HAND_SIZE)
            {
                break;
            }

            CardSO drawnCard = deckManager.DrawCard();
            if (drawnCard != null)
            {
                player.AddCardToHand(drawnCard);
            }
            else
            {
                break;
            }
        }
    }

    public List<CardSO> GetPlayerHand()
    {
        return new List<CardSO>(player.Hand);
    }

    public List<CardSO> GetPlayedCards()
    {
        return new List<CardSO>(player.PlayedCards);
    }

    public bool CanPlayCard(CardSO card)
    {
        return player.Hand.Contains(card);
    }

    public void ApplyCardEffect(CardSO card, PathogenSO target = null)
    {
        if (CanPlayCard(card))
        {
            card.ApplyEffect(player, player.PlayedCards, target);
        }
    }

    public int GetHandSize()
    {
        return player.Hand.Count;
    }
    public int GetMaxHandSize()
    {
        return MAX_HAND_SIZE;
    }
    // Removed GetMaxCardsPerTurn() as this is now managed by TurnManager

    public PlayerStats GetPlayerStats()
    {
        return new PlayerStats
        {
            HP = player.HP,
            MaxHP = player.MaxHP,
            Defense = player.Defense,
            Tokens = player.Tokens,
            HandSize = player.Hand.Count,
            PlayedCardsCount = player.PlayedCards.Count
        };
    }
}

[System.Serializable]
public struct PlayerStats
{
    public int HP;
    public int MaxHP;
    public int Defense;
    public int Tokens;
    public int HandSize;
    public int PlayedCardsCount;
}
