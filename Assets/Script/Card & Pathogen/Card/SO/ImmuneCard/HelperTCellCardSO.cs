using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Helper T Cell", menuName = "Card/Immune/HelperT")]
public class HelperTCellCardSO : ImmuneCardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Helper T-Cell activates B-Cells and Cytotoxic T-Cells
        // On its own, it just takes up a field slot for activation
        Debug.Log("Helper T-Cell: Activated for B-Cell and Cytotoxic T-Cell combos");
    }
}
