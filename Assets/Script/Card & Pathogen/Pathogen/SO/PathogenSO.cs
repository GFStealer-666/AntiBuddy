using UnityEngine;

public abstract class PathogenSO : ScriptableObject
{
    public string pathogenName;
    public int health;
    public int attackPower;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        Debug.Log($"{pathogenName} took {damage} damage. HP left: {health}");
    }

    public virtual void AttackPlayer(Player player)
    {
        player.TakeDamage(attackPower);
        Debug.Log($"{pathogenName} attacks the player for {attackPower} HP.");
    }
}
