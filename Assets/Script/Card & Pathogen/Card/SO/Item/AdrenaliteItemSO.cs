using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Adrenaline Item", menuName = "Cards/Items/Adrenaline")]
public class AdrenaliteItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Adrenaline: Grants 5 tokens when used (purchased with 10 HP)
        Debug.Log("Using Adrenaline - Granting 5 tokens");
        
        CardEffects.AddTokens(player, 5);
        
        Debug.Log($"Adrenaline used - gained 5 tokens");
        
        // Log via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.LogCardEffect("Adrenaline", "gained 5 tokens");
    }
}
