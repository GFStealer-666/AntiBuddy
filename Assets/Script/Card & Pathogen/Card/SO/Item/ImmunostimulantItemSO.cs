using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Immunostimulant Item", menuName = "Cards/Items/Immunostimulant")]
public class ImmunostimulantItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Immunostimulant: 100% defense until next player turn (cost 5 HP)
        Debug.Log("Using Immunostimulant - Granting 100% defense until next turn");
        
        // Add 100% percentage defense (this will block all damage until next turn)
        CardEffects.AddPercentageDefense(player, 100);
        
        Debug.Log("Immunostimulant: Player now has 100% defense until next turn!");
    }
}
