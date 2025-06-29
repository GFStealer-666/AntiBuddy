using UnityEngine;

public abstract class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite cardIcon;
    public string description;

    // Allow setting card cost if needed in future
    public int power;
    public int cost;

    public abstract void Use(Player player, PathogenSO target);
}
