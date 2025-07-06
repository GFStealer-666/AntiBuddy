using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class PathogenAbility
{
    private PathogenData pathogenData;
    
    // Runtime state for special behaviors
    private Dictionary<string, object> runtimeState = new Dictionary<string, object>();
    
    public event Action<PathogenAbilityType> OnAbilityActivated;
    public event Action<System.Type> OnCardBlocked;
    public event Action<int> OnMutation;
    public event Action<float> OnResistanceGained;
    
    public PathogenAbility(PathogenData data)
    {
        pathogenData = data;
        InitializeRuntimeState();
    }
    
    private void InitializeRuntimeState()
    {
        // Initialize any pathogen-specific runtime state
        runtimeState["turnsSinceMutation"] = 0;
        runtimeState["isResistant"] = false;
        runtimeState["totalMultiplication"] = 0;
    }
    
    public void ProcessTurnStart(List<CardSO> playedCards)
    {
        pathogenData.currentTurn++;
        
        // Calculate attack timing based on attackInterval
        pathogenData.canAttackThisTurn = ShouldAttackThisTurn();
        
        // Process each ability in the dictionary
        ProcessAllAbilities(playedCards);
    }
    
    private bool ShouldAttackThisTurn()
    {
        // Attack on first turn (turn 1), then follow the interval
        // Example: If interval is 3, attack on turns 1, 4, 7, 10, etc.
        bool shouldAttack = ((pathogenData.currentTurn - 1) % pathogenData.AttackInterval) == 0;
        
        if (shouldAttack)
        {
            Debug.Log($"{pathogenData.PathogenName}: ATTACKING this turn (Turn {pathogenData.currentTurn}) - Attack interval: {pathogenData.AttackInterval}");
        }
        else
        {
            int turnsUntilNextAttack = pathogenData.AttackInterval - ((pathogenData.currentTurn - 1) % pathogenData.AttackInterval);
            Debug.Log($"{pathogenData.PathogenName}: Resting this turn (Turn {pathogenData.currentTurn}) - Next attack in {turnsUntilNextAttack} turn(s)");
        }
        
        return shouldAttack;
    }
    
    private void ProcessAllAbilities(List<CardSO> playedCards)
    {
        foreach (var kvp in pathogenData.Abilities)
        {
            PathogenAbilityType abilityType = kvp.Key;
            PathogenAbilityData abilityData = kvp.Value;
            
            // Check if this ability should trigger this turn
            bool shouldTrigger = abilityData.turnInterval > 0 && 
                               (pathogenData.currentTurn % abilityData.turnInterval) == 0;
            
            if (shouldTrigger)
            {
                ProcessSpecificAbility(abilityType, abilityData, playedCards);
            }
        }
    }
    
    private void ProcessSpecificAbility(PathogenAbilityType abilityType, PathogenAbilityData abilityData, List<CardSO> playedCards)
    {
        switch (abilityType)
        {
            case PathogenAbilityType.BlockCards:
                ProcessCardBlocking(abilityData);
                break;
                
            case PathogenAbilityType.ExtraDamage:
                OnAbilityActivated?.Invoke(PathogenAbilityType.ExtraDamage);
                Debug.Log($"{pathogenData.PathogenName}: Extra damage! (+{abilityData.value} damage)");
                break;
                
            case PathogenAbilityType.Regeneration:
                OnAbilityActivated?.Invoke(PathogenAbilityType.Regeneration);
                Debug.Log($"{pathogenData.PathogenName}: Regenerating! (+{abilityData.value} HP)");
                break;
                
            case PathogenAbilityType.Mutation:
                OnAbilityActivated?.Invoke(PathogenAbilityType.Mutation);
                OnMutation?.Invoke(abilityData.value);
                Debug.Log($"{pathogenData.PathogenName}: Mutating! (+{abilityData.value} HP)");
                break;
        }
    }
    
    private void ProcessCardBlocking(PathogenAbilityData abilityData)
    {
        // Block specific card types based on the CardSO references
        foreach (CardSO targetCard in abilityData.targetCards)
        {
            if (targetCard != null)
            {
                System.Type cardType = targetCard.GetType();
                OnCardBlocked?.Invoke(cardType);
                Debug.Log($"{pathogenData.PathogenName}: Blocking {cardType.Name}");
            }
        }
        
        OnAbilityActivated?.Invoke(PathogenAbilityType.BlockCards);
        
        // Create a readable list of blocked card names
        var blockedCardNames = abilityData.targetCards
            .Where(card => card != null)
            .Select(card => card.cardName)
            .ToList();
            
        Debug.Log($"{pathogenData.PathogenName}: Blocking cards: {string.Join(", ", blockedCardNames)}");
    }
    
    #region Public Interface
    
    public bool CanAttackThisTurn() => pathogenData.canAttackThisTurn;
    
    public bool IsCardBlocked(System.Type cardType)
    {
        // Check if pathogen has blocking ability
        var blockingAbility = pathogenData.template.GetAbility(PathogenAbilityType.BlockCards);
        if (blockingAbility == null) return false;
        
        // Check if blocking is active this turn
        bool isBlockingActive = blockingAbility.turnInterval > 0 && 
                               (pathogenData.currentTurn % blockingAbility.turnInterval) == 0;
        
        if (!isBlockingActive) return false;
        
        // Check if this specific card type is in the target list
        foreach (CardSO targetCard in blockingAbility.targetCards)
        {
            if (targetCard != null && targetCard.GetType() == cardType)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public int GetExtraDamage()
    {
        var extraDamageAbility = pathogenData.template.GetAbility(PathogenAbilityType.ExtraDamage);
        if (extraDamageAbility == null) return 0;
        
        bool isActive = extraDamageAbility.turnInterval > 0 && 
                       (pathogenData.currentTurn % extraDamageAbility.turnInterval) == 0;
        
        return isActive ? extraDamageAbility.value : 0;
    }
    
    public int GetHealingAmount()
    {
        var healingAbility = pathogenData.template.GetAbility(PathogenAbilityType.Regeneration);
        if (healingAbility == null) return 0;
        
        bool isActive = healingAbility.turnInterval > 0 && 
                       (pathogenData.currentTurn % healingAbility.turnInterval) == 0;
        
        return isActive ? healingAbility.value : 0;
    }
    
    
    #endregion
    
    #region Debug Methods
    
    /// <summary>
    /// Debug method to simulate attack pattern for testing
    /// </summary>
    public void DebugAttackPattern(int maxTurns = 10)
    {
        Debug.Log($"=== Attack Pattern for {pathogenData.PathogenName} (Interval: {pathogenData.AttackInterval}) ===");
        
        int originalTurn = pathogenData.currentTurn;
        
        for (int turn = 1; turn <= maxTurns; turn++)
        {
            pathogenData.currentTurn = turn;
            bool wouldAttack = ((turn - 1) % pathogenData.AttackInterval) == 0;
            string status = wouldAttack ? "ATTACK" : "rest";
            Debug.Log($"Turn {turn}: {status}");
        }
        
        // Restore original turn
        pathogenData.currentTurn = originalTurn;
    }
    
    #endregion
}