using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Cytotoxic Cell", menuName = "Card/Immune/Cytotoxic")]
public class CytotoxicCellCardSO : ImmuneCardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Cytotoxic T-Cell needs Helper T-Cell to activate
        if (CardEffects.HasCardType<HelperTCellCardSO>(playedCards))
        {
            // Activated by Helper T-Cell - deal 25 damage
            CardEffects.DealDamage(target, 25);
            Debug.Log("Cytotoxic T-Cell activated: 25 damage to pathogen");
        }
        else
        {
            // Without Helper T-Cell - no effect, just takes up field slot
            Debug.Log("Cytotoxic T-Cell: No effect without Helper T-Cell activation");
        }
    }
}
