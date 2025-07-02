using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "B-Cell", menuName = "Card/Immune/B-Cell")]
public class BCellCardSO : ImmuneCardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // B-Cell creates antibodies when Helper T-Cell is present
        if (CardEffects.HasCardType<HelperTCellCardSO>(playedCards))
        {
            // Create antibody response - 50% damage reduction this turn
            CardEffects.AddPercentageDefense(player, 50);
            Debug.Log("B-Cell + Helper T-Cell: Antibody response! 50% damage reduction this turn");
        }
        else
        {
            // B-Cell alone has no effect - needs Helper T-Cell activation
            Debug.Log("B-Cell: No effect without Helper T-Cell");
        }
    }
}