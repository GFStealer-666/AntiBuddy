using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Card", menuName = "Card/Defense")]
public class DefenseCardSO : CardSO
{
    public override void Use(Player player, PathogenSO target)
    {
        // You can later use a status effect system instead
        Debug.Log($"{cardName} used: player gets temporary defense effect.");
        // e.g. player.SetTemporaryDefense(50);
    }
}
