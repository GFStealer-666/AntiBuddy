using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Helper T Cell", menuName = "Card/Immune/HelperT")]
public class HelperTCellCardSO : ImmuneCardSO
{
    void Awake()
    {
        cardType = ImmuneCardType.Instant; // Helper T activates immediately
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Empty effect - Helper T-Cell just enables combos
        // It does not have a direct effect on its own
    }
}
