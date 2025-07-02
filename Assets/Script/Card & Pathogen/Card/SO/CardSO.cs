using UnityEngine;
using System.Collections.Generic;
public abstract class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite cardIcon;
    public string description;

    // Allow setting card cost if needed in future
    public int power;

    public abstract void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target);
}

public class CardEffectContext
{
    public Player player;
    public PathogenSO target;
    public List<CardSO> cardsThisTurn;
    public GameManager game; 
}
