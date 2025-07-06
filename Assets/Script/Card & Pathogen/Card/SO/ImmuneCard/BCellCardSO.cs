using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "B-Cell", menuName = "Card/Immune/B-Cell")]
public class BCellCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Get GameManager for logging
        var gameManager = FindFirstObjectByType<GameManager>();
        
        // Check if Helper T-Cell is in play
        if (!HasHelperTCellInPlay(playedCards))
        {
            Debug.Log("B-Cell needs Helper T-Cell to activate");
            if (gameManager != null)
                gameManager.LogCardEffect("B-Cell", "needs Helper T-Cell to activate");
            return;
        }
        
        // Create antibody response - 50% damage reduction
        CardEffects.AddPercentageDefense(player, 50);
        Debug.Log("B-Cell activated by Helper T-Cell: Antibody response! 50% damage reduction");
        
        if (gameManager != null)
            gameManager.LogCardEffect("B-Cell", "activated by Helper T-Cell - 50% damage reduction");
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