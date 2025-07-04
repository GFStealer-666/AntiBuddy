using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Natural Killer", menuName = "Card/Immune/NaturalKiller")]
public class NaturalKillerCardSO : ImmuneCardSO
{
    void Awake()
    {
        cardType = ImmuneCardType.Instant; // Natural Killer activates immediately
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Natural Killer cells instantly attack for random 5-20 HP
        int damage = UnityEngine.Random.Range(5, 21);
        CardEffects.DealDamage(target, damage);
        
        Debug.Log($"Natural Killer: Instant attack for {damage} damage");
    }
}
