using UnityEngine;

[CreateAssetMenu(fileName = "Influenza", menuName = "Pathogen/Influenza")]
public class InfluenzaPathogenSO : PathogenSO
{
    [Header("Influenza Specific")]
    public int multiplicationRate = 4; // HP gained per turn
    private int currentMultiplication = 0;

    void Awake()
    {
        pathogenName = "Influenza";
        maxHitPoints = 50;
        attackPower = 4;
    }

    public override void OnTurnStart(System.Collections.Generic.List<CardSO> playedCards)
    {
        base.OnTurnStart(playedCards); // Call base to increment turn counter
        
        // Influenza multiplies each turn (gains HP)
        currentMultiplication += multiplicationRate;
        maxHitPoints += multiplicationRate;
        
        Debug.Log($"{pathogenName}: Multiplying! Gained {multiplicationRate} HP. Current HP: {maxHitPoints}");
    }

    public override void AttackPlayer(Player player)
    {
        // Base attack
        base.AttackPlayer(player);
        
        Debug.Log($"{pathogenName}: Spreading rapidly! Total multiplication: +{currentMultiplication} HP");
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        // Influenza can be contained but multiplies if not dealt with quickly
        if (maxHitPoints > 0)
        {
            Debug.Log($"{pathogenName}: Still spreading! Will multiply next turn.");
        }
    }
}
