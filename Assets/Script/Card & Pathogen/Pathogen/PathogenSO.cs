using UnityEngine;
using System.Collections.Generic;

public abstract class PathogenSO : ScriptableObject
{
    [Header("Basic Stats")]
    public string pathogenName;
    public int maxHitPoints;
    public int attackPower;
    public int tokenDropOnDeath = 1; 
    public Sprite pathogenSprite;
    
    [Header("Attack Pattern")]
    public int attackInterval = 1; // How often pathogen attacks
    
    [Header("Special Abilities")]
    public PathogenAbilityDictionary abilities = new PathogenAbilityDictionary();
    
    [Header("Description")]
    [TextArea(3, 5)]
    public string description;
    
    // Helper methods
    public bool HasAbility(PathogenAbilityType abilityType)
    {
        return abilities.ContainsKey(abilityType);
    }
    
    public PathogenAbilityData GetAbility(PathogenAbilityType abilityType)
    {
        return abilities.TryGetValue(abilityType, out PathogenAbilityData data) ? data : null;
    }
    
    public int GetAbilityInterval(PathogenAbilityType abilityType)
    {
        var ability = GetAbility(abilityType);
        return ability?.turnInterval ?? 0;
    }
    
    public int GetAbilityValue(PathogenAbilityType abilityType)
    {
        var ability = GetAbility(abilityType);
        return ability?.value ?? 0;
    }
}