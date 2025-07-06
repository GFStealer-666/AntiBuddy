using UnityEngine;
using System;

/// <summary>
/// Handles pathogen attack logic and damage calculation
/// </summary>
public class PathogenAttack
{
    private PathogenData pathogenData;
    private PathogenAbility pathogenAbility;
    
    public event Action<int> OnAttackExecuted;
    public event Action OnAttackSkipped;
    
    public PathogenAttack(PathogenData data, PathogenAbility ability)
    {
        pathogenData = data;
        pathogenAbility = ability;
    }
    
    public void AttackPlayer(Player player)
    {
        if (!pathogenData.isAlive)
        {
            Debug.Log($"{pathogenData.PathogenName} is dead and cannot attack");
            return;
        }
        
        if (!pathogenAbility.CanAttackThisTurn()) // * Check if the pathogen can attack this turn
        {
            Debug.Log($"{pathogenData.PathogenName} is not attacking this turn");
            OnAttackSkipped?.Invoke();
            return;
        }
        
        int totalDamage = CalculateDamage();
        
        if (totalDamage > 0)
        {
            player.TakeDamage(totalDamage);
            OnAttackExecuted?.Invoke(totalDamage);
            Debug.Log($"{pathogenData.PathogenName} attacks for {totalDamage} damage!");
        }
    }
    
    private int CalculateDamage()
    {
        int baseDamage = pathogenData.AttackPower;
        int extraDamage = pathogenAbility.GetExtraDamage();
        
        return baseDamage + extraDamage;
    }
    
    public int GetAttackPower()
    {
        return pathogenData.AttackPower;
    }
    
    public int GetTotalDamageThisTurn()
    {
        return CalculateDamage();
    }
}