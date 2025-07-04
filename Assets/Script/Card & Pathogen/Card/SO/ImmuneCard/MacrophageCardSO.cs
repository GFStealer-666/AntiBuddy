using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Macrophage", menuName = "Card/Immune/Macrophage")]
public class MacrophageCardSO : ImmuneCardSO
{
    [Header("Card Rewards")]
    public HelperTCellCardSO helperTCellReward;

    void Awake()
    {
        cardType = ImmuneCardType.Instant; // Macrophages activate immediately
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Macrophage provides 25% defense, 5 damage, and gives 1 Helper T-Cell
        CardEffects.AddPercentageDefense(player, 25);
        CardEffects.DealDamage(target, 5);
        
        // Add Helper T-Cell to hand
        if (helperTCellReward != null)
        {
            CardEffects.AddSpecificCardToHand(player, helperTCellReward);
            Debug.Log("Macrophage: 25% defense, 5 damage, +1 Helper T-Cell to hand");
        }
        else
        {
            Debug.Log("Macrophage: 25% defense, 5 damage (no Helper T-Cell reward configured)");
        }
    }
}
