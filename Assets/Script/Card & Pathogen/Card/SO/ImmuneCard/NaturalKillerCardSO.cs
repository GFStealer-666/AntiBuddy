using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Natural Killer", menuName = "Card/Immune/NaturalKiller")]
public class NaturalKillerCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Random damage between 5-20
        int baseDamage = Random.Range(5, 21);
        
        // Use vaccine boost compatible method
        CardEffects.DealDamageWithBoost(player, target, baseDamage);
        
        // Calculate final damage for logging (check if boost was active)
        int finalDamage = player.IsVaccineBoostActive() ? baseDamage * 2 : baseDamage;
        
        Debug.Log($"Natural Killer: Instant attack for {finalDamage} damage{(player.IsVaccineBoostActive() ? " (BOOSTED!)" : "")}");
        
        // Log via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogCardEffect("Natural Killer", $"instant attack for {finalDamage} damage{(player.IsVaccineBoostActive() ? " (boosted)" : "")}");
            if (target != null)
                gameManager.LogDamage("Player (Natural Killer)", target.GetPathogenName(), finalDamage);
        }
    }
}
