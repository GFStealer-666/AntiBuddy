using UnityEngine;
using System;

public class PlayerHealth
{
    public int HP { get; private set; }
    public int MaxHP { get; private set; }

    public event Action<int, int> OnHealthChanged; // currentHP, maxHP
    public event Action OnPlayerDied;

    public PlayerHealth(int startingHP)
    {
        HP = startingHP;
        MaxHP = startingHP;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(0, damage);
        HP -= actualDamage;
        if (HP < 0) HP = 0;
        
        OnHealthChanged?.Invoke(HP, MaxHP);
        if (HP <= 0)
        {
            OnPlayerDied?.Invoke();
        }
        
        Console.WriteLine($"Player took {actualDamage} damage. Current HP: {HP}");
    }

    public void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        OnHealthChanged?.Invoke(HP, MaxHP);
        Console.WriteLine($"Player healed {amount} HP. Current HP: {HP}");
    }

    public void IncreaseMaxHP(int amount)
    {
        MaxHP += amount;
        OnHealthChanged?.Invoke(HP, MaxHP);
        Console.WriteLine($"Max HP increased by {amount}. New Max HP: {MaxHP}");
    }

    public bool IsAlive => HP > 0;
    public float HealthPercentage => MaxHP > 0 ? (float)HP / MaxHP : 0f;
}
