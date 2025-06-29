using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathogenManager : MonoBehaviour
{
    public List<PathogenSO> pathogens;

    void Start()
    {
        pathogens = new List<PathogenSO>();

        // Example: Create pathogens
        
    }

    public void PathogenAttack(Player player)
    {
        foreach (var pathogen in pathogens)
        {
            pathogen.AttackPlayer(player);
        }
    }

    public bool AllPathogensDefeated()
    {
        return pathogens.All(p => p.health <= 0);
    }
}
