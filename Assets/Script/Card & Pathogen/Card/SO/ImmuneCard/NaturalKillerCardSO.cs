using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Natural Killer", menuName = "Card/Immune/NaturalKiller")]
public class NaturalKillerCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Random damage between 5-20
        int damage = Random.Range(5, 21);
        CardEffects.DealDamage(target, damage);
        
        Debug.Log($"Natural Killer: Instant attack for {damage} damage");
        
        // Log via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LogCardEffect("Natural Killer", $"instant attack for {damage} damage");
            if (target != null)
                gameManager.LogDamage("Player (Natural Killer)", target.GetPathogenName(), damage);
        }
    }
}
