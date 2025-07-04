using UnityEngine;
using System;

/// <summary>
/// Handles pathogen health, damage, and death logic only
/// </summary>
public class PathogenHealth
{
    private PathogenData pathogenData;
    
    public event Action<int> OnHealthChanged;
    public event Action<int> OnDamageTaken;
    public event Action OnPathogenDied;
    
    public PathogenHealth(PathogenData data)
    {
        pathogenData = data;
    }
    
    public void TakeDamage(int damage)
    {
        if (!pathogenData.isAlive || damage <= 0) return;
        
        int actualDamage = Mathf.Min(damage, pathogenData.currentHitPoints);
        pathogenData.currentHitPoints -= actualDamage;
        
        if (pathogenData.currentHitPoints <= 0)
        {
            pathogenData.currentHitPoints = 0;
            pathogenData.isAlive = false;
            OnPathogenDied?.Invoke();
        }
        
        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(pathogenData.currentHitPoints);
        
        Debug.Log($"{pathogenData.PathogenName} took {actualDamage} damage. HP: {pathogenData.currentHitPoints}/{pathogenData.MaxHitPoints}");
    }
    
    public void Heal(int amount)
    {
        if (!pathogenData.isAlive || amount <= 0) return;
        
        int oldHealth = pathogenData.currentHitPoints;
        pathogenData.currentHitPoints = Mathf.Min(pathogenData.currentHitPoints + amount, pathogenData.MaxHitPoints);
        
        int actualHealing = pathogenData.currentHitPoints - oldHealth;
        
        if (actualHealing > 0)
        {
            OnHealthChanged?.Invoke(pathogenData.currentHitPoints);
            Debug.Log($"{pathogenData.PathogenName} healed for {actualHealing} HP. Current HP: {pathogenData.currentHitPoints}/{pathogenData.MaxHitPoints}");
        }
    }
    
    public bool IsAlive()
    {
        return pathogenData.isAlive && pathogenData.currentHitPoints > 0;
    }
    
    public int GetCurrentHealth()
    {
        return pathogenData.currentHitPoints;
    }
    
    public int GetMaxHealth()
    {
        return pathogenData.MaxHitPoints;
    }
    
    public float GetHealthPercentage()
    {
        return pathogenData.GetHealthPercentage();
    }
}