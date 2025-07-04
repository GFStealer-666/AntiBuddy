using UnityEngine;
using System.Collections.Generic;
using static PathogenAbilityType;

[CreateAssetMenu(fileName = "HIV", menuName = "Pathogen/HIV")]
public class HIVPathogenSO : PathogenSO
{
    [Header("Card References")]
    public List<CardSO> cardTypeToBlocks;
    
    void Awake()
    {
        pathogenName = "HIV";
        maxHitPoints = 70;
        attackPower = 2;
        attackInterval = 1;
        
        
        abilities = new PathogenAbilityDictionary
        {
            // Mutation ability - every 2 turns
            {
                BlockCards,
                new PathogenAbilityData
                (
                    PathogenAbilityType.BlockCards,
                    2, // Ability Interval
                    0,
                    cardTypeToBlocks
                )
                   
            },
        };
    }
    
    // Remove all behavior methods - they belong in PathogenAbility now
}