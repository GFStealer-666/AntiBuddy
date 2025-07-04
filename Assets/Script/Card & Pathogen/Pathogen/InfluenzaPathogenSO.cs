using UnityEngine;

[CreateAssetMenu(fileName = "Influenza", menuName = "Pathogen/Influenza")]
public class InfluenzaPathogenSO : PathogenSO
{
    void Awake()
    {
        pathogenName = "Influenza";
        maxHitPoints = 50;
        attackPower = 4;
        attackInterval = 1;
        
    }
    
    // Remove all behavior methods - they belong in PathogenAbility now
}