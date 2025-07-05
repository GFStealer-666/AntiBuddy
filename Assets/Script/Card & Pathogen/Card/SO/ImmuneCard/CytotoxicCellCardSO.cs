using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Cytotoxic Cell", menuName = "Card/Immune/Cytotoxic")]
public class CytotoxicCellCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Check if Helper T-Cell is in play
        if (!HasHelperTCellInPlay(playedCards))
        {
            Debug.Log("Cytotoxic T-Cell needs Helper T-Cell to activate");
            return;
        }
        
        // Deal damage to pathogen
        CardEffects.DealDamage(target, 25);
        Debug.Log("Cytotoxic T-Cell activated by Helper T-Cell: 25 damage to pathogen");
    }
    
    private bool HasHelperTCellInPlay(List<CardSO> playedCards)
    {
        foreach (var card in playedCards)
        {
            if (card is HelperTCellCardSO)
                return true;
        }
        return false;
    }
}
