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
        
        // Simple, direct effects - no complex activation logic
        CardEffects.AddPercentageDefense(player, 25);
        if (gameManager != null)
            gameManager.LogCardEffect("Macrophage", "gained 25% defense");
        
        CardEffects.DealDamage(target, 5);
        if (gameManager != null && target != null)
            gameManager.LogDamage("Player (Macrophage)", target.GetPathogenName(), 5);
        
        // Add Helper T-Cell to hand
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
        
        Debug.Log("Macrophage: 25% defense, 5 damage, +1 Helper T-Cell to hand");
    }
}
