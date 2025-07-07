using UnityEngine;
using System;

/// <summary>
/// Handles pathogen health, damage, and death logic only
/// </summary>
public class PathogenHealth
{
    private PathogenData pathogenData;
    
    public event Action<int> OnHealthChanged;
    public event Action<int> OnDamageTaken;
    public event Action OnPathogenDied;
    
    public PathogenHealth(PathogenData data)
    {
        pathogenData = data;
    }
    
    public void TakeDamage(int damage)
    {
        if (!pathogenData.isAlive || damage <= 0) return;
        
        int actualDamage = Mathf.Min(damage, pathogenData.currentHitPoints);
        pathogenData.currentHitPoints -= actualDamage;
        
        // Play damage sound
        PlayPathogenDamageAudio(pathogenData.PathogenName, actualDamage);
        
        if (pathogenData.currentHitPoints <= 0)
        {
            pathogenData.currentHitPoints = 0;
            pathogenData.isAlive = false;
            
            // Play death sound
            PlayPathogenDeathAudio(pathogenData.PathogenName);
            OnPathogenDied?.Invoke();
        }
        
        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(pathogenData.currentHitPoints);
        
        Debug.Log($"{pathogenData.PathogenName} took {actualDamage} damage. HP: {pathogenData.currentHitPoints}/{pathogenData.MaxHitPoints}");
    }
    
    public void Heal(int amount)
    {
        if (!pathogenData.isAlive || amount <= 0) return;
        
        int oldHealth = pathogenData.currentHitPoints;
        pathogenData.currentHitPoints = Mathf.Min(pathogenData.currentHitPoints + amount, pathogenData.MaxHitPoints);
        
        int actualHealing = pathogenData.currentHitPoints - oldHealth;
        
        if (actualHealing > 0)
        {
            // Play heal sound
            PlayPathogenHealAudio(pathogenData.PathogenName, actualHealing);
            
            OnHealthChanged?.Invoke(pathogenData.currentHitPoints);
            Debug.Log($"{pathogenData.PathogenName} healed for {actualHealing} HP. Current HP: {pathogenData.currentHitPoints}/{pathogenData.MaxHitPoints}");
        }
    }
    
    public bool IsAlive()
    {
        return pathogenData.isAlive && pathogenData.currentHitPoints > 0;
    }
    
    public int GetCurrentHealth()
    {
        return pathogenData.currentHitPoints;
    }
    
    public int GetMaxHealth()
    {
        return pathogenData.MaxHitPoints;
    }
    
    public float GetHealthPercentage()
    {
        return pathogenData.GetHealthPercentage();
    }
    
    #region Audio Methods
    
    private void PlayPathogenDamageAudio(string pathogenName, int damage)
    {
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPathogenDamageSound");
                method?.Invoke(null, new object[] { pathogenName, damage });
            }
            else
            {
                Debug.Log($"[AUDIO] Pathogen {pathogenName} took {damage} damage - play damage sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} took {damage} damage - play damage sound");
        }
    }
    
    private void PlayPathogenDeathAudio(string pathogenName)
    {
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPathogenDeathSound");
                method?.Invoke(null, new object[] { pathogenName });
            }
            else
            {
                Debug.Log($"[AUDIO] Pathogen {pathogenName} died - play death sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} died - play death sound");
        }
    }
    
    private void PlayPathogenHealAudio(string pathogenName, int healAmount)
    {
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPathogenHealSound");
                method?.Invoke(null, new object[] { pathogenName, healAmount });
            }
            else
            {
                Debug.Log($"[AUDIO] Pathogen {pathogenName} healed {healAmount} HP - play heal sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} healed {healAmount} HP - play heal sound");
        }
    }
    
    #endregion
}