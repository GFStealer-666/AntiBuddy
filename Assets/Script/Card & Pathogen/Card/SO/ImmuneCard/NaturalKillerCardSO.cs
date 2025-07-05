using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Natural Killer", menuName = "Card/Immune/NaturalKiller")]
public class NaturalKillerCardSO : CardSO
{
    public override void ApplyEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Random damage between 5-20
        int damage = Random.Range(5, 21);
        CardEffects.DealDamage(target, damage);
        
        Debug.Log($"Natural Killer: Instant attack for {damage} damage");
    }
}
