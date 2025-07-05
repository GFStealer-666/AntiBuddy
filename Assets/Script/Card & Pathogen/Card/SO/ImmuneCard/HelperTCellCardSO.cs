using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Helper T Cell", menuName = "Card/Immune/HelperT")]
public class HelperTCellCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Helper T-Cell just enables other cards - no direct effect
        Debug.Log("Helper T-Cell: Enabling combo cards");
    }
}
