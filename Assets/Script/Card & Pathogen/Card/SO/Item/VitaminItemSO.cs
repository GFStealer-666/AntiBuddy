using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Vitamin Item", menuName = "Cards/Items/Vitamin")]
public class VitaminItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Vitamin: Heal 20 HP (cost was paid during purchase)
        Debug.Log("Using Vitamin - Heal 20 HP");
        
        CardEffects.HealPlayer(player, 20);
        Debug.Log($"Vitamin used successfully - healed 20 HP");
    }
}
