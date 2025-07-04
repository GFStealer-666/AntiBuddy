using UnityEngine;
using System.Collections.Generic;
public abstract class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite frontCardImage;
    public Sprite backCardImage;
    public string description;
    public abstract void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target);
}
