using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Covid19", menuName = "Pathogen/Covid19")]
public class Covid19PathogenSO : PathogenSO
{
    [Header("Covid-19 Specific")]
    public int cytotoxicAttackDamage = 8;
    public int cytotoxicAttackDelay = 2; // turns
    private int turnsSinceCytotoxicPlayed = 0;
    private bool cytotoxicWasPlayedAlone = false;

    void Awake()
    {
        pathogenName = "Covid-19";
        maxHitPoints = 50;
        attackPower = 8;
    }

    // Covid-19 blocks Cytotoxic T-Cell cards from being played
    public override bool IsCardBlocked(System.Type cardType)
    {
        if (cardType == typeof(CytotoxicCellCardSO))
        {
            Debug.Log($"{pathogenName}: Blocking Cytotoxic T-Cell card - viral interference!");
            return true;
        }
        return false;
    }

    public override void OnTurnStart(List<CardSO> playedCards)
    {
        base.OnTurnStart(playedCards); // Call base to increment turn counter

        // Check if Cytotoxic T-Cell was played alone this turn
        if (playedCards.Count == 1 && playedCards[0] is CytotoxicCellCardSO)
        {
            cytotoxicWasPlayedAlone = true;
            turnsSinceCytotoxicPlayed = 0;
            Debug.Log($"{pathogenName}: Detected lone Cytotoxic T-Cell! Will counter-attack in {cytotoxicAttackDelay} turns.");
        }

        // Count down turns if cytotoxic was played alone
        if (cytotoxicWasPlayedAlone)
        {
            turnsSinceCytotoxicPlayed++;
            if (turnsSinceCytotoxicPlayed >= cytotoxicAttackDelay)
            {
                // Reset and prepare for attack
                cytotoxicWasPlayedAlone = false;
                turnsSinceCytotoxicPlayed = 0;
            }
        }
    }

    public override void AttackPlayer(Player player)
    {
        // If cytotoxic counter-attack is ready, deal extra damage
        if (cytotoxicWasPlayedAlone && turnsSinceCytotoxicPlayed >= cytotoxicAttackDelay)
        {
            player.TakeDamage(cytotoxicAttackDamage);
            Debug.Log($"{pathogenName}: Counter-attack against lone Cytotoxic! Deals {cytotoxicAttackDamage} HP damage.");
            cytotoxicWasPlayedAlone = false;
        }
        else
        {
            // Normal attack
            base.AttackPlayer(player);
        }
    }
}
