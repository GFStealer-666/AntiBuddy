using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Wear Mask Item", menuName = "Cards/Items/Wear Mask")]
public class WearMaskItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Wear a Mask: 50% defense until next player turn (cost 2 tokens or 5 HP)
        Debug.Log("Using Wear a Mask - Applying 50% defense until next turn");
        
        CardEffects.AddPercentageDefense(player, 50);
        
        Debug.Log("Wear a Mask: Player now has 50% defense until next turn!");
    }
}
