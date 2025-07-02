using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "Natural Killer", menuName = "Card/Immune/NaturalKiller")]
public class NaturalKillerCardSO : ImmuneCardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Natural Killer cells instantly attack for random 5-20 HP
        int damage = UnityEngine.Random.Range(5, 21);
        CardEffects.DealDamage(target, damage);
        
        Debug.Log($"Natural Killer: Instant attack for {damage} damage");
    }
}
