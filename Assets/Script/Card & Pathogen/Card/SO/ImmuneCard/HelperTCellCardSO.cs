using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Helper T Cell", menuName = "Card/Immune/HelperT")]
public class HelperTCellCardSO : ImmuneCardSO
{
    void Awake()
    {
        cardType = ImmuneCardType.Instant; // Helper T activates immediately
    }

    protected override void DoCardEffect(Player player, List<CardSO> playedCards, PathogenSO target)
    {
        // Helper T-Cell provides small direct benefit and enables other cards
        CardEffects.AddTokens(player, 1);
        Debug.Log("Helper T-Cell: +1 token and enables B-Cell/Cytotoxic combos");
    }
}
