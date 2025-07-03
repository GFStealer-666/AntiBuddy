using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dengue", menuName = "Pathogen/Dengue")]
public class DenguePathogenSO : PathogenSO
{
    [Header("Dengue Specific")]
    public int adeWeakeningAmount = 10; // Amount to weaken player
    public int healOnAttack = 10; // HP gained when attacking

    void Awake()
    {
        pathogenName = "Dengue";
        maxHitPoints = 50;
        attackPower = 10;
    }

    // Dengue blocks Macrophage cards from being played
    public override bool IsCardBlocked(System.Type cardType)
    {
        if (cardType == typeof(MacrophageCardSO))
        {
            Debug.Log($"{pathogenName}: Blocking Macrophage card - ADE prevention!");
            return true;
        }
        return false;
    }

    // Dengue attacks every other turn (1 turn attack, 1 turn rest)
    public override bool ShouldAttackThisTurn()
    {
        bool shouldAttack = (currentTurn % 2) == 1; // Attack on odd turns (1, 3, 5...)
        if (!shouldAttack)
        {
            Debug.Log($"{pathogenName}: Resting this turn (Turn {currentTurn})");
        }
        return shouldAttack;
    }

    public override void OnTurnStart(List<CardSO> playedCards)
    {
        base.OnTurnStart(playedCards); // Call base to increment turn counter
        
        // Check if Macrophage was played - causes ADE (Antibody-Dependent Enhancement)
        foreach (var card in playedCards)
        {
            if (card is MacrophageCardSO)
            {
                Debug.Log($"{pathogenName}: ADE triggered by Macrophage! Player becomes weaker.");
                // This will be handled in AttackPlayer method
                break;
            }
        }
    }

    public override void AttackPlayer(Player player)
    {
        // Normal attack
        base.AttackPlayer(player);
        
        // Dengue heals itself when attacking (represents viral replication)
        Heal(healOnAttack);
        
        // Check if player used Macrophage recently (ADE effect)
        var playedCards = player.PlayedCards;
        bool macrophageUsed = false;
        foreach (var card in playedCards)
        {
            if (card is MacrophageCardSO)
            {
                macrophageUsed = true;
                break;
            }
        }

        if (macrophageUsed)
        {
            // ADE: Make player more vulnerable (reduce defense or deal extra damage)
            player.TakeDamage(adeWeakeningAmount);
            Debug.Log($"{pathogenName}: ADE effect! Additional {adeWeakeningAmount} damage due to Macrophage interference.");
        }
    }
}
