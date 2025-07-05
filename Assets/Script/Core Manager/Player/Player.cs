using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public PlayerHealth PlayerHealth { get; private set; }
    public PlayerDefense PlayerDefense { get; private set; }
    public PlayerTokens PlayerTokens { get; private set; }
    public PlayerCards PlayerCards { get; private set; }
    public PlayerInventory PlayerInventory { get; private set; }

    public Player(int startingHP)
    {
        PlayerHealth = new PlayerHealth(startingHP);
        PlayerDefense = new PlayerDefense();
        PlayerTokens = new PlayerTokens();
        PlayerCards = new PlayerCards();
        PlayerInventory = new PlayerInventory();
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = PlayerDefense.CalculateActualDamage(damage);
        PlayerHealth.TakeDamage(actualDamage);
        
        Console.WriteLine($"Player took {actualDamage} damage (original: {damage}, blocked: {damage - actualDamage})");
    }

    public bool PlayCard(CardSO card, Pathogen target = null)
    {
        return PlayerCards.PlayCard(card, this, target);
    }

    public bool CanPlayCard(CardSO card)
    {
        return PlayerCards.CanPlayCard(card);
    }

    public bool UseItem(ItemSO item)
    {
        if (item.CanUse(this))
        {
            item.UseItem(this);
            return true;
        }
        return false;
    }

    public bool CanUseItem(ItemSO item)
    {
        return item.CanUse(this);
    }

    public void ResetTurnStats()
    {
        PlayerDefense.ResetDefense();
        PlayerCards.ResetTurnStats();
    }

    // Convenience properties for backward compatibility
    public int HP => PlayerHealth.HP;
    public int MaxHP => PlayerHealth.MaxHP;
    public int Tokens => PlayerTokens.Tokens;
    public int Defense => PlayerDefense.Defense;
    public int PercentageDefense => PlayerDefense.PercentageDefense;
    public List<CardSO> Deck => PlayerCards.Deck;
    public List<CardSO> Hand => PlayerCards.Hand;
    public List<InventorySlot> Item => PlayerInventory.Items;
    public List<CardSO> PlayedCards => PlayerCards.PlayedCards;
}
