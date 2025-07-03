using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "B-Cell", menuName = "Card/Immune/B-Cell")]
public class BCellCardSO : ImmuneCardSO
{
    void Awake()
    {
        cardType = ImmuneCardType.Combo; // B-Cell needs Helper T to activate
    }

    protected override bool CanActivateCombo(List<CardSO> cardsInField)
    {
        // B-Cell needs Helper T-Cell to activate
        return CardEffects.HasCardType<HelperTCellCardSO>(cardsInField);
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Create antibody response - 50% damage reduction this turn
        CardEffects.AddPercentageDefense(player, 50);
        Debug.Log("B-Cell activated by Helper T-Cell: Antibody response! 50% damage reduction");
    }
}