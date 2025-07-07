using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "B-Cell", menuName = "Card/Immune/B-Cell")]
public class BCellCardSO : CardSO
{
    [System.NonSerialized]
    private bool hasBeenActivated = false;
    
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Prevent double activation
        if (hasBeenActivated)
        {
            Debug.Log("B-Cell: Already activated this turn, skipping");
            return;
        }
        
        // Get GameManager for logging
        var gameManager = FindFirstObjectByType<GameManager>();
        
        // Check if Helper T-Cell is in play
        if (!HasHelperTCellInPlay(playedCards))
        {
            Debug.Log("B-Cell: Needs Helper T-Cell to activate - waiting for Helper T-Cell");
            if (gameManager != null)
                gameManager.LogCardEffect("B-Cell", "needs Helper T-Cell to activate");
            return; // Don't apply effect yet - will be activated by Helper T-Cell
        }
        
        // Mark as activated to prevent double-activation
        hasBeenActivated = true;
        
        // Create antibody response - 50% damage reduction (vaccine boost compatible)
        CardEffects.AddPercentageDefenseWithBoost(player, 50);
        
        int finalDefense = player.IsVaccineBoostActive() ? 100 : 50;
        Debug.Log($"B-Cell activated by Helper T-Cell: Antibody response! {finalDefense}% damage reduction{(player.IsVaccineBoostActive() ? " (BOOSTED!)" : "")}");
        
        if (gameManager != null)
            gameManager.LogCardEffect("B-Cell", $"activated by Helper T-Cell - {finalDefense}% damage reduction{(player.IsVaccineBoostActive() ? " (boosted)" : "")}");
    }
    
    public void ForceActivate(Player player, Pathogen target)
    {
        if (hasBeenActivated) return;
        
        hasBeenActivated = true;
        CardEffects.AddPercentageDefense(player, 50);
        Debug.Log("B-Cell: Force activated by Helper T-Cell!");
    }
    
    public void ResetActivation()
    {
        hasBeenActivated = false;
    }
    
    private bool HasHelperTCellInPlay(List<CardSO> playedCards)
    {
        // Check all played cards in this turn for Helper T-Cell
        foreach (var card in playedCards)
        {
            if (card is HelperTCellCardSO)
                return true;
        }
        return false;
    }
}