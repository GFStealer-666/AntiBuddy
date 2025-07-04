using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Cytotoxic Cell", menuName = "Card/Immune/Cytotoxic")]
public class CytotoxicCellCardSO : ImmuneCardSO
{
    void Awake()
    {
        cardType = ImmuneCardType.Combo; // This card requires combo activation
    }

    protected override bool CanActivateCombo(List<CardSO> cardsInField)
    {
        // Cytotoxic T-Cell needs Helper T-Cell to activate
        return CardEffects.HasCardType<HelperTCellCardSO>(cardsInField);
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, Pathogen target)
    {
        // Effect only runs if CanActivateCombo returned true
        CardEffects.DealDamage(target, 25);
        Debug.Log("Cytotoxic T-Cell activated by Helper T-Cell: 25 damage to pathogen");
    }
}
