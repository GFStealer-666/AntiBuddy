using UnityEngine;
using static PathogenAbilityType;

[CreateAssetMenu(fileName = "Superbugs", menuName = "Pathogen/Superbugs")]
public class SuperbugsPathogenSO : PathogenSO
{

    
    void Awake()
    {
        pathogenName = "Superbugs";
        maxHitPoints = 70;
        attackPower = 20;
        attackInterval = 2; 
        
        abilities = new PathogenAbilityDictionary
        {
            // Mutation ability - every 2 turns
            {
                Mutation,
                new PathogenAbilityData
                (
                    PathogenAbilityType.Mutation,
                    1, // Ability Interval
                    5 // Value of Mutation ability (healing amount when mutating)
                )
            },
            
        };
    }
}