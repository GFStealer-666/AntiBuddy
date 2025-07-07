using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Helper T Cell", menuName = "Card/Immune/HelperT")]
public class HelperTCellCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Helper T-Cell enables other cards - check for combo activation immediately
        Debug.Log("Helper T-Cell: Enabling combo cards and activating any waiting combos");
        
        // Immediately activate any combo cards that were played earlier this turn
        ActivateWaitingCombos(player, playedCards, target);
        
        // Log the effect via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.LogCardEffect("Helper T-Cell", "enabled combo cards");
    }
    
    private void ActivateWaitingCombos(Player player, List<CardSO> playedCards, Pathogen target)
    {
        Debug.Log("Helper T-Cell: Checking for combo cards to activate...");
        
        foreach (var card in playedCards)
        {
            // Activate B-Cell and Cytotoxic cards that were played before Helper T-Cell
            if (card is BCellCardSO bCell)
            {
                Debug.Log("Helper T-Cell: Activating B-Cell combo!");
                bCell.ForceActivate(player, target);
            }
            else if (card is CytotoxicCellCardSO cytotoxic)
            {
                Debug.Log("Helper T-Cell: Activating Cytotoxic T-Cell combo!");
                cytotoxic.ForceActivate(player, target);
            }
        }
    }
}
