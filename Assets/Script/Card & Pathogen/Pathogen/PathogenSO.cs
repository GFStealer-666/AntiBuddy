using UnityEngine;

public abstract class PathogenSO : ScriptableObject
{
    [Space(10)]
    public string pathogenName;
    public int maxHitPoints;
    public int attackPower;
    
    [Space(10)]
    [Header("Turn System")]
    protected int currentTurn = 0;

    public virtual void TakeDamage(int damage)
    {
        maxHitPoints -= damage;
        if (maxHitPoints < 0) maxHitPoints = 0;
        Debug.Log($"{pathogenName} took {damage} damage. HP left: {maxHitPoints}");
    }

    // New method to check if a card type is blocked by this pathogen
    public virtual bool IsCardBlocked(System.Type cardType)
    {
        return false; // By default, no cards are blocked
    }

    // New method to determine if pathogen should attack this turn
    public virtual bool ShouldAttackThisTurn()
    {
        return true; // By default, attack every turn
    }

    // Called at the start of each turn
    public virtual void OnTurnStart(System.Collections.Generic.List<CardSO> playedCards)
    {
        currentTurn++;
    }

    public virtual void AttackPlayer(Player player)
    {
        if (ShouldAttackThisTurn())
        {
            player.TakeDamage(attackPower);
            Debug.Log($"{pathogenName} attacks the player for {attackPower} HP.");
        }
        else
        {
            Debug.Log($"{pathogenName} is not attacking this turn.");
        }
    }

    public virtual void Heal(int amount)
    {
        maxHitPoints += amount;
        Debug.Log($"{pathogenName} healed for {amount} HP. Current HP: {maxHitPoints}");
    }
}
