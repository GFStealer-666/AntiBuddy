using System;
using System.Collections.Generic;
public class Player
{
    public int HP { get; set; }
    public int Tokens { get; set; }
    public List<CardSO> Deck { get; set; } // Personal deck of cards

    public Player(int startingHP)
    {
        HP = startingHP;
        Tokens = 0;
        Deck = new List<CardSO>();
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP < 0) HP = 0;
        Console.WriteLine($"Player took {damage} damage. Current HP: {HP}");
    }

    public void Heal(int amount)
    {
        HP += amount;
        Console.WriteLine($"Player healed {amount} HP. Current HP: {HP}");
    }

    public void AddTokens(int amount)
    {
        Tokens += amount;
        Console.WriteLine($"Player received {amount} tokens. Current tokens: {Tokens}");
    }
}
