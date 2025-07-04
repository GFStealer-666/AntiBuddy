using UnityEngine;
using static PathogenAbilityType;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dengue", menuName = "Pathogen/Dengue")]
public class DenguePathogenSO : PathogenSO
{
    [Header("Card References")]
    public List<CardSO> cardTypeToBlocks;
    void Awake()
    {
        pathogenName = "Dengue";
        maxHitPoints = 50;
        attackPower = 10;
        attackInterval = 2; // Attack every 2 turns (alternating: attack -> stop -> attack -> stop)

        abilities = new PathogenAbilityDictionary
        {
            {
                BlockCards,
                new PathogenAbilityData
                (
                    PathogenAbilityType.BlockCards,
                    1, // Ability Interval
                    0, // No specific value for this ability
                    cardTypeToBlocks
                )

            }

        };
    }
}