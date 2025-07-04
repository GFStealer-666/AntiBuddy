using UnityEngine;
using System;

public class PlayerDefense
{
    public int Defense { get; private set; }
    public int PercentageDefense { get; private set; }

    public event Action<int, int> OnDefenseChanged; // flatDefense, percentageDefense

    public PlayerDefense()
    {
        Defense = 0;
        PercentageDefense = 0;
    }

    public int CalculateActualDamage(int incomingDamage)
    {
        // Apply percentage defense first
        int damageAfterPercentage = incomingDamage;
        if (PercentageDefense > 0)
        {
            damageAfterPercentage = Mathf.RoundToInt(incomingDamage * (100 - PercentageDefense) / 100f);
        }
        
        // Then apply flat defense
        int actualDamage = Mathf.Max(0, damageAfterPercentage - Defense);
        return actualDamage;
    }

    public void AddDefense(int amount)
    {
        Defense += amount;
        OnDefenseChanged?.Invoke(Defense, PercentageDefense);
        Console.WriteLine($"Player gained {amount} defense. Current defense: {Defense}");
    }

    public void AddPercentageDefense(int percentage)
    {
        PercentageDefense = Mathf.Max(PercentageDefense, percentage);
        OnDefenseChanged?.Invoke(Defense, PercentageDefense);
        Console.WriteLine($"Player gained {percentage}% defense. Current percentage defense: {PercentageDefense}%");
    }

    public void ResetDefense()
    {
        Defense = 0;
        PercentageDefense = 0;
        OnDefenseChanged?.Invoke(Defense, PercentageDefense);
    }
}
