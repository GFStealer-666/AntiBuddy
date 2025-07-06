using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Wash Hands Item", menuName = "Cards/Items/Wash Hands")]
public class WashHandsItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Wash Hands: 80% defense vs COVID/Influenza, 20% vs others (cost 2 tokens or 5 HP)
        Debug.Log("Using Wash Hands - Applying pathogen-specific defense");
        
        float defensePercentage = 20f; // Default 20% for most pathogens
        
        if (target != null)
        {
            // Check if target is COVID or Influenza pathogen
            string pathogenName = target.GetPathogenName().ToLower();
            if (pathogenName.Contains("covid") || pathogenName.Contains("influenza") || pathogenName.Contains("flu"))
            {
                defensePercentage = 80f; // 80% defense against enveloped viruses
                Debug.Log($"Wash Hands: Effective against {target.GetPathogenName()} - 80% defense applied");
            }
            else
            {
                Debug.Log($"Wash Hands: Less effective against {target.GetPathogenName()} - 20% defense applied");
            }
        }
        else
        {
            Debug.Log("Wash Hands: No specific target - applying 20% general defense");
        }
        
        // Apply percentage defense until next turn
        CardEffects.AddPercentageDefense(player, (int)defensePercentage);
        
        Debug.Log($"Wash Hands: Applied {defensePercentage}% defense until next turn");
    }
}
