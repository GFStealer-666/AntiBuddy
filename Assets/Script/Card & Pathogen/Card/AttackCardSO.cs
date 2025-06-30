using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Card/Attack")]
public class AttackCardSO : CardSO
{
    public override void Use(Player player, PathogenSO target)
    {
        if (target != null)
        {
            target.TakeDamage(power);
            Debug.Log($"{cardName} dealt {power} damage to {target.pathogenName}");
        }
    }
}