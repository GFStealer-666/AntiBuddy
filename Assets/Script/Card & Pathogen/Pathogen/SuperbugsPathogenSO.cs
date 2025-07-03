using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Superbugs", menuName = "Pathogen/Superbugs")]
public class SuperbugsPathogenSO : PathogenSO
{
    [Header("Superbugs Specific")]
    public int mutationInterval = 2; // Mutates every 2 turns
    public int minDamage = 2;
    public int maxDamage = 20;
    
    private int turnsSinceMutation = 0;
    private bool isResistant = false;
    private System.Random random = new System.Random();

    void Awake()
    {
        pathogenName = "Superbugs";
        maxHitPoints = 70;
        attackPower = UnityEngine.Random.Range(minDamage, maxDamage + 1);
    }

    // SuperBugs attacks every 2 turns
    public override bool ShouldAttackThisTurn()
    {
        bool shouldAttack = (currentTurn % 2) == 0; // Attack on even turns (2, 4, 6...)
        if (!shouldAttack)
        {
            Debug.Log($"{pathogenName}: Building resistance this turn (Turn {currentTurn})");
        }
        return shouldAttack;
    }

    public override void OnTurnStart(List<CardSO> playedCards)
    {
        base.OnTurnStart(playedCards); // Call base to increment turn counter
        
        turnsSinceMutation++;
        
        // Mutate every 2 turns
        if (turnsSinceMutation >= mutationInterval)
        {
            Mutate();
            turnsSinceMutation = 0;
        }
        
        // Show drug resistance
        if (isResistant)
        {
            Debug.Log($"{pathogenName}: Showing drug resistance! Some treatments may be less effective.");
        }
    }

    private void Mutate()
    {
        // Random mutation effects
        int mutationType = random.Next(0, 3);
        
        switch (mutationType)
        {
            case 0: // Increase attack power
                attackPower = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                Debug.Log($"{pathogenName}: Mutated! New attack power: {attackPower}");
                break;
                
            case 1: // Gain resistance
                isResistant = true;
                Debug.Log($"{pathogenName}: Mutated! Gained drug resistance!");
                break;
                
            case 2: // Heal slightly
                Heal(5);
                Debug.Log($"{pathogenName}: Mutated! Recovered 5 HP through adaptation.");
                break;
        }
    }

    public override void AttackPlayer(Player player)
    {
        // Random damage between min and max
        int randomDamage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        player.TakeDamage(randomDamage);
        
        Debug.Log($"{pathogenName}: Variable attack! Dealt {randomDamage} damage (range: {minDamage}-{maxDamage})");
        
        if (isResistant)
        {
            Debug.Log($"{pathogenName}: Drug resistance active - harder to treat!");
        }
    }

    public override void TakeDamage(int damage)
    {
        // If resistant, reduce incoming damage
        if (isResistant)
        {
            int reducedDamage = Mathf.Max(1, damage / 2);
            Debug.Log($"{pathogenName}: Drug resistance! Damage reduced from {damage} to {reducedDamage}");
            base.TakeDamage(reducedDamage);
            
            // Resistance fades after being attacked
            isResistant = false;
        }
        else
        {
            base.TakeDamage(damage);
        }
    }
}
