using UnityEngine;
using System.Collections.Generic;
public abstract class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite cardIcon;
    public string description;
    public abstract void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target);
}
