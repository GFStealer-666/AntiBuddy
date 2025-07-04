// filepath: d:\Immunity-Boardgame\Assets\Script\Core Manager\Pathogen\PathogenData.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PathogenData
{
    [Header("Template Reference")]
    public PathogenSO template;
    
    [Header("Runtime State Only")]
    public int currentHitPoints;
    public int currentTurn;
    public bool isAlive;
    public bool canAttackThisTurn;
    
    public PathogenData(PathogenSO pathogenTemplate)
    {
        template = pathogenTemplate;
        currentHitPoints = pathogenTemplate.maxHitPoints;
        currentTurn = 0;
        isAlive = true;
        canAttackThisTurn = true;
    }
    
    // Access template data through properties
    public string PathogenName => template.pathogenName;
    public int MaxHitPoints => template.maxHitPoints;
    public int AttackPower => template.attackPower;
    public Sprite PathogenSprite => template.pathogenSprite;
    public int AttackInterval => template.attackInterval;
    public PathogenAbilityDictionary Abilities => template.abilities;
    public string Description => template.description;
    
    public float GetHealthPercentage()
    {
        return MaxHitPoints > 0 ? (float)currentHitPoints / MaxHitPoints : 0f;
    }
    
    public bool IsDead()
    {
        return currentHitPoints <= 0;
    }
}

public enum PathogenAbilityType
{
    None,
    BlockCards,      // Blocks certain card types 
    ExtraDamage,     // Does extra damage periodically
    Regeneration,   // Heals over time
    Mutation    
}