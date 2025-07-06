using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Cytokine Item", menuName = "Cards/Items/Cytokine")]
public class CytokineItemSO : ItemSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Cytokine: Spawns Helper T Cell on the field (cost 5 tokens or 10 HP)
        Debug.Log("Using Cytokine - Spawning Helper T Cell");
        
        // Find and play a Helper T Cell card to the field
        var turnManager = FindFirstObjectByType<TurnManager>();
        var cardField = FindFirstObjectByType<CardField>();
        
        if (turnManager != null && cardField != null)
        {
            // Create a temporary Helper T Cell for the field
            var helperTCell = ScriptableObject.CreateInstance<HelperTCellCardSO>();
            helperTCell.cardName = "Helper T Cell (from Cytokine)";
            
            // Add directly to field
            if (cardField.TryPlayCardToField(helperTCell))
            {
                Debug.Log("Cytokine: Successfully spawned Helper T Cell on field");
            }
            else
            {
                Debug.LogWarning("Cytokine: Could not add Helper T Cell to field (field full?)");
            }
        }
        else
        {
            Debug.LogError("Cytokine: Could not find TurnManager or CardField");
        }
    }
}
