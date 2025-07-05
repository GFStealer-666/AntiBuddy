using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Macrophage", menuName = "Card/Immune/Macrophage")]
public class MacrophageCardSO : CardSO
{
    [Header("Macrophage Settings")]
    public CardSO helperTCellReward;

    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Simple, direct effects - no complex activation logic
        CardEffects.AddPercentageDefense(player, 25);
        CardEffects.DealDamage(target, 5);
        
        // Add Helper T-Cell to hand
        if (helperTCellReward != null)
        {
            CardEffects.AddSpecificCardToHand(player, helperTCellReward);
        }
        
        Debug.Log("Macrophage: 25% defense, 5 damage, +1 Helper T-Cell to hand");
    }
}
