using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Cytotoxic Cell", menuName = "Card/Immune/Cytotoxic")]
public class CytotoxicCellCardSO : CardSO
{
    [System.NonSerialized]
    private bool hasBeenActivated = false;
    
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Prevent double activation
        if (hasBeenActivated)
        {
            Debug.Log("Cytotoxic T-Cell: Already activated this turn, skipping");
            return;
        }
        
        // Get GameManager for logging
        var gameManager = FindFirstObjectByType<GameManager>();
        
        // Check if Helper T-Cell is in play
        if (!HasHelperTCellInPlay(playedCards))
        {
            Debug.Log("Cytotoxic T-Cell: Needs Helper T-Cell to activate - waiting for Helper T-Cell");
            if (gameManager != null)
                gameManager.LogCardEffect("Cytotoxic T-Cell", "needs Helper T-Cell to activate");
            return; // Don't apply effect yet - will be activated by Helper T-Cell
        }
        
        // Mark as activated to prevent double-activation
        hasBeenActivated = true;
        
        // Deal damage to pathogen (vaccine boost compatible)
        CardEffects.DealDamageWithBoost(player, target, 25);
        
        int finalDamage = player.IsVaccineBoostActive() ? 50 : 25;
        Debug.Log($"Cytotoxic T-Cell activated by Helper T-Cell: {finalDamage} damage to pathogen{(player.IsVaccineBoostActive() ? " (BOOSTED!)" : "")}");
        
        if (gameManager != null)
        {
            gameManager.LogCardEffect("Cytotoxic T-Cell", $"activated by Helper T-Cell - {finalDamage} damage{(player.IsVaccineBoostActive() ? " (boosted)" : "")}");
            if (target != null)
                gameManager.LogDamage("Player (Cytotoxic T-Cell)", target.GetPathogenName(), finalDamage);
        }
    }
    
    public void ForceActivate(Player player, Pathogen target)
    {
        if (hasBeenActivated) return;
        
        hasBeenActivated = true;
        CardEffects.DealDamage(target, 25);
        Debug.Log("Cytotoxic T-Cell: Force activated by Helper T-Cell!");
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
