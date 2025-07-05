using UnityEngine;
using System.Collections.Generic;

// Simplified CardSO - no more complex inheritance!
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    [Header("Basic Card Info")]
    public string cardName;
    public Sprite frontCardImage;
    public Sprite backCardImage;
    [TextArea(3, 5)]
    public string description;
    
    // Simple, concrete implementation instead of abstract
    public virtual void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // For now, just log - we'll add data-driven effects later
        Debug.Log($"{cardName} effect applied!");
    }
}
