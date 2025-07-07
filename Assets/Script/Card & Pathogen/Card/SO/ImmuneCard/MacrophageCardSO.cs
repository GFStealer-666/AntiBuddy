using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Macrophage", menuName = "Card/Immune/Macrophage")]
public class MacrophageCardSO : CardSO
{
    [Header("Macrophage Settings")]
    public CardSO helperTCellReward;

    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        Debug.Log($"Macrophage: Starting effect application. Current hand size: {player.PlayerCards.Hand.Count}");
        
        // Get GameManager for logging
        var gameManager = FindFirstObjectByType<GameManager>();
        
        // Simple, direct effects - no complex activation logic (vaccine boost compatible)
        CardEffects.AddPercentageDefenseWithBoost(player, 25);
        int finalDefense = player.IsVaccineBoostActive() ? 50 : 25;
        if (gameManager != null)
            gameManager.LogCardEffect("Macrophage", $"gained {finalDefense}% defense{(player.IsVaccineBoostActive() ? " (boosted)" : "")}");
        
        CardEffects.DealDamageWithBoost(player, target, 5);
        int finalDamage = player.IsVaccineBoostActive() ? 10 : 5;
        if (gameManager != null && target != null)
            gameManager.LogDamage("Player (Macrophage)", target.GetPathogenName(), finalDamage);
        
        // Add Helper T-Cell to hand (cards added to hand are not doubled by vaccine)
        if (helperTCellReward != null)
        {
            Debug.Log($"Macrophage: Adding {helperTCellReward.cardName} to hand");
            CardEffects.AddSpecificCardToHand(player, helperTCellReward);
            Debug.Log($"Macrophage: After adding card, hand size: {player.PlayerCards.Hand.Count}");
        }
        else
        {
            Debug.LogWarning("Macrophage: helperTCellReward is null!");
        }
        
        int finalDefenseForLog = player.IsVaccineBoostActive() ? 50 : 25;
        int finalDamageForLog = player.IsVaccineBoostActive() ? 10 : 5;
        Debug.Log($"Macrophage: {finalDefenseForLog}% defense, {finalDamageForLog} damage, +1 Helper T-Cell to hand{(player.IsVaccineBoostActive() ? " (BOOSTED!)" : "")}");
    }
}
