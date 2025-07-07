using UnityEngine;
using System;

public class PlayerHealth
{
    public int HP { get; private set; }
    public int MaxHP { get; private set; }

    public event Action<int, int> OnHealthChanged; // currentHP, maxHP
    public event Action OnPlayerDied;

    public PlayerHealth(int startingHP)
    {
        HP = startingHP;
        MaxHP = startingHP;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(0, damage);
        int oldHP = HP;
        HP -= actualDamage;
        if (HP < 0) HP = 0;
        
        Debug.Log($"PlayerHealth: Took {actualDamage} damage. HP: {oldHP} -> {HP}/{MaxHP}");
        
        // Play damage sound
        if (actualDamage > 0)
        {
            PlayPlayerDamageAudio(actualDamage);
        }
        
        OnHealthChanged?.Invoke(HP, MaxHP);
        if (HP <= 0)
        {
            Debug.Log("PlayerHealth: Player died!");
            PlayPlayerDeathAudio();
            OnPlayerDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        int oldHP = HP;
        HP = Mathf.Min(MaxHP, HP + amount);
        int actualHealing = HP - oldHP;
        
        Debug.Log($"PlayerHealth: Healed {actualHealing} HP. HP: {oldHP} -> {HP}/{MaxHP}");
        
        // Play heal sound
        if (actualHealing > 0)
        {
            PlayPlayerHealAudio(actualHealing);
        }
        
        OnHealthChanged?.Invoke(HP, MaxHP);
    }

    public void IncreaseMaxHP(int amount)
    {
        MaxHP += amount;
        Debug.Log($"PlayerHealth: Max HP increased by {amount}. New Max HP: {MaxHP}");
        OnHealthChanged?.Invoke(HP, MaxHP);
    }

    public bool IsAlive => HP > 0;
    public float HealthPercentage => MaxHP > 0 ? (float)HP / MaxHP : 0f;

    #region Audio Methods
    
    private void PlayPlayerDamageAudio(int damage)
    {
        // Try to use AudioHelper if available, otherwise log
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPlayerDamageSound");
                method?.Invoke(null, new object[] { damage });
            }
            else
            {
                Debug.Log($"[AUDIO] Player took {damage} damage - play damage sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log($"[AUDIO] Player took {damage} damage - play damage sound");
        }
    }
    
    private void PlayPlayerDeathAudio()
    {
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPlayerDeathSound");
                method?.Invoke(null, null);
            }
            else
            {
                Debug.Log("[AUDIO] Player died - play death sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log("[AUDIO] Player died - play death sound");
        }
    }
    
    private void PlayPlayerHealAudio(int healAmount)
    {
        try
        {
            var audioHelperType = System.Type.GetType("AudioHelper");
            if (audioHelperType != null)
            {
                var method = audioHelperType.GetMethod("PlayPlayerHealSound");
                method?.Invoke(null, new object[] { healAmount });
            }
            else
            {
                Debug.Log($"[AUDIO] Player healed {healAmount} HP - play heal sound");
            }
        }
        catch (System.Exception)
        {
            Debug.Log($"[AUDIO] Player healed {healAmount} HP - play heal sound");
        }
    }
    
    #endregion
}
