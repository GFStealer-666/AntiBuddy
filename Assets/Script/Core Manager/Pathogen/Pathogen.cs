using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Main pathogen class that coordinates all pathogen components
/// Acts as a facade for the pathogen subsystems
/// Runtime representation of a pathogen in the game
/// Contains data, health, abilities, and attack logic
/// </summary>
public class Pathogen
{
    private PathogenData data;
    private PathogenHealth health;
    private PathogenAbility ability;
    private PathogenAttack attack;
    
    public event Action<Pathogen> OnPathogenDied;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnDamageTaken;
    public event Action<int> OnAttackExecuted;
    
    public Pathogen(PathogenSO template)
    {
        // Initialize data
        data = new PathogenData(template);
        
        // Initialize components
        health = new PathogenHealth(data);
        ability = new PathogenAbility(data);
        attack = new PathogenAttack(data, ability);
        
        // Wire up events
        health.OnPathogenDied += () => OnPathogenDied?.Invoke(this);
        health.OnHealthChanged += (hp) => OnHealthChanged?.Invoke(hp);
        health.OnDamageTaken += (dmg) => OnDamageTaken?.Invoke(dmg);
        attack.OnAttackExecuted += (dmg) => OnAttackExecuted?.Invoke(dmg);
        
        // Handle regeneration ability
        ability.OnAbilityActivated += HandleAbilityActivated;
    }
    
    private void HandleAbilityActivated(PathogenAbilityType abilityType)
    {
        switch (abilityType)
        {
            case PathogenAbilityType.Regeneration:
                int healAmount = ability.GetHealingAmount();
                if (healAmount > 0)
                {
                    health.Heal(healAmount);
                }
                break;
                
            case PathogenAbilityType.Mutation:
                int mutationHeal = data.template.GetAbilityValue(PathogenAbilityType.Mutation);
                if (mutationHeal > 0)
                {
                    health.Heal(mutationHeal);
                }
                break;
        }
    }
    
    #region Public Interface
    
    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }
    
    public void Heal(int amount)
    {
        health.Heal(amount);
    }
    
    public void AttackPlayer(Player player)
    {
        attack.AttackPlayer(player);
    }
    
    public void ProcessTurnStart(List<CardSO> playedCards)
    {
        ability.ProcessTurnStart(playedCards);
    }
    
    public bool IsCardBlocked(System.Type cardType)
    {
        return ability.IsCardBlocked(cardType);
    }
    
    #endregion
    
    #region Getters
    
    public PathogenData GetData()
    {
        return data;
    }
    public PathogenHealth GetHealth()
    {
        return health;
    }
    
    public string GetPathogenName()
    {
        return data.PathogenName;
    }
    
    public int GetCurrentHealth()
    {
        return health.GetCurrentHealth();
    }
    
    public int GetMaxHealth()
    {
        return health.GetMaxHealth();
    }
    
    public float GetHealthPercentage()
    {
        return health.GetHealthPercentage();
    }
    
    public int GetAttackPower()
    {
        return attack.GetAttackPower();
    }
    
    public int GetTotalDamageThisTurn()
    {
        return attack.GetTotalDamageThisTurn();
    }
    
    public bool IsAlive()
    {
        return health.IsAlive();
    }
    
    public bool CanAttackThisTurn()
    {
        return ability.CanAttackThisTurn();
    }
    
    public Sprite GetSprite()
    {
        return data.PathogenSprite;
    }
    
    public int GetCurrentTurn()
    {
        return data.currentTurn;
    }
    
    #endregion
}