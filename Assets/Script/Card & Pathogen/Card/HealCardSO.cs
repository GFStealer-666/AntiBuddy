using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Card", menuName = "Card/Heal")]
public class HealCardSO : CardSO
{
    public override void Use(Player player, PathogenSO target)
    {
        player.Heal(power);
        Debug.Log($"{cardName} healed {power} HP.");
    }
}
