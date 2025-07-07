using UnityEngine;

/// <summary>
/// Simple audio helper that can be used before the full AudioManager is set up
/// This provides a bridge between damage systems and audio without compilation dependencies
/// </summary>
public static class AudioHelper
{
    /// <summary>
    /// Play player damage sound with fallback to debug logging
    /// </summary>
    public static void PlayPlayerDamageSound(int damage)
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPlayerDamageSound(damage);
        }
        else
        {
            Debug.Log($"[AUDIO] Player took {damage} damage - play damage sound (AudioManager not found)");
        }
    }
    
    /// <summary>
    /// Play player death sound with fallback to debug logging
    /// </summary>
    public static void PlayPlayerDeathSound()
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPlayerDeathSound();
        }
        else
        {
            Debug.Log("[AUDIO] Player died - play death sound (AudioManager not found)");
        }
    }
    
    /// <summary>
    /// Play player heal sound with fallback to debug logging
    /// </summary>
    public static void PlayPlayerHealSound(int healAmount)
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPlayerHealSound(healAmount);
        }
        else
        {
            Debug.Log($"[AUDIO] Player healed {healAmount} HP - play heal sound (AudioManager not found)");
        }
    }
    
    /// <summary>
    /// Play pathogen damage sound with fallback to debug logging
    /// </summary>
    public static void PlayPathogenDamageSound(string pathogenName, int damage)
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPathogenDamageSound(pathogenName, damage);
        }
        else
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} took {damage} damage - play damage sound (AudioManager not found)");
        }
    }
    
    /// <summary>
    /// Play pathogen death sound with fallback to debug logging
    /// </summary>
    public static void PlayPathogenDeathSound(string pathogenName)
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPathogenDeathSound(pathogenName);
        }
        else
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} died - play death sound (AudioManager not found)");
        }
    }
    
    /// <summary>
    /// Play pathogen heal sound with fallback to debug logging
    /// </summary>
    public static void PlayPathogenHealSound(string pathogenName, int healAmount)
    {
        var audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayPathogenHealSound(pathogenName, healAmount);
        }
        else
        {
            Debug.Log($"[AUDIO] Pathogen {pathogenName} healed {healAmount} HP - play heal sound (AudioManager not found)");
        }
    }
}
