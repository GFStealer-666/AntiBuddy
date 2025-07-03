using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HIV", menuName = "Pathogen/HIV")]
public class HIVPathogenSO : PathogenSO
{
    [Header("HIV Specific")]
    public int helperTCellDamage = 2;
    private bool hasAttackedHelperT = false;

    void Awake()
    {
        pathogenName = "HIV";
        maxHitPoints = 70;
        attackPower = 2;
    }

    // HIV blocks Helper T-Cell cards from being played
    public override bool IsCardBlocked(System.Type cardType)
    {
        if (cardType == typeof(HelperTCellCardSO))
        {
            Debug.Log($"{pathogenName}: Blocking Helper T-Cell card - immune system suppression!");
            return true;
        }
        return false;
    }

    public override void OnTurnStart(List<CardSO> playedCards)
    {
        base.OnTurnStart(playedCards); // Call base to increment turn counter
        
        // HIV specifically targets Helper T-Cells
        foreach (var card in playedCards)
        {
            if (card is HelperTCellCardSO)
            {
                hasAttackedHelperT = true;
                Debug.Log($"{pathogenName}: Targeting Helper T-Cell! This will weaken immune response.");
                break;
            }
        }
    }

    public override void AttackPlayer(Player player)
    {
        // Check if Helper T-Cell was played this turn
        var playedCards = player.PlayedCards;
        bool helperTUsed = false;
        
        foreach (var card in playedCards)
        {
            if (card is HelperTCellCardSO)
            {
                helperTUsed = true;
                break;
            }
        }

        if (helperTUsed)
        {
            // HIV attacks Helper T-Cells specifically, weakening immune system
            Debug.Log($"{pathogenName}: Attacking Helper T-Cell! Immune system compromised.");
            
            // Remove Helper T-Cell benefits or reduce their effectiveness
            // This could reduce future combo effectiveness
            player.TakeDamage(helperTCellDamage);
            
            // HIV also damages the overall immune system
            base.AttackPlayer(player);
        }
        else
        {
            // Normal attack when no Helper T-Cell present
            base.AttackPlayer(player);
        }
    }

    // HIV has persistent effects
    public override void TakeDamage(int damage)
    {
        // HIV is harder to eliminate completely
        base.TakeDamage(damage);
        
        if (maxHitPoints <= 0)
        {
            Debug.Log($"{pathogenName}: Difficult to eliminate completely. May return if immune system is weakened.");
        }
    }
}
