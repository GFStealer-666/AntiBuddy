using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Vaccine Item", menuName = "Cards/Items/Vaccine")]
public class VaccineItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Vaccine: Doubles the effect of the next card played
        Debug.Log("Using Vaccine - Next card effect will be doubled!");
        
        player.ApplyVaccineBoost();
        
        // Log the effect via GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.LogCardEffect("Vaccine", "next card effect will be doubled");
        
        Debug.Log("Vaccine: Ready to boost next card effect!");
    }
}
